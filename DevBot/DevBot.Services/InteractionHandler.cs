using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
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
            _client.ComponentInteractionCreated += ComponentCreatedEvent;


        }

        public async Task ComponentCreatedEvent(DiscordClient client, ComponentInteractionCreateEventArgs eventArgs) {
            Log.Logger.Information(eventArgs.Id);
            string response = "";
            switch (eventArgs.Id) {

                case "avatar_component":

                    if (eventArgs.Interaction.Data.Values[0] == "server") {
                        response = eventArgs.Guild.Members[eventArgs.User.Id].GetGuildAvatarUrl(ImageFormat.Auto);
                    }

                    if (eventArgs.Interaction.Data.Values[0] == "global") {
                        response = eventArgs.User.GetAvatarUrl(ImageFormat.Auto);
                    }   

                    await eventArgs.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder()
                                                                                                          .WithContent(response)
                                                                                                          .AddComponents(eventArgs.Message.Components));
                    break;


            }


        }

    }


}