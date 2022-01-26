using DSharpPlus.SlashCommands;
using DSharpPlus;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using DevBot.Services.API;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.EventHandling;

namespace DevBot.Modules {

    class StackCmds : ApplicationCommandModule {
        
        public StackExchangeService _stackService {get; set;}
        public DiscordClient _client {get; set;}

        [ContextMenu(ApplicationCommandType.MessageContextMenu, "Search StackOverflow")]
        public async Task StackSearchmsg(ContextMenuContext ctx) {
            
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            List<StackExchangeQuestion> questions = await _stackService.SearchAnswers(ctx.TargetMessage.Content);

            
            List<Page> pages = new List<Page>();
            int result = 1;
            int results = questions.Count;
            
            foreach (var question in questions) {

                var embed = new DiscordEmbedBuilder()
                            .WithTitle($"{question.title}?")
                            .WithAuthor($"{question.owner.displayName} Asks...", question.link, question.owner.profileImage)
                            .WithColor(0xF2740D)
                            .WithFooter($"Score: {question.score}  ⋅  Views: {question.viewCount}  ⋅  Last Activity: {question.lastActivityDate.ToShortDateString()}  ⋅  Result {result}/{results}");
                
                pages.Add(new Page(embed: embed));
                result ++;
            }

            var interactivity = _client.GetInteractivity();

            if (pages.Count == 0) {
                var embed = new DiscordEmbedBuilder()
                            .WithTitle("⋅ I couldn't find anything on StackOverflow!")
                            .WithDescription("```Please check your spelling and word choice```")
                            .WithColor(DiscordColor.Red);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
                return;
            }

            await ctx.Interaction.SendPaginatedResponseAsync(false, ctx.Member, pages, asEditResponse: true);

            //await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                                                            //.AddEmbed(embed))
            // await interactivity.SendPaginatedResponseAsync(ctx.Interaction, false, ctx.Member, pages);
        }

        [ContextMenu(ApplicationCommandType.MessageContextMenu, "Search Askubuntu")]
        public async Task UbuntuSearchmsg(ContextMenuContext ctx) {
            
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            List<StackExchangeQuestion> questions = await _stackService.SearchUbuntu(ctx.TargetMessage.Content);

            
            List<Page> pages = new List<Page>();
            int result = 1;
            int results = questions.Count;
            
            foreach (var question in questions) {

                var embed = new DiscordEmbedBuilder()
                            .WithTitle($"{question.title}?")
                            .WithAuthor($"{question.owner.displayName} Asks...", question.link, question.owner.profileImage)
                            .WithColor(0xDC461D)
                            .WithFooter($"Score: {question.score}  ⋅  Views: {question.viewCount}  ⋅  Last Activity: {question.lastActivityDate.ToShortDateString()}  ⋅  Result {result}/{results}");
                
                pages.Add(new Page(embed: embed));
                result ++;
            }

            var interactivity = _client.GetInteractivity();

            if (pages.Count == 0) {
                var embed = new DiscordEmbedBuilder()
                            .WithTitle("⋅ I couldn't find anything on AskUbuntu!")
                            .WithDescription("```Please check your spelling and word choice```")
                            .WithColor(DiscordColor.Red);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
                return;
            }

            await ctx.Interaction.SendPaginatedResponseAsync(false, ctx.Member, pages, asEditResponse: true);

            //await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                                                            //.AddEmbed(embed))
            // await interactivity.SendPaginatedResponseAsync(ctx.Interaction, false, ctx.Member, pages);
        }

    }


}