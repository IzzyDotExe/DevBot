using DSharpPlus;
using DSharpPlus.CommandsNext;
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


            await dbservice.CreateConnection();
            await discordClient.ConnectAsync();
            localeService.BuildLocales();
            await dbservice.BuildCaches();
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