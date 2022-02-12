using DSharpPlus.SlashCommands;
using DSharpPlus;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using DevBot.Services.API;
using DevBot.Services;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.EventHandling;

namespace DevBot.Modules {

    class StackCmds : ApplicationCommandModule {
        
        public StackExchangeService _stackService {get; set;}
        public DiscordClient _client {get; set;}
        public LocaleService _locale {get; set;}

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
                            .WithAuthor($"{question.owner.displayName} {_locale.TranslatableText("devbot.modules.stackexchange.asks", ctx.User.Id)}", question.link, question.owner.profileImage)
                            .WithColor(0xF2740D)
                            .WithFooter($"{_locale.TranslatableText("devbot.modules.stackexchange.score", ctx.User.Id)} {question.score}  ⋅  {_locale.TranslatableText("devbot.modules.stackexchange.views", ctx.User.Id)} {question.viewCount}  ⋅  {_locale.TranslatableText("devbot.modules.stackexchange.activity", ctx.User.Id)} {question.lastActivityDate.ToShortDateString()}  ⋅  {_locale.TranslatableText("devbot.modules.stackexchange.result", ctx.User.Id)} {result}/{results}");
                
                pages.Add(new Page(embed: embed));
                result ++;
                
            }

            var interactivity = _client.GetInteractivity();

            if (pages.Count == 0) {
                var embed = new DiscordEmbedBuilder()
                            .WithTitle(_locale.TranslatableText("devbot.modules.stackexchange.notfound.overflow", ctx.User.Id))
                            .WithDescription($"```{_locale.TranslatableText("devbot.modules.stackexchange.notfound.check", ctx.User.Id)}```")
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
                            .WithAuthor($"{question.owner.displayName} {_locale.TranslatableText("devbot.modules.stackexchange.asks", ctx.User.Id)}", question.link, question.owner.profileImage)
                            .WithColor(0xDC461D)
                            .WithFooter($"{_locale.TranslatableText("devbot.modules.stackexchange.score", ctx.User.Id)} {question.score}  ⋅  {_locale.TranslatableText("devbot.modules.stackexchange.views", ctx.User.Id)} {question.viewCount}  ⋅  {_locale.TranslatableText("devbot.modules.stackexchange.activity", ctx.User.Id)} {question.lastActivityDate.ToShortDateString()}  ⋅  {_locale.TranslatableText("devbot.modules.stackexchange.result", ctx.User.Id)} {result}/{results}");
                
                pages.Add(new Page(embed: embed));
                result ++;
            }

            var interactivity = _client.GetInteractivity();

            if (pages.Count == 0) {
                var embed = new DiscordEmbedBuilder()
                            .WithTitle(_locale.TranslatableText("devbot.modules.stackexchange.notfound.ubuntu", ctx.User.Id))
                            .WithDescription($"```{_locale.TranslatableText("devbot.modules.stackexchange.notfound.check", ctx.User.Id)}```")
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