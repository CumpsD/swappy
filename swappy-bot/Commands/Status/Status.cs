namespace SwappyBot.Commands.Status
{
    using System;
    using System.Linq;
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
                "[{StateId}] Command /{Command}, Server: {Server}, User: {User}",
                stateId,
                SlashCommands.Status,
                Context.Guild.Name,
                Context.User.Username);

            var swap = await GetSwap(stateId);

            if (swap.IsFailed)
            {
                // Send support message
                await ModifyOriginalResponseAsync(x =>
                {
                    x.Flags = MessageFlags.SuppressEmbeds;
                    x.Content =
                        "💩 Something has gone wrong, you can try again, or contact us on [Discord](https://discord.gg/wwzZ7a7aQn) for support. (**" +
                        swap.BuildError() + 
                        "**)";
                });

                return;
            }

            var statusResponse = await StatusProvider.GetStatusAsync(
                _logger,
                _configuration,
                _httpClientFactory,
                swap.Value.DepositChannel!);
            
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
                swap.Value.SwapStatus = statusResponse.Value.Body;
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
            var swapCompleted = string.Equals(status.State, "COMPLETE", StringComparison.Ordinal);
            var swapExpired = status.DepositChannelExpired;

            if (swapCompleted)
            {
                var url = $"{_configuration.CompletedSwapUrl}/{status.SwapId}";
                message = 
                    $"🎉 Your swap is **completed**, you can view it on **[Chainflip's official website]({url})**.";
            }
            else if (swapExpired)
            {
                if (status.SwapId != null)
                {
                    // https://scan.chainflip.io/swaps
                    var url = $"{_configuration.CompletedSwapUrl}/{status.SwapId}";
                    message =
                        $"⌛ Your swap has **expired**, you can view it on **[Chainflip's official website]({url})**.";
                }
                else
                {
                    // https://scan.chainflip.io/channels
                    var url = $"{_configuration.DepositChannelUrl}/{swap.Value.DepositChannel}";
                    message =
                        $"⌛ Your swap has **expired**, you can view it on **[Chainflip's official website]({url})**.";
                }
            }
            else if (string.Equals(status.State, "AWAITING_DEPOSIT", StringComparison.Ordinal))
            {
                // we are waiting for the user to send funds
                var url = $"{_configuration.DepositChannelUrl}/{swap.Value.DepositChannel}";
                message =
                    $"⌛ Your swap is **waiting for funds**, you can view it on **[Chainflip's official website]({url})**.";
            }
            else if (string.Equals(status.State, "DEPOSIT_RECEIVED", StringComparison.Ordinal))
            {
                // funds have been received and the swap is being performed
                var url = $"{_configuration.DepositChannelUrl}/{swap.Value.DepositChannel}";
                message =
                    $"⚙️ Your swap has received funds and is **being performed**, you can view it on **[Chainflip's official website]({url})**.";
            }
            else if (string.Equals(status.State, "SWAP_EXECUTED", StringComparison.Ordinal))
            {
                // funds have been swapped through the AMM and awaiting scheduling
                var url = $"{_configuration.DepositChannelUrl}/{swap.Value.DepositChannel}";
                message =
                    $"⚙️ Your swap has been swapped and is **awaiting scheduling**, you can view it on **[Chainflip's official website]({url})**.";
            }
            else if (string.Equals(status.State, "BROADCAST_REQUESTED", StringComparison.Ordinal))
            {
                // a validator has been requested to send the funds
                var url = $"{_configuration.DepositChannelUrl}/{swap.Value.DepositChannel}";
                message =
                    $"🏦 Your swap has been scheduled and is **awaiting sending**, you can view it on **[Chainflip's official website]({url})**.";
                
            }
            else if (string.Equals(status.State, "BROADCASTED", StringComparison.Ordinal))
            {
                // the transaction has been included in a block on the destination chain
                var url = $"{_configuration.DepositChannelUrl}/{swap.Value.DepositChannel}";
                message =
                    $"🪙 Your swap has been **sent on the destination chain**, you can view it on **[Chainflip's official website]({url})**.";
            }
            else if (string.Equals(status.State, "BROADCAST_ABORTED", StringComparison.Ordinal))
            {
                // the transaction could not be successfully completed
                message = "💩 Something has gone wrong, please contact us on [Discord](https://discord.gg/wwzZ7a7aQn) for support.";
            }
            else if (string.Equals(status.State, "COMPLETE", StringComparison.Ordinal))
            {
                // the transaction has been confirmed beyond our safety margin
                var url = $"{_configuration.CompletedSwapUrl}/{status.SwapId}";
                message =
                    $"🪙 Your swap has been **received in the destination wallet**, you can view it on **[Chainflip's official website]({url})**.";
            }

            if (string.IsNullOrWhiteSpace(message))
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
    }
}