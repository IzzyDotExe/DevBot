using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity.Enums;
using Serilog;


namespace DevBot.Services {


    class InteractionHandler {

        private readonly DiscordClient _client;
        private readonly IServiceProvider _provider;

        public InteractionHandler(DiscordClient client, IServiceProvider provider) {
            

            Log.Logger.Information("Interaction Handler loaded!");

            _client = client;
            _provider = provider;
            _client.UseInteractivity(new InteractivityConfiguration() {AckPaginationButtons = true});
            // _client.ContextMenuInteractionCreated += ContextMenuInteractionCreatedCallback;


        }

    }


}