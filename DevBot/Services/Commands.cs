using DSharpPlus;
using Microsoft.Extensions.DependencyInjection;
using System;
using DSharpPlus.SlashCommands;
using DevBot.Modules;

namespace DevBot.Services {
    public class CommandHandler {
        private readonly DiscordClient _client;
        private readonly IServiceProvider _provider;

        public CommandHandler(DiscordClient client, IServiceProvider provider) {
            
            _client = client;
            _provider = provider;

            var slashCommandsConfig = new SlashCommandsConfiguration() {
                Services = provider
            };


            var slashcommands = _client.UseSlashCommands(slashCommandsConfig);

            slashcommands.RegisterCommands<Utils>(707260999496892436);

        }

    }
}