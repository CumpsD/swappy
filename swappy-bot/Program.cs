namespace SwappyBot
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Discord;
    using Discord.Interactions;
    using Discord.WebSocket;
    using SwappyBot.Configuration;
    using SwappyBot.EntityFramework;
    using SwappyBot.Infrastructure.Options;
    using SwappyBot.Modules;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Serilog;

    public class Program
    {
        private static readonly CancellationTokenSource CancellationTokenSource = new();

        public static void Main()
        {
            var ct = CancellationTokenSource.Token;

            AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
                Log.Fatal(
                    (Exception)eventArgs.ExceptionObject,
                    "Encountered a fatal exception, exiting program.");
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();
            
            var container = ConfigureServices(configuration);
            var logger = container.GetRequiredService<ILogger<Program>>();
            var applicationName = Assembly.GetEntryAssembly()?.GetName().Name;
            
            // Bit of a hack to get the EF Logger injected
            container.GetRequiredService<EntityFrameworkLogger>();

            logger.LogInformation(
                "Starting {ApplicationName}",
                applicationName);
            
            Console.CancelKeyPress += (_, eventArgs) =>
            { 
                logger.LogInformation("Requesting disconnect...");
                CancellationTokenSource.Cancel();
                eventArgs.Cancel = true;
            };

            try
            {
                #if DEBUG
                Console.WriteLine($"Press ENTER to start {applicationName}...");
                Console.ReadLine();
                #endif
                
                // Run the bot
                var bot = container.GetRequiredService<Bot>();
                var botTask = bot.RunAsync(ct);
                
                Console.WriteLine("Running... Press CTRL + C to exit.");
                botTask.GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "Encountered a fatal exception, exiting program.");

                throw;
            }
            
            logger.LogInformation("Stopping...");
            
            // Allow some time for flushing before shutdown.
            Log.CloseAndFlush();
            Thread.Sleep(1000);
        }

        private static AutofacServiceProvider ConfigureServices(
            IConfiguration configuration)
        {
            var services = new ServiceCollection();

            var builder = new ContainerBuilder();

            builder
                .RegisterModule(new LoggingModule(configuration, services));

            var tempProvider = services.BuildServiceProvider();
            var loggerFactory = tempProvider.GetRequiredService<ILoggerFactory>();
            var connectionString = configuration.GetConnectionString("Bot");

            var botConfiguration = configuration
                .GetSection(BotConfiguration.Section)
                .Get<BotConfiguration>()!;
            
            services
                .ConfigureAndValidate<BotConfiguration>(configuration.GetSection(BotConfiguration.Section))
                
                .AddHttpClient(
                    "Quote",
                    x =>
                    {
                        x.BaseAddress = new Uri(botConfiguration.QuoteUrl);
                        x.DefaultRequestHeaders.UserAgent.ParseAdd("discord-swappy");
                    })
                
                .Services
                    
                .AddHttpClient(
                    "Deposit",
                    x =>
                    {
                        x.BaseAddress = new Uri(botConfiguration.DepositUrl);
                        x.DefaultRequestHeaders.UserAgent.ParseAdd("discord-swappy");
                    })
                
                .Services
                
                .AddDbContextPool<BotContext>((provider, options) => options
                    .UseLoggerFactory(provider.GetRequiredService<ILoggerFactory>())
                    .UseMySql(connectionString, Db.Version)
                    .UseLoggerFactory(loggerFactory));

            builder
                .Register(x => new EntityFrameworkLogger(x.Resolve<ILoggerFactory>()))
                .SingleInstance();
            
            builder
                .Register(_ => new DiscordSocketClient(new DiscordSocketConfig
                {
                    GatewayIntents = 
                        GatewayIntents.Guilds |
                        GatewayIntents.GuildMembers |
                        GatewayIntents.GuildMessages |
                        GatewayIntents.GuildMessageReactions |
                        GatewayIntents.GuildMessageTyping |
                        GatewayIntents.MessageContent,
                    LogGatewayIntentWarnings = true,
                    AlwaysDownloadUsers = true,
                    LogLevel = LogSeverity.Debug
                }))
                .SingleInstance();

            builder
                .Register(x => new InteractionService(
                    x.Resolve<DiscordSocketClient>(),
                    new InteractionServiceConfig
                    {
                        LogLevel = LogSeverity.Debug,
                        DefaultRunMode = Discord.Interactions.RunMode.Async
                    }))
                .SingleInstance();
            
            builder
                .RegisterType<InteractionHandler>()
                .SingleInstance();
            
            builder
                .RegisterType<Bot>()
                .SingleInstance();
            
            builder
                .Populate(services);

            return new AutofacServiceProvider(builder.Build());
        }
    }
}
