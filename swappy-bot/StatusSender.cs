namespace SwappyBot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Text.Json.Serialization;
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
        private readonly CancellationTokenSource _cts = new();
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
        
        public void Start()
        {
            _timerTask = RunAsync();
            _logger.LogInformation("Started StatusSender");
        }

        public async Task Stop()
        {
            if (_timerTask is null)
                return;

            _cts.Cancel();
            await _timerTask;
            _cts.Dispose();

            _logger.LogInformation("Stopped StatusSender");
        }

        private async Task RunAsync()
        {
            try
            {
                while (await _timer.WaitForNextTickAsync(_cts.Token))
                {
                    _logger.LogInformation("Getting swaps which are not processed yet");
                    
                    // Get swaps which are not replied to
                    var swaps = await _dbContext
                        .SwapState
                        .Where(x => 
                            x.DepositChannel != null &&
                            x.AnnouncementIds != null &&
                            x.Replied != null && x.Replied.Value == false)
                        .ToListAsync();

                    _logger.LogInformation(
                        "Processing {Count} swaps", 
                        swaps.Count);
                    
                    await ProcessSwaps(swaps);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "StatusSender had a problem");
            }
        }

        private async Task ProcessSwaps(List<SwapState> swaps)
        {
            using var client = _httpClientFactory.CreateClient("Broker");
            
            // Get status for each swap
            foreach (var swap in swaps)
            {
                _logger.LogInformation(
                    "Processing swap {Reference}", 
                    swap.StateId);
                
                var depositParts = swap.DepositChannel!.Split('-');

                var issuedBlock = depositParts[0];
                var network = depositParts[1];
                var channelId = depositParts[2];

                var statusRequest =
                    $"status-by-deposit-channel" +
                    $"?issuedBlock={issuedBlock}" +
                    $"&network={network}" +
                    $"&channelId={channelId}" +
                    $"&apiKey={_configuration.BrokerApiKey}";
                
                var statusResponse = await client.GetAsync(statusRequest);
                            
                if (statusResponse.IsSuccessStatusCode)
                {
                    var statusBody = await statusResponse.Content.ReadAsStringAsync();

                    _logger.LogInformation(
                        "Broker API returned {StatusCode}: {Body}\nRequest: {QuoteRequest}",
                        statusResponse.StatusCode,
                        statusBody,
                        statusRequest);

                    if (string.IsNullOrWhiteSpace(statusBody))
                        continue;

                    swap.SwapStatus = statusBody;
                    var status = JsonSerializer.Deserialize<SwapStatusResponse>(statusBody);
                    var swapCompleted = string.Equals(status!.Status.State, "COMPLETE", StringComparison.Ordinal);

                    // If completed, reply and mark it as done
                    if (swapCompleted)
                    {
                        await AnnounceSwapCompleted(swap, status.Status);
                        swap.Replied = true;
                    }
                    else
                    {
                        // If not completed and expired, mark as done
                        var swapExpired = status.Status.DepositChannelExpired;
                        if (swapExpired)
                            swap.Replied = true;
                    }

                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    var error = await statusResponse.Content.ReadAsStringAsync();
            
                    _logger.LogError(
                        "Broker API returned {StatusCode}: {Error}\nRequest: {QuoteRequest}",
                        statusResponse.StatusCode,
                        error,
                        statusRequest);
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
            var amountFrom = decimal.Parse(status.DepositAmount) / Convert.ToDecimal(Math.Pow(10, assetFrom.Decimals));
            var amountTo = decimal.Parse(status.DepositAmount) / Convert.ToDecimal(Math.Pow(10, assetTo.Decimals));
            
            for (var i = 0; i < _configuration.NotificationChannelIds.Length; i++)
            {
                var discordChannel = (SocketTextChannel)await _client.GetChannelAsync(_configuration.NotificationChannelIds[i]);
                var discordMessage = await discordChannel.GetMessageAsync(messages[i]);
                
                await discordMessage.AddReactionAsync(_checkEmoji);
                
                await discordChannel.SendMessageAsync(
                    $"A swap from **{amountFrom.ToString(assetFrom.FormatString)} {assetFrom.Name} ({assetFrom.Ticker})** to **{amountTo.ToString(assetTo.FormatString)} {assetTo.Name} ({assetTo.Ticker})** was just completed! 🎉 \n" +
                    $"Use `/swap` to use my services as well. 😎",
                    messageReference: new MessageReference(discordMessage.Id, failIfNotExists: true));
                
                await Task.Delay(2000);
            }
        }
    }

    public class SwapStatusResponse
    {
        [JsonPropertyName("id")] 
        public ulong Id { get; set; }

        [JsonPropertyName("status")]
        public SwapStatus Status { get; set; }
    }
    
    public class SwapStatus 
    {
        [JsonPropertyName("state")]
        public string State { get; set; }
        
        [JsonPropertyName("isDepositChannelExpired")]
        public bool DepositChannelExpired { get; set; }
        
        [JsonPropertyName("depositAmount")]
        public string? DepositAmount { get; set; }

        [JsonPropertyName("egressAmount")]
        public string? EgressAmount { get; set; }
    }
}