using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using DevBot.Modules;
using DevBot.Services;
using DevBot.Services.API;



namespace DevBot {
    class Program {
        
        static void Main(string[] args) {

            var settingsBuilder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("settings.json");

            var settings = settingsBuilder.Build();

            DevBotClient(settings).GetAwaiter().GetResult();
        }
        
        static async Task DevBotClient(IConfigurationRoot settings) {

            var services = ConfigureServices(settings);

            services.GetRequiredService<CommandHandler>();
            var dbservice = services.GetRequiredService<DbService>();
            var discordClient = services.GetRequiredService<DiscordClient>();
            var webdashboard = services.GetRequiredService<DashboardService>();
            var localeService = services.GetRequiredService<LocaleService>();
            var stackService = services.GetRequiredService<StackExchangeService>();
            var githubService = services.GetRequiredService<GithubService>();
            services.GetRequiredService<UptimeService>();
            services.GetRequiredService<InteractionHandler>();

            await dbservice.CreateConnection();
            await discordClient.ConnectAsync();
            localeService.BuildLocales();
            await dbservice.BuildCaches();
            
            discordClient.Ready += ( async (c, e) => {
                Log.Logger.Information($"Logged in as {discordClient.CurrentUser}");
                Log.Logger.Information("Ready!");
            });
            // await webdashboard.Start();

            await Task.Delay(-1);

        }

        static IServiceProvider ConfigureServices(IConfigurationRoot settings) {

            var logfactory = setupLogger();

            var discordConfig = new DiscordConfiguration() {
                
                Token = settings.GetSection("discord:token").Value,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
                MinimumLogLevel = LogLevel.Debug,
                LoggerFactory = logfactory
            };
        
            var discordClient = new DiscordClient(discordConfig);


            var services = new ServiceCollection()

                        .AddSingleton<IConfigurationRoot>(settings)
                        .AddSingleton<DbService>()
                        .AddSingleton<DiscordClient>(discordClient)
                        .AddSingleton<CommandHandler>()
                        .AddSingleton<LocaleService>()
                        .AddSingleton<DashboardService>()
                        .AddSingleton<UptimeService>()
                        .AddSingleton<StackExchangeService>()
                        .AddSingleton<InteractionHandler>()
                        .AddSingleton<GithubService>()
                        .BuildServiceProvider();

            return services; 
        }

        static ILoggerFactory setupLogger() {
            Log.Logger = new LoggerConfiguration()
                        .WriteTo.Console()
                        .CreateLogger();
            
            var logFactory = new LoggerFactory().AddSerilog();
            
            return logFactory;
            
        }

    }

}