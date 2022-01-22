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

            var discordConfig = new DiscordConfiguration() {
                
                Token = settings.GetSection("Bot:Token").Value,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
                MinimumLogLevel = LogLevel.Debug
            };
        
            var discordClient = new DiscordClient(discordConfig);


            var services = new ServiceCollection()
                    .AddSingleton<Psql>()
                    .AddSingleton<IConfigurationRoot>(settings)
                        .AddSingleton<DiscordClient>(discordClient)
                        .AddSingleton<CommandHandler>()
                        .BuildServiceProvider();

            services.GetRequiredService<CommandHandler>();

            await discordClient.ConnectAsync();

            await Task.Delay(-1);

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