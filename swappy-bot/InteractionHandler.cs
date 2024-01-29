namespace SwappyBot
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Discord;
    using Discord.Interactions;
    using Discord.WebSocket;
    using SwappyBot.Infrastructure;
    using Microsoft.Extensions.Logging;

    public class InteractionHandler
    {
        private readonly ILogger<InteractionHandler> _logger;
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _commands;
        private readonly IServiceProvider _services;

        public InteractionHandler(
            ILogger<InteractionHandler> logger,
            DiscordSocketClient client, 
            InteractionService commands, 
            IServiceProvider services)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _commands = commands ?? throw new ArgumentNullException(nameof(commands));
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public async Task InitializeAsync()
        {
            _commands.Log += message =>
            {
                message.Log(_logger);
                return Task.CompletedTask;
            };
            
            await _commands.AddModulesAsync(
                Assembly.GetEntryAssembly(),
                _services);

            _client.InteractionCreated += HandleInteraction;

            _commands.SlashCommandExecuted += (_, _, _) => Task.CompletedTask;
            _commands.ContextCommandExecuted += (_, _, _) => Task.CompletedTask;
            _commands.ComponentCommandExecuted += (_, _, _) => Task.CompletedTask;
        }

        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                await _commands.ExecuteCommandAsync(
                    new SocketInteractionContext(_client, arg),
                    _services);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, 
                    ex.Message);

                if (arg.Type == InteractionType.ApplicationCommand)
                {
                    await arg
                        .GetOriginalResponseAsync()
                        .ContinueWith(async msg => await msg.Result.DeleteAsync());
                }
            }
        }
    }
}