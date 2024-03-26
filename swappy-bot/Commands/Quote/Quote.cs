namespace SwappyBot.Commands.Quote
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Discord;
    using Discord.Interactions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using SwappyBot.Configuration;

    public class Quote : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly BotConfiguration _configuration;
        
        public Quote(
            ILogger<Quote> logger,
            IOptions<BotConfiguration> options,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _configuration = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        [EnabledInDm(false)]
        [SlashCommand(SlashCommands.Quote, "Get a quote between two assets.")]
        public async Task Execute(
            [Summary(description: "Amount to get a quote for")][MinValue(0)] decimal amount,
            [Summary(description: "Asset to swap from")] Assets.AllAssets from,
            [Summary(description: "Asset to swap to")] Assets.AllAssets to)
        {
            await DeferAsync(ephemeral: true);

            var stateId = $"0x{Guid.NewGuid():N}";

            _logger.LogInformation(
                "[{StateId}] Command /{Command} {Amount} {From} {To}, Server: {Server}, User: {User}",
                stateId,
                SlashCommands.Quote,
                amount,
                from,
                to,
                Context.Guild.Name,
                Context.User.Username);
            
            var assetFrom = Assets.SupportedAssets[from.ToString()];
            var assetTo = Assets.SupportedAssets[to.ToString()];
            
            if (from == to)
            {
                await ModifyOriginalResponseAsync(x =>
                    x.Content =
                        $"Swapping **{amount} {assetFrom.Ticker}** to itself would probably not make much sense!");

                return;
            }

            if (amount < assetFrom.MinimumAmount)
            {
                await ModifyOriginalResponseAsync(x =>
                    x.Content =
                        $"**{amount} {assetFrom.Ticker}** is below the minimum amount for **{assetFrom.Name}**, it needs to be greater than **{assetFrom.MinimumAmount} {assetFrom.Ticker}**.");

                return;
            }

            _logger.LogInformation(
                "[{StateId}] Getting quote from {Amount} {SourceAsset} to {DestinationAsset}",
                stateId,
                amount,
                assetFrom.Ticker,
                assetTo.Ticker);
            
            var quoteResult = await QuoteProvider.GetQuoteAsync(
                _logger,
                _configuration,
                _httpClientFactory,
                amount,
                assetFrom,
                assetTo);
            
            if (quoteResult.IsFailed)
            {
                // Send support message
                await ModifyOriginalResponseAsync(x =>
                {
                    x.Flags = MessageFlags.SuppressEmbeds;
                    x.Content =
                        "💩 Something has gone wrong, you can try again, or contact us on [Discord](https://discord.gg/wwzZ7a7aQn) for support. (**" +
                        quoteResult.BuildError() + 
                        "**)";
                });
                
                return;
            }
            
            var quote = quoteResult.Value;

            var quoteReceive = quote.EgressAmount;
            
            await ModifyOriginalResponseAsync(x =>
            {
                x.Components = BuildSwapButton(stateId);
                x.Content =
                    $"Swapping **{amount} {assetFrom.Ticker}** would give you about **{quoteReceive} {assetTo.Ticker}** after fees.";
            });
            
            _logger.LogInformation(
                "[{StateId}] Offered quote from {Amount} {SourceAsset} to {DestinationAmount} {DestinationAsset}",
                stateId,
                amount,
                assetFrom.Ticker,
                quoteReceive,
                assetTo.Ticker);
        }

        [ComponentInteraction("quote-swap-*")]
        public async Task QuoteSwap(
            string stateId)
        {
            await DeferAsync(ephemeral: true);

            _logger.LogInformation(
                "[{StateId}] Moving on to Swap Wizard",
                stateId);

            await ModifyOriginalResponseAsync(x =>
                x.Components = BuildSwapButton(stateId, false));
            
            var buttons = BuildIntroButtons(stateId);

            try
            {
                var channel = (ITextChannel)Context.Channel;
                var threadName = $"Swap {stateId}";
                var thread = await channel.CreateThreadAsync(
                    threadName,
                    ThreadType.PrivateThread,
                    ThreadArchiveDuration.OneWeek,
                    invitable: false);

                await thread.JoinAsync();

                await thread.AddUserAsync((IGuildUser)Context.User);

                _logger.LogInformation(
                    "[{StateId}] Created thread {Thread}",
                    stateId,
                    threadName);

                // await thread.SendMessageAsync(
                //     "Hi! It looks like you want to make a swap. I can help you with that.\n" +
                //     "Let me start of by mentioning this is a **private thread** and other users **cannot** see this.\n" +
                //     "\n" +
                //     "Start by selecting one of the following options:\n" +
                //     "* **Swap**: Start the swapping wizard.\n" +
                //     "* **Rates**: Take a look at the current swapping rates.\n" +
                //     "* **Help**: See some frequently asked questions.\n" +
                //     "* **About**: Learn more about this bot and get in touch.",
                //     components: buttons);

                await thread.SendMessageAsync(
                    "Hi! It looks like you want to make a swap. I can help you with that.\n\n" +
                    "Let me start of by mentioning this is a **private thread** and other users **cannot** see this.\n" +
                    "Additionally, I would like to mention this bot is a **community** bot and **not developed by Chainflip Labs GmbH**. " +
                    "By continuing you acknowledge and agree with the Disclaimer (click the button to view).\n\n" +
                    "My source can be reviewed at [GitHub in the `swappy` repository](https://github.com/CumpsD/swappy) to verify all steps.\n" +
                    "You can get in touch with my developers on [Discord](https://discord.gg/wwzZ7a7aQn) in case you have questions.\n" +
                    $"If you reach out for support, be sure to mention reference **{stateId}** to make it easier to help you.",
                    components: buttons,
                    flags: MessageFlags.SuppressEmbeds);

                await FollowupAsync(
                    $"Hi! I've created a new **private thread** with you called **Swap {stateId}** to help you with your swap.",
                    ephemeral: true);
            }
            catch (Exception e)
            {
                if (e is InteractionException && e.InnerException != null)
                {
                    await ModifyOriginalResponseAsync(x =>
                    {
                        x.Content =
                            $"💩 Hi! I've tried creating a new **private thread** with you but received an error: **{e.InnerException.Message}**. You can try again, or contact us on [Discord](https://discord.gg/wwzZ7a7aQn) for support.";
                        x.Flags = MessageFlags.SuppressEmbeds;
                    });
                }
                else
                {
                    await ModifyOriginalResponseAsync(x =>
                    {
                        x.Content =
                            $"💩 Hi! I've tried creating a new **private thread** with you but received an error: **{e.Message}**. You can try again, or contact us on [Discord](https://discord.gg/wwzZ7a7aQn) for support.";
                        x.Flags = MessageFlags.SuppressEmbeds;
                    });
                }
            }
        }
        
        private static MessageComponent BuildSwapButton(
            string stateId,
            bool swapEnabled = true)
        {
            var swapEmoji = new Emoji("🎉");

            return new ComponentBuilder()
                .WithButton(
                    "Start Swapping",
                    $"quote-swap-{stateId}",
                    ButtonStyle.Primary,
                    swapEmoji,
                    disabled: !swapEnabled)
                .Build();
        }
        
        private static MessageComponent BuildIntroButtons(
            string stateId,
            bool swapEnabled = true)
        {
            var swapEmoji = new Emoji("🚀");
            var disclaimerEmoji = new Emoji("ℹ️");

            return new ComponentBuilder()
                .WithButton(
                    "Swap",
                    $"swap-step1-{stateId}-false",
                    ButtonStyle.Primary,
                    swapEmoji,
                    disabled: !swapEnabled)
                .WithButton(
                    "Disclaimer",
                    $"disclaimer-initial-{stateId}",
                    ButtonStyle.Secondary,
                    disclaimerEmoji,
                    disabled: !swapEnabled)
                .Build();
        }
    }
}