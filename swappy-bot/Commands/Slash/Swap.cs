namespace SwappyBot.Commands.Slash
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using Discord;
    using Discord.API;
    using Discord.Interactions;
    using Discord.WebSocket;
    using SwappyBot.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Nethereum.Util;
    using SwappyBot.EntityFramework;
    using SwappyBot.Infrastructure;
    using Emoji = Discord.Emoji;

    public class Swap : InteractionModuleBase<SocketInteractionContext>
    {
        private static readonly Dictionary<string, AssetInfo> _supportedAssets = new()
        {
            { "btc", new AssetInfo(
                "btc", 
                "BTC", 
                "Bitcoin", 
                "Bitcoin", 
                8,
                0.0007,
                0.65, 
                [0.001, 0.002, 0.005, 0.01, 0.02, 0.05, 0.1, 0.2, 0.5],
                x => AddressValidator.IsValidAddress(x, "btc")) },
            
            { "dot", new AssetInfo(
                "dot", 
                "DOT", 
                "Polkadot", 
                "Polkadot", 
                10,
                4,
                4_100, 
                [10, 20, 50, 150, 300, 700, 1000, 2000, 4000],
                x => true) },
            
            { "eth", new AssetInfo(
                "eth", 
                "ETH", 
                "Ethereum",
                "Ethereum",
                18,
                0.01,
                11,
                [0.02, 0.04, 0.1, 0.2, 0.5, 1, 2, 5, 10],
                x => AddressUtil.Current.IsNotAnEmptyAddress(x) &&
                     AddressUtil.Current.IsValidAddressLength(x) &&
                     AddressUtil.Current.IsValidEthereumAddressHexFormat(x) &&
                     (AddressUtil.Current.IsChecksumAddress(x) ||  x == x.ToLower() || x[2..] == x[2..].ToUpper())) },
            
            { "flip", new AssetInfo(
                "flip",
                "FLIP",
                "Chainflip",
                "Ethereum",
                18,
                4,
                5_700, 
                [10, 20, 50, 150, 300, 1000, 2000, 4000, 5500],
                x => AddressUtil.Current.IsNotAnEmptyAddress(x) &&
                     AddressUtil.Current.IsValidAddressLength(x) &&
                     AddressUtil.Current.IsValidEthereumAddressHexFormat(x) &&
                     (AddressUtil.Current.IsChecksumAddress(x) ||  x == x.ToLower() || x[2..] == x[2..].ToUpper())) },
            
            { "usdc", new AssetInfo(
                "usdc", 
                "USDC", 
                "ethUSDC",
                "Ethereum",
                6,
                20,
                25_000,
                [25, 50, 100, 500, 1000, 2500, 5000, 10000, 20000],
                x => AddressUtil.Current.IsNotAnEmptyAddress(x) &&
                     AddressUtil.Current.IsValidAddressLength(x) &&
                     AddressUtil.Current.IsValidEthereumAddressHexFormat(x) &&
                     (AddressUtil.Current.IsChecksumAddress(x) || x == x.ToLower() || x[2..] == x[2..].ToUpper())) },
        };
        
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly BotConfiguration _configuration;
        private readonly BotContext _dbContext;

        public Swap(
            ILogger<Swap> logger,
            IOptions<BotConfiguration> options,
            IHttpClientFactory httpClientFactory,
            BotContext dbContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _configuration = options.Value ?? throw new ArgumentNullException(nameof(options));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        
        [EnabledInDm(false)]
        [SlashCommand(SlashCommands.Swap, "Perform a swap. This will create a new private thread to help you make a swap.")]
        public async Task Execute()
        {
            _logger.LogInformation(
                "Command /{Command}, User: {User}",
                SlashCommands.Swap,
                Context.User.Username);

            await DeferAsync(ephemeral: true);
            
            var stateId = $"0x{Guid.NewGuid():N}";

            var buttons = BuildIntroButtons(stateId);

            var channel = (ITextChannel)Context.Channel;
            var thread = await channel.CreateThreadAsync(
                $"Swap {stateId}",
                ThreadType.PrivateThread,
                ThreadArchiveDuration.OneWeek,
                invitable: false);

            await thread.JoinAsync();

            await thread.AddUserAsync((IGuildUser)Context.User);

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
                "Hi! It looks like you want to make a swap. I can help you with that.\n" +
                "Let me start of by mentioning this is a **private thread** and other users **cannot** see this.\n" +
                "Additionally, I would like to mention this bot is a **community** bot and not an official Chainflip-developed product.\n" +
                "My source can be reviewed at [GitHub in the `swappy` repository](https://github.com/CumpsD/swappy) to verify all steps.",
                components: buttons);
            
            await ModifyOriginalResponseAsync(x => 
                x.Content = $"Hi! I've created a new **private thread** with you called **Swap {stateId}** to help you with your swap.");
        }

        [ComponentInteraction("swap-step1-*")]
        public async Task SwapStep1(
            string stateId)
        {
            await DeferAsync();
            
            await ModifyOriginalResponseAsync(x =>
                x.Components = BuildIntroButtons(stateId, false));

            var assetFrom = BuildAssetSelect(
                "Select an asset to send",
                $"swap-step2-{stateId}");

            await Context.Channel.SendMessageAsync(
                "The following steps will guide you through your swap. At the end you will be presented with all details to review before you make the actual swap.");
            
            await Context.Channel.SendMessageAsync(
                "First of all, select the asset you want to swap **from**:", 
                components: assetFrom);
            
            await _dbContext.SwapState.AddAsync(
                new SwapState
                {
                    StateId = stateId,
                    SwapStarted = DateTimeOffset.UtcNow,
                    ServerId = Context.Guild.Id,
                    ServerName = Context.Guild.Name,
                    UserId = Context.User.Id,
                    UserName = Context.User.Username
                });
            
            await _dbContext.SaveChangesAsync();
        }

        [ComponentInteraction("swap-step2-*")]
        public async Task SwapStep2(
            string stateId)
        {
            await DeferAsync();

            var data = ((SocketMessageComponent)Context.Interaction).Data.Values.First();
            var assetFrom = _supportedAssets[data];
            
            await ModifyOriginalResponseAsync(x =>
                x.Components = BuildSelectedAssetSelect(
                    $"swap-step2-{stateId}",
                    assetFrom));
            
            var assetTo = BuildAssetSelect(
                "Select an asset to receive",
                $"swap-step3-{stateId}",
                assetFrom);

            await Context.Channel.SendMessageAsync(
                $"You've chosen to send **{assetFrom.Name} ({assetFrom.Ticker})**. Now select the asset you want to swap **to**:", 
                components: assetTo);

            var swapState = await _dbContext.SwapState.FindAsync(stateId);

            swapState.AssetFrom = assetFrom.Id;

            await _dbContext.SaveChangesAsync();
        }
        
        [ComponentInteraction("swap-step3-*")]
        public async Task SwapStep3(
            string stateId)
        {
            await DeferAsync();

            var swapState = await _dbContext.SwapState.FindAsync(stateId);

            var data = ((SocketMessageComponent)Context.Interaction).Data.Values.First();
            var assetFrom = _supportedAssets[swapState.AssetFrom];
            var assetTo = _supportedAssets[data];
            
            await ModifyOriginalResponseAsync(x =>
                x.Components = BuildSelectedAssetSelect(
                    $"swap-step3-{stateId}",
                    assetTo));
            
            var amount = BuildAmountButtons(
                $"swap-step4-{stateId}-for",
                assetFrom);
            
            await Context.Channel.SendMessageAsync(
                $"You've chosen to swap **{assetFrom.Name} ({assetFrom.Ticker})** to **{assetTo.Name} ({assetTo.Ticker})**. It's time to select the **amount** of **{assetFrom.Name} ({assetFrom.Ticker})** you want to swap.\n" +
                $"\n" +
                $"The following restrictions are currently in place:\n" +
                $"Minimum amount: **{assetFrom.MinimumAmount} {assetFrom.Ticker}**\n" +
                $"Maximum amount: **{assetFrom.MaximumAmount} {assetFrom.Ticker}**\n" +
                $"\n" +
                $"⚠️️ **Any funds sent outside of this range will be unrecoverable!** ⚠️",
                components: amount);
            
            swapState.AssetTo = assetTo.Id;

            await _dbContext.SaveChangesAsync();
        }

        [ComponentInteraction("swap-step4-*-for-*")]
        public async Task SwapStep4(
            string stateId,
            string amountText)
        {
            if (amountText.Equals("custom", StringComparison.InvariantCultureIgnoreCase))
            {
                var modal = new ModalBuilder()
                    .WithTitle("Custom Amount")
                    .WithCustomId($"swap-step4b-{stateId}")
                    .AddTextInput(
                        "Amount",
                        $"swap-step4b-{stateId}",
                        placeholder: "100",
                        maxLength: 8,
                        minLength: 1,
                        required: true)
                    .Build();

                await Context.Interaction.RespondWithModalAsync(modal);
                return;
            }
            
            await DeferAsync();
            
            var swapState = await _dbContext.SwapState.FindAsync(stateId);

            var assetFrom = _supportedAssets[swapState.AssetFrom];
            var assetTo = _supportedAssets[swapState.AssetTo];
            
            await ModifyOriginalResponseAsync(x =>
                x.Components = BuildAmountButtons(
                    $"swap-step4-{stateId}-for",
                    assetFrom,
                    false));

            if (!double.TryParse(amountText, out var amount))
            {
                // Amount is not a number
                var amountButtons = BuildAmountButtons(
                    $"swap-step4-{stateId}-for",
                    assetFrom);

                await Context.Channel.SendMessageAsync(
                    "❌ The amount you specified is not a number, please try again.",
                    components: amountButtons);

                return;
            }

            if (amount < assetFrom.MinimumAmount || amount > assetFrom.MaximumAmount)
            {
                // Amount is outside of valid ranges
                var amountButtons = BuildAmountButtons(
                    $"swap-step4-{stateId}-for",
                    assetFrom);

                await Context.Channel.SendMessageAsync(
                    "❌ The amount you specified is outside of the current restrictions, please try again.",
                    components: amountButtons);

                return;
            }
            
            var address = BuildAddressButton(
                $"swap-step5-{stateId}",
                assetTo);
            
            var quote = await GetQuoteAsync(
                amount,
                assetFrom,
                assetTo);
            
            var quoteReceive = double.Parse(quote.EgressAmount) / Math.Pow(10, assetTo.Decimals);
            
            await Context.Channel.SendMessageAsync(
                $"You've chosen to swap **{amountText} {assetFrom.Name} ({assetFrom.Ticker})** to **{assetTo.Name} ({assetTo.Ticker})**.\n" +
                $"This would result in you receiving about **{quoteReceive} {assetTo.Ticker}** after fees.\n" +
                $"\n" +
                $"Provide the **destination** address where you want to receive **{assetTo.Name} ({assetTo.Ticker})**.\n" +
                $"This is not the final step, there will be a review step at the end, before performing the final swap.\n" +
                $"\n" +
                $"⚠️ **Double check to ensure the destination address is 100% accurate! Nobody has the ability to recover funds if you input the incorrect destination address!** ⚠️",
                components: address);
            
            swapState.Amount = amount;

            await _dbContext.SaveChangesAsync();
        }

        [ModalInteraction("swap-step4b-*")]
        public async Task SwapStep4b(
            string stateId,
            DummyModal _)
        {
            var data = ((SocketModal)Context.Interaction).Data;
            var amount = data.Components.First().Value;

            await SwapStep4(
                stateId,
                amount);
        }

        [ComponentInteraction("swap-step5-*")]
        public async Task SwapStep5(
            string stateId)
        {
            var modal = new ModalBuilder()
                .WithTitle("Destination Address")
                .WithCustomId($"swap-step5b-{stateId}")
                .AddTextInput(
                    "Address",
                    $"swap-step5b-{stateId}",
                    placeholder: ".....",
                    maxLength: 1000,
                    minLength: 1,
                    required: true)
                .Build();

            await Context.Interaction.RespondWithModalAsync(modal);
        }

        [ModalInteraction("swap-step5b-*")]
        public async Task SwapStep5b(
            string stateId,
            DummyModal _)
        {
            var data = ((SocketModal)Context.Interaction).Data;
            var address = data.Components.First().Value;

            await DeferAsync();
            
            var swapState = await _dbContext.SwapState.FindAsync(stateId);

            var assetFrom = _supportedAssets[swapState.AssetFrom];
            var assetTo = _supportedAssets[swapState.AssetTo];

            await ModifyOriginalResponseAsync(x =>
                x.Components = BuildAddressButton(
                    $"swap-step5-{stateId}",
                    assetTo,
                    false));

            if (string.IsNullOrWhiteSpace(address))
            {
                var addressButton = BuildAddressButton(
                    $"swap-step5-{stateId}",
                    assetTo);

                await Context.Channel.SendMessageAsync(
                    $"❌ You need to provide a valid **{assetTo.Name} ({assetTo.Ticker})** address, please try again.",
                    components: addressButton);

                return;
            }
            
            if (!assetTo.AddressValidator(address))
            {
                var addressButton = BuildAddressButton(
                    $"swap-step5-{stateId}",
                    assetTo);

                await Context.Channel.SendMessageAsync(
                    $"❌ The address you specified is not a valid **{assetTo.Name} ({assetTo.Ticker})** address, please try again.",
                    components: addressButton);

                return;
            }            
            
            var quote = await GetQuoteAsync(
                swapState.Amount.Value,
                assetFrom,
                assetTo);
            
            var quoteTime = DateTimeOffset.UtcNow;
            var quoteDeposit = double.Parse(quote.IngressAmount) / Math.Pow(10, assetFrom.Decimals);
            var quoteReceive = double.Parse(quote.EgressAmount) / Math.Pow(10, assetTo.Decimals);
            var quoteRate = $"1 {assetFrom.Ticker} ≈ {quoteReceive / quoteDeposit} {assetTo.Ticker} | 1 {assetTo.Ticker} ≈ {quoteDeposit / quoteReceive} {assetFrom.Ticker}";
            // var quotePlatformFee = 0.01;
            // var quoteChainflipFee = 5.49;
            
            swapState.DestinationAddress = address;

            swapState.QuoteTime = quoteTime;
            swapState.QuoteDeposit = quoteDeposit;
            swapState.QuoteReceive = quoteReceive;
            swapState.QuoteRate = quoteRate;
            // swapState.QuotePlatformFee = quotePlatformFee;
            // swapState.QuoteChainflipFee = quoteChainflipFee;

            var swapButtons = BuildSwapButtons("swap-step6", stateId);
            
            await Context.Channel.SendMessageAsync(
                $"You are ready to perform a swap from **{assetFrom.Name} ({assetFrom.Ticker})** to **{assetTo.Name} ({assetTo.Ticker})**.\n" +
                $"\n" +
                $"Deposit: **{quoteDeposit} {assetFrom.Ticker}**\n" +
                $"Receive: **{quoteReceive} {assetTo.Ticker}**\n" +
                // $"Estimated Rate: **{quoteRate}**\n" +
                // $"Estimated Platform Fee: **${quotePlatformFee}**\n" +
                // $"Estimated Protocol & Gas Fee: **${quoteChainflipFee}**\n" +
                $"\n" +
                $"Destination Address: **{swapState.DestinationAddress}**\n" +
                $"Quote Date: **{quoteTime:yyyy-MM-dd HH:mm:ss}**\n" +
                $"\n" +
                $"⚠️ **Review your Destination Address and the amounts carefully!** ⚠️\n" +
                $"The final amount received may vary due to market conditions and network fees.\n" +
                $"\n" +
                $"Do you want to proceed with the swap?",
                components: swapButtons);
            
            await _dbContext.SaveChangesAsync();
        }

        [ComponentInteraction("swap-step6-ok-*")]
        public async Task SwapStep6Approve(
            string stateId)
        {
            await DeferAsync();
            
            await ModifyOriginalResponseAsync(x =>
                x.Components = BuildSwapButtons(
                    "swap-step6",
                    stateId, 
                    false));

            await Context.Channel.SendMessageAsync(
                "ℹ️ Chainflip is generating a **Deposit Address** for your swap, please wait a few seconds.");
            
            var typing = Context.Channel.EnterTypingState();

            var swapState = await _dbContext.SwapState.FindAsync(stateId);
       
            var assetFrom = _supportedAssets[swapState.AssetFrom];
            var assetTo = _supportedAssets[swapState.AssetTo];
            
            swapState.SwapAccepted = DateTimeOffset.UtcNow;
            
            await _dbContext.SaveChangesAsync();
            
            var deposit = await GetDepositChannelAsync(
                assetFrom,
                assetTo,
                swapState.DestinationAddress,
                _configuration.CommissionBps.Value);

            var depositAddress = deposit.Address;
            var depositBlock = deposit.IssuedBlock;
            var depositChannel = deposit.ChannelId;

            typing.Dispose();

            await Context.Channel.SendMessageAsync( 
                "✅ Chainflip has generated a **Deposit Address** for your swap, please review the following information carefully:\n" +
                "\n" +
                "❗❗ **Swap Information** ❗❗\n" +
                $"* Send **{swapState.QuoteDeposit} {assetFrom.Ticker}** within 1 hour, delays may result in a different rate.\n" +
                $"* Funds sent to an expired Deposit Address are **unrecoverable**.\n" +
                $"* Funds exceeding **{swapState.QuoteDeposit} {assetFrom.Ticker}** are processed at current market rates.\n" +
                $"\n" +
                $"⚠️ Minimum amount: **{assetFrom.MinimumAmount} {assetFrom.Ticker}**\n" +
                $"⚠️ Maximum amount: **{assetFrom.MaximumAmount} {assetFrom.Ticker}**\n" +
                $"⚠️ Any funds sent outside of this range will be lost!\n" +
                $"\n" +
                $"Swapping **{assetFrom.Name} ({assetFrom.Ticker})** to **{assetTo.Name} ({assetTo.Ticker})**\n" +
                $"Deposit: **{swapState.QuoteDeposit} {assetFrom.Ticker}**\n" +
                $"Receive: **{swapState.QuoteReceive} {assetTo.Ticker}**\n" +
                $"Destination Address: **{swapState.DestinationAddress}**\n" +
                $"\n" +
                $"📩 **Deposit Address**: **`{depositAddress}`**\n" +
                $"\n" +
                $"⚠️ Send **exactly {swapState.QuoteDeposit} {assetFrom.Ticker}** on the **{assetFrom.Network}** network.\n" +
                $"\n" +
                $"🧐 **Verify** the Deposit Address on [Chainflip's official website]({_configuration.ExplorerUrl}/{depositBlock}-{assetFrom.Network}-{depositChannel})!\n" +
                $"\n" +
                $"ℹ️ In case you have any questions, your swap has reference id **{stateId}**\n" +
                $"\n" +
                $"🙏 Thank you for using **swappy!** Feel free to type `/swap` in the main channel and come back any time! 😎");
            
            var threadChannel = (IThreadChannel)Context.Channel;

            await threadChannel.ModifyAsync(x =>
            {
                x.Locked = true;
                x.Name = $"[DONE] Swap {stateId}";
            });

            await threadChannel.LeaveAsync();
        }

        [ComponentInteraction("swap-step6-nok-*")]
        public async Task SwapStep6Cancel(
            string stateId)
        {
            await DeferAsync();
            
            await ModifyOriginalResponseAsync(x =>
                x.Components = BuildSwapButtons(
                    "swap-step6",
                    stateId, 
                    false));

            await Context.Channel.SendMessageAsync(
                "You **cancelled** your swap. No worries, feel free to type `/swap` in the main channel and come back any time! 😎");

            var threadChannel = (IThreadChannel)Context.Channel;

            await threadChannel.ModifyAsync(x =>
            {
                x.Locked = true;
                x.Name = $"[DONE] Swap {stateId}";
            });

            await threadChannel.LeaveAsync();
            
            var swapState = await _dbContext.SwapState.FindAsync(stateId);
       
            swapState.SwapCancelled = DateTimeOffset.UtcNow;
            
            await _dbContext.SaveChangesAsync();
        }

        private static MessageComponent BuildIntroButtons(
            string stateId,
            bool swapEnabled = true)
        {
            var swapEmoji = new Emoji("🚀");
            var pricesEmoji = new Emoji("💵");
            var helpEmoji = new Emoji("❓");
            var aboutEmoji = new Emoji("ℹ️");

            return new ComponentBuilder()
                .WithButton(
                    "Swap",
                    $"swap-step1-{stateId}",
                    ButtonStyle.Primary,
                    swapEmoji,
                    disabled: !swapEnabled)
                // .WithButton(
                //     "Rates",
                //     "get-rates",
                //     ButtonStyle.Secondary,
                //     pricesEmoji)
                // .WithButton(
                //     "Help",
                //     "get-help",
                //     ButtonStyle.Secondary,
                //     helpEmoji)
                // .WithButton(
                //     "About",
                //     "about",
                //     ButtonStyle.Secondary,
                //     aboutEmoji)
                .Build();
        }

        private static MessageComponent BuildAmountButtons(
            string id,
            AssetInfo asset,
            bool amountEnabled = true)
        {
            var builder = new ComponentBuilder();

            foreach (var amount in asset.SuggestedAmounts)
            {
                builder = builder
                    .WithButton(
                        $"{amount} {asset.Ticker}",
                        $"{id}-{amount}",
                        ButtonStyle.Secondary,
                        disabled: !amountEnabled);
            }

            builder = builder
                .WithButton(
                    "Custom",
                    $"{id}-custom",
                    ButtonStyle.Secondary,
                    disabled: !amountEnabled);
            
            return builder.Build();
        }

        private static MessageComponent BuildAddressButton(
            string id,
            AssetInfo asset,
            bool addressEnabled = true)
            => new ComponentBuilder()
                .WithButton(
                    $"Input destination {asset.Name} ({asset.Ticker}) address",
                    id,
                    ButtonStyle.Secondary,
                    disabled: !addressEnabled)
                .Build();

        private static MessageComponent BuildSwapButtons(
            string id,
            string stateId,
            bool enabled = true)
        {
            var swapEmoji = new Emoji("🚀");

            return new ComponentBuilder()
                .WithButton(
                    "Swap!",
                    $"{id}-ok-{stateId}",
                    ButtonStyle.Success,
                    swapEmoji,
                    disabled: !enabled)
                .WithButton(
                    "Cancel",
                    $"{id}-nok-{stateId}",
                    ButtonStyle.Danger,
                    disabled: !enabled)
                .Build();
        }

        private static MessageComponent BuildAssetSelect(
            string placeholder,
            string id,
            AssetInfo? excludeAsset = null,
            bool enabled = true)
        {
            var assetsSelect = new SelectMenuBuilder()
                .WithPlaceholder(placeholder)
                .WithCustomId(id)
                .WithMinValues(1)
                .WithMaxValues(1)
                .WithDisabled(!enabled);

            foreach (var asset in _supportedAssets.Keys)
            {
                if (excludeAsset != null && asset == excludeAsset.Id)
                    continue;
                
                var assetInfo = _supportedAssets[asset];

                assetsSelect = assetsSelect
                    .AddOption(
                        assetInfo.Ticker,
                        assetInfo.Id,
                        assetInfo.Name);
            }
            
            return new ComponentBuilder()
                .WithSelectMenu(assetsSelect)
                .Build();
        }

        private static MessageComponent BuildSelectedAssetSelect(
            string id,
            AssetInfo selectedAsset)
        {
            var assetsSelect = new SelectMenuBuilder()
                .WithPlaceholder(selectedAsset.Ticker)
                .WithCustomId(id)
                .WithDisabled(true)
                .AddOption("Dummy", "dummy", "Dummy");
            
            return new ComponentBuilder()
                .WithSelectMenu(assetsSelect)
                .Build();
        }

        private async Task<QuoteResponse?> GetQuoteAsync(
            double amount, 
            AssetInfo assetFrom, 
            AssetInfo assetTo)
        {
            // https://chainflip-swap.chainflip.io/quote?amount=1500000000000000000&srcAsset=ETH&destAsset=BTC
            using var client = _httpClientFactory.CreateClient("Quote");

            var convertedAmount = amount * Math.Pow(10, assetFrom.Decimals);
            
            var quote = await client.GetFromJsonAsync<QuoteResponse>(
                $"quote?amount={convertedAmount}&srcAsset={assetFrom.Ticker}&destAsset={assetTo.Ticker}");

            quote.IngressAmount = convertedAmount.ToString(CultureInfo.InvariantCulture);
            
            return quote;
        }
        
        private async Task<DepositAddressResult?> GetDepositChannelAsync(
            AssetInfo assetFrom, 
            AssetInfo assetTo,
            string destinationAddress,
            int commissionBps)
        {
            using var client = _httpClientFactory.CreateClient("Deposit");

            var response = await client.PostAsJsonAsync(
                string.Empty,
                new DepositAddressRequest(assetFrom, assetTo, destinationAddress, commissionBps));

            var result = await response.Content.ReadFromJsonAsync<DepositAddress>();
            
            return result.Result;
        }
    }

    public record AssetInfo(
        string Id, 
        string Ticker,
        string Name, 
        string Network, 
        int Decimals,
        double MinimumAmount, 
        double MaximumAmount,
        double[] SuggestedAmounts,
        Func<string, bool> AddressValidator);

    public class DummyModal : IModal
    {
        public string Title { get; }
    }

    public class QuoteResponse
    {
        [JsonIgnore]
        public string IngressAmount { get; set; }
        
        [JsonPropertyName("egressAmount")]
        public string EgressAmount { get; set; }

        [JsonPropertyName("intermediateAmount")]
        public string IntermediateAmount { get; set; }
        
        [JsonPropertyName("includedFees")]
        public QuoteFees[] Fees { get; set; }
    }

    public class QuoteFees
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        [JsonPropertyName("chain")]
        public string Chain { get; set; }
        
        [JsonPropertyName("asset")]
        public string Asset { get; set; }
        
        [JsonPropertyName("amount")]
        public string Amount { get; set; }
    }

    public class DepositAddress
    {
        [JsonPropertyName("result")]
        public DepositAddressResult Result { get; set; }
    }

    public class DepositAddressResult
    {
        [JsonPropertyName("address")]
        public string Address { get; set; }
        
        [JsonPropertyName("issued_block")]
        public double IssuedBlock { get; set; }
        
        [JsonPropertyName("channel_id")]
        public double ChannelId { get; set; }
    }

    public class DepositAddressRequest
    {
        [JsonPropertyName("id")]
        public string Id { get; } = "1";

        [JsonPropertyName("jsonrpc")] 
        public string JsonRpcVersion { get; } = "2.0";

        [JsonPropertyName("method")] 
        public string Method { get; } = "broker_request_swap_deposit_address";

        [JsonPropertyName("params")] 
        public dynamic[] Parameters { get; }
        
        public DepositAddressRequest(
            AssetInfo assetFrom, 
            AssetInfo assetTo, 
            string destinationAddress, 
            int commissionBps)
        {
            Parameters =
            [
                new ChainId(assetFrom.Network, assetFrom.Ticker),
                new ChainId(assetTo.Network, assetTo.Ticker),
                destinationAddress,
                commissionBps
            ];
        }
    }

    public class ChainId
    {
        [JsonPropertyName("chain")] 
        public string Network { get; }
        
        [JsonPropertyName("asset")] 
        public string Asset { get; }

        public ChainId(
            string network,
            string asset)
        {
            Network = network;
            Asset = asset;
        }
    }
}