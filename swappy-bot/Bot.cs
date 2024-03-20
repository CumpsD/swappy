namespace SwappyBot
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Discord;
    using Discord.Interactions;
    using Discord.WebSocket;
    using SwappyBot.Configuration;
    using SwappyBot.Infrastructure;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Serilog;

    public class Bot
    {
        private readonly ILogger<Bot> _logger;
        private readonly StatusSender _statusSender;
        private readonly DiscordSocketClient _client;
        private readonly BotConfiguration _configuration;

        public Bot(
            ILogger<Bot> logger,
            IOptions<BotConfiguration> options,
            StatusSender statusSender,
            DiscordSocketClient client,
            InteractionHandler interactionHandler,
            InteractionService interactionService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _statusSender = statusSender ?? throw new ArgumentNullException(nameof(statusSender));
            _configuration = options.Value ?? throw new ArgumentNullException(nameof(options));

            _client = InitializeClient(
                    client ?? throw new ArgumentNullException(nameof(client)),
                    interactionHandler ?? throw new ArgumentNullException(nameof(interactionHandler)),
                    interactionService ?? throw new ArgumentNullException(nameof(interactionService)))
                .GetAwaiter()
                .GetResult();
        }
        
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Running...");
            
            await _client.LoginAsync(
                TokenType.Bot,
                _configuration.Token);

            await _client.StartAsync();

            cancellationToken.WaitHandle.WaitOne();
            
            if (_client.ConnectionState != ConnectionState.Disconnected)
            {
                await _client.LogoutAsync();
                await _client.StopAsync();
            }
        }

        private async Task<DiscordSocketClient> InitializeClient(
            DiscordSocketClient client,
            InteractionHandler interactionHandler,
            InteractionService interactionService)
        {
            client.Log += message =>
            {
                message.Log(_logger);
                return Task.CompletedTask;
            };
            
            client.Ready += async () =>
            {
                await interactionService.RegisterCommandsGloballyAsync();
                
                _logger.LogInformation("Registered commands");
                
                _statusSender.Start();
            };

            client.Disconnected += exception =>
            {
                _statusSender.Stop();
                
                if (exception is GatewayReconnectException)
                {
                    _logger.LogInformation(
                        exception, 
                        $"Reconnecting: {exception.Message}");

                    return Task.CompletedTask;
                }
                
                _logger.LogError(
                    exception, 
                    exception.Message);
                
                Log.CloseAndFlush();
                
                Environment.Exit(-1);
                
                return Task.CompletedTask;
            }; 
            
            await interactionHandler.InitializeAsync();
            
            return client;
        }
    }
}