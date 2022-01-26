using DSharpPlus.SlashCommands;
using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DevBot.Services;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;

namespace DevBot.Modules {

    public class RequireId : SlashCheckBaseAttribute {
        public ulong UserId;
        

        public RequireId(ulong userId) {
            this.UserId = userId;
        }

        public override Task<bool> ExecuteChecksAsync(InteractionContext ctx) {
            if (ctx.User.Id != UserId)
                return Task.FromResult(false);
            else
                return Task.FromResult(true);
        }
    }

    [SlashCommandGroup("dev", "developer commands")]
    class Dev : ApplicationCommandModule {
        
        [SlashCommandGroup("Dashboard", "Manage the web dashboard from the bot!")]
        class DashboardCmds : ApplicationCommandModule {

            public DashboardService _dashboardService {get; set;}
            public IConfigurationRoot _settings {get; set;}
                    
            [SlashCommand("restart", "Restart the dashboard")]
            [RequireOwner]
            public async Task RestartCmd(InteractionContext ctx) {

                
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder() {
                    IsEphemeral=true
                }.WithContent("Restarting..."));
                await _dashboardService.Restart();

            }
            
            [SlashCommand("start", "Start the dashboard")]
            [RequireOwner]
            public async Task StartCmd(InteractionContext ctx) {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder() {
                    IsEphemeral=true
                }.WithContent("Starting..."));
                
                await _dashboardService.Start();

            }

            [SlashCommand("stop", "Stop the dashboard")]
            [RequireOwner]
            public async Task StopCmd(InteractionContext ctx) {

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder() {
                    IsEphemeral=true
                }.WithContent("Stopping..."));
                _dashboardService.Stop();

            }

        }

    }
}