namespace SwappyBot.Commands.Status
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Discord;
    using Discord.Interactions;
    using FluentResults;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using SwappyBot.Configuration;
    using SwappyBot.EntityFramework;
    using SwappyBot.Infrastructure;

    public class Status : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly BotContext _dbContext;
        private readonly BotConfiguration _configuration;
        
        public Status(
            ILogger<Status> logger,
            IOptions<BotConfiguration> options,
            IHttpClientFactory httpClientFactory,
            BotContext dbContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _configuration = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        [EnabledInDm(false)]
        [SlashCommand(SlashCommands.Status, "Get the status of your swap.")]
        public async Task Execute(
            [Summary(description: "Reference to check")] string reference)
        {
            await DeferAsync(ephemeral: true);

            var stateId = reference.ToLowerInvariant();

            _logger.LogInformation(
                "[{StateId}] Command /{Command} {Reference}, Server: {Server}, User: {User}",
                stateId,
                SlashCommands.Status,
                reference,
                Context.Guild.Name,
                Context.User.Username);

            var swapState = await GetSwap(stateId);

            if (swapState.IsFailed)
            {
                // Send support message
                await ModifyOriginalResponseAsync(x =>
                {
                    x.Flags = MessageFlags.SuppressEmbeds;
                    x.Content =
                        "💩 Something has gone wrong, you can try again, or contact us on [Discord](https://discord.gg/wwzZ7a7aQn) for support. (**" +
                        swapState.BuildError() + 
                        "**)";
                });

                return;
            }

            var swap = swapState.Value;

            if (swap.SwapAccepted.IsEmptyDate() && swap.SwapCancelled.IsEmptyDate())
            {
                await ModifyOriginalResponseAsync(x =>
                {
                    x.Flags = MessageFlags.SuppressEmbeds;
                    x.Content =
                        "🙈 Your swap is **not yet started**! You are still busy preparing one, finish it first before checking the status.";
                });

                return;
            }

            if (swap.SwapAccepted.IsEmptyDate() && !swap.SwapCancelled.IsEmptyDate())
            {
                await ModifyOriginalResponseAsync(x =>
                {
                    x.Flags = MessageFlags.SuppressEmbeds;
                    x.Content =
                        "🙈 Your swap is **cancelled**! Start a new one by typing `/swap`.";
                });

                return;
            }

            if (string.IsNullOrWhiteSpace(swap.DepositChannel))
            {
                // Send support message
                await ModifyOriginalResponseAsync(x =>
                {
                    x.Flags = MessageFlags.SuppressEmbeds;
                    x.Content =
                        "💩 Something has gone wrong, you can try again, or contact us on [Discord](https://discord.gg/wwzZ7a7aQn) for support. (**" +
                        "No deposit channel found" + 
                        "**)";
                });

                return;
            }
            
            await SendChainflipStatus(swap);
        }

        private async Task<Result<SwapState>> GetSwap(string stateId)
        {
            try
            {
                var swap = await _dbContext
                    .SwapState
                    .FirstOrDefaultAsync(x => x.StateId == stateId);

                return swap == null 
                    ? Result.Fail("Swap does not exist.") 
                    : Result.Ok(swap);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "Could not fetch swap");
                
                return Result.Fail("Something has gone wrong while fetching swap.");
            }
        }

        private async Task SendChainflipStatus(SwapState swap)
        {
            var statusResponse = await StatusProvider.GetStatusAsync(
                _logger,
                _configuration,
                _httpClientFactory,
                swap.DepositChannel!);
            
            if (statusResponse.IsFailed)
            {
                // Send support message
                await ModifyOriginalResponseAsync(x =>
                {
                    x.Flags = MessageFlags.SuppressEmbeds;
                    x.Content =
                        "💩 Something has gone wrong, you can try again, or contact us on [Discord](https://discord.gg/wwzZ7a7aQn) for support. (**" +
                        statusResponse.BuildError() + 
                        "**)";
                });

                return;
            }

            try
            {
                swap.SwapStatus = statusResponse.Value.Body;
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "Could not update swap status");
            }

            var status = statusResponse.Value.Status;

            var message = string.Empty;

            if (string.Equals(status.State, "completed", StringComparison.OrdinalIgnoreCase))
            {
                // the transaction has been confirmed beyond our safety margin
                message =
                    $"🎉 Your swap is **completed** and has been **received in the destination wallet**, you can view it on **[Chainflip's official website]({BuildUrl(swap, status)})**. *(reference {swap.StateId})*";
            }
            else if (status.DepositChannelStatus.IsExpired)
            {
                message =
                    $"⌛ Your swap has **expired**, you can view it on **[Chainflip's official website]({BuildUrl(swap, status)})**. *(reference {swap.StateId})*";
            }
            else if (string.Equals(status.State, "waiting", StringComparison.OrdinalIgnoreCase))
            {
                // we are waiting for the user to send funds
                message =
                    $"⌛ Your swap is **waiting for funds**, you can view it on **[Chainflip's official website]({BuildUrl(swap, status)})**. *(reference {swap.StateId})*";
            }
            else if (string.Equals(status.State, "receiving", StringComparison.OrdinalIgnoreCase))
            {
                // funds have been received and the swap is being performed
                message =
                    $"⚙️ Your swap has **received funds** and is **being performed**, you can view it on **[Chainflip's official website]({BuildUrl(swap, status)})**. *(reference {swap.StateId})*";
            }
            else if (string.Equals(status.State, "swapping", StringComparison.OrdinalIgnoreCase))
            {
                // funds have been swapped through the AMM and awaiting scheduling
                message =
                    $"⚙️ Your swap has **been swapped** and is **awaiting scheduling**, you can view it on **[Chainflip's official website]({BuildUrl(swap, status)})**. *(reference {swap.StateId})*";
            }
            else if (string.Equals(status.State, "sending", StringComparison.OrdinalIgnoreCase))
            {
                // a validator has been requested to send the funds
                message =
                    $"🏦 Your swap has **been scheduled** and is **awaiting sending**, you can view it on **[Chainflip's official website]({BuildUrl(swap, status)})**. *(reference {swap.StateId})*";
                
            }
            else if (string.Equals(status.State, "sent", StringComparison.OrdinalIgnoreCase))
            {
                // the transaction has been included in a block on the destination chain
                message =
                    $"🪙 Your swap has been **sent on the destination chain**, you can view it on **[Chainflip's official website]({BuildUrl(swap, status)})**. *(reference {swap.StateId})*";
            }
            else if (string.Equals(status.State, "failed", StringComparison.OrdinalIgnoreCase))
            {
                // the transaction could not be successfully completed
                message = 
                    $"💩 Something has gone wrong, please contact us on [Discord](https://discord.gg/wwzZ7a7aQn) for support. *(reference {swap.StateId})*";
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                await ModifyOriginalResponseAsync(x =>
                {
                    x.Flags = MessageFlags.SuppressEmbeds;
                    x.Content = message;
                });
            }
            else
            {
                await ModifyOriginalResponseAsync(x =>
                {
                    x.Flags = MessageFlags.SuppressEmbeds;
                    x.Content =
                        "💩 Something has gone wrong, you can try again, or contact us on [Discord](https://discord.gg/wwzZ7a7aQn) for support. (**" +
                        "Swap status could not be parsed" + 
                        "**)";
                });
            }
        }

        private string BuildUrl(SwapState swap, SwapStatus status) 
            => string.IsNullOrWhiteSpace(status.SwapId)
                ? $"{_configuration.DepositChannelUrl}/{swap.DepositChannel}"
                : $"{_configuration.CompletedSwapUrl}/{status.SwapId}";
    }
}