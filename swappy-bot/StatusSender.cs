namespace SwappyBot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Discord;
    using Discord.WebSocket;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using SwappyBot.Commands;
    using SwappyBot.Configuration;
    using SwappyBot.EntityFramework;

    public class StatusSender
    {
        private readonly ILogger<StatusSender> _logger;
        private readonly BotConfiguration _configuration;
        private readonly DiscordSocketClient _client;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly BotContext _dbContext;

        private readonly Emoji _checkEmoji = new("✅");

        private readonly PeriodicTimer _timer;
        private CancellationTokenSource? _cts;
        private Task? _timerTask;
        
        public StatusSender(
            ILogger<StatusSender> logger,
            IOptions<BotConfiguration> options,
            DiscordSocketClient client,
            IHttpClientFactory httpClientFactory,
            BotContext dbContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = options.Value ?? throw new ArgumentNullException(nameof(options));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(_configuration.StatusCheckIntervalInSeconds.Value));
        }
        
        public void Start() => _timerTask = RunAsync();

        public async Task Stop()
        {
            if (_timerTask is null)
                return;
            
            if (_cts is null)
                return;

            _cts.Cancel();
            await _timerTask;
            _cts.Dispose();

            _timerTask = null;
            _logger.LogInformation("Stopped StatusSender");
        }

        private async Task RunAsync()
        {
            try
            {
                _logger.LogInformation("Started StatusSender");
                _cts = new CancellationTokenSource();

                while (await _timer.WaitForNextTickAsync(_cts.Token))
                {
                    // Get swaps which are not replied to
                    var swaps = await _dbContext
                        .SwapState
                        .Where(x => 
                            x.DepositChannel != null &&
                            x.AnnouncementIds != null &&
                            x.Replied != null && x.Replied.Value == false)
                        .ToListAsync();
                    
                    if (swaps.Count == 0)
                        continue;

                    _logger.LogInformation(
                        "Processing {Count} swaps", 
                        swaps.Count);
                    
                    await ProcessSwaps(swaps);
                }
            }
            catch (OperationCanceledException) {}
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "StatusSender had a problem");
            }
        }

        private async Task ProcessSwaps(List<SwapState> swaps)
        {
            // Get status for each swap
            foreach (var swap in swaps)
            {
                _logger.LogInformation(
                    "Processing swap {Reference}", 
                    swap.StateId);
                
                var status = await StatusProvider.GetStatusAsync(
                    _logger, 
                    _configuration, 
                    _httpClientFactory, 
                    swap.DepositChannel!);

                if (!status.IsFailed)
                {
                    swap.SwapStatus = status.Value.Body;

                    var s = status!.Value;
                    var swapCompleted = string.Equals(s.Status.State, "completed", StringComparison.OrdinalIgnoreCase);

                    // If completed, reply and mark it as done
                    if (swapCompleted)
                    {
                        await AnnounceSwapCompleted(swap, s.Status);
                        swap.Replied = true;
                    }
                    else
                    {
                        // If not completed and expired, mark as done
                        var swapExpired = s.Status.DepositChannelStatus.IsExpired;
                        if (swapExpired)
                            swap.Replied = true;
                    }

                    await _dbContext.SaveChangesAsync();
                }
                
                await Task.Delay(2000);
            }
        }

        private async Task AnnounceSwapCompleted(
            SwapState swapState, 
            SwapStatus status)
        {
            var messages = swapState
                .AnnouncementIds!
                .Split('|')
                .Select(ulong.Parse)
                .ToList();

            var assetFrom = Assets.SupportedAssets[swapState.AssetFrom];
            var assetTo = Assets.SupportedAssets[swapState.AssetTo];
            var amountFrom = status.DepositStatus.DepositAmount.Value;
            var amountTo = status.EgressStatus.EgressAmount.Value;
            
            for (var i = 0; i < _configuration.NotificationChannelIds.Length; i++)
            {
                var discordChannel = (SocketTextChannel)await _client.GetChannelAsync(_configuration.NotificationChannelIds[i]);
                var discordMessage = await discordChannel.GetMessageAsync(messages[i]);
                
                await discordMessage.AddReactionAsync(_checkEmoji);
                
                await discordChannel.SendMessageAsync(
                    $"A swap from **{amountFrom.ToString(assetFrom.FormatString)} {assetFrom.Name} ({assetFrom.Ticker})** to **{amountTo.ToString(assetTo.FormatString)} {assetTo.Name} ({assetTo.Ticker})** was just **completed**! 🎉 \n" +
                    $"Use `/swap` to use my services as well. 😎",
                    messageReference: new MessageReference(discordMessage.Id, failIfNotExists: true));
                
                await Task.Delay(2000);
            }
            
            _logger.LogInformation(
                "[{StateId}] Announced completed swap from {Amount} {SourceAsset} to {DestinationAmount} {DestinationAsset} at {DestinationAddress} via {DepositAddress}",
                swapState.StateId,
                amountFrom,
                assetFrom.Ticker,
                amountTo,
                assetTo.Ticker,
                swapState.DestinationAddress,
                swapState.DepositAddress);
        }
    }
}