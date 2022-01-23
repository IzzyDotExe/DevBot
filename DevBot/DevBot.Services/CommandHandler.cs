using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using Serilog;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using DevBot.Modules;
using DevBot.Services;

namespace DevBot.Services {
    public class CommandHandler {
        private readonly DiscordClient _client;
        private readonly IServiceProvider _provider;
        private readonly LocaleService _localeService;

        public CommandHandler(DiscordClient client, IServiceProvider provider) {
            
            _client = client;
            _provider = provider;
            _localeService = _provider.GetRequiredService<LocaleService>();

            var slashCommandsConfig = new SlashCommandsConfiguration() {
                Services = provider
            };

            var slashcommands = _client.UseSlashCommands(slashCommandsConfig);

            slashcommands.RegisterCommands<Utils>(707260999496892436);
            slashcommands.RegisterCommands<Dev>(707260999496892436);

            Log.Logger.Information("Commands loaded!");

            slashcommands.SlashCommandErrored += SlashCommandError;

        }

        public async Task SlashCommandError(SlashCommandsExtension slashcommands, SlashCommandErrorEventArgs eventArgs) {
            if (eventArgs.Exception is SlashExecutionChecksFailedException slex) {
                foreach (var check in slex.FailedChecks)
                    if (check is RequireId att)
                        await eventArgs.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(){
                            IsEphemeral = true
                            }.WithContent($"You do not have permission to use this command"));
            } else {
                Log.Logger.Error(eventArgs.Exception.ToString());
            }
        }



    }
}