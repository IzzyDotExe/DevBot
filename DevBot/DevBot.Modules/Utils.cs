using DSharpPlus.SlashCommands;
using DSharpPlus;
using DSharpPlus.Exceptions;
using System.Collections.Generic;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using System.Threading.Tasks;
using System.Diagnostics;
using DSharpPlus.Entities;
using DevBot.Services;
using Serilog;
using Newtonsoft.Json.Linq;
using System.Net;


namespace DevBot.Modules
{
    class Utils : ApplicationCommandModule {

        public DbService _dbService {get; set;}
        public DiscordClient _client {get; set;}
        public LocaleService _locale {get; set;}

        public UptimeService _uptimeService {get; set;}
        
        [SlashCommand("ping", "Find out the bot's latency")]
        public async Task PingCommand(InteractionContext ctx) {

            Stopwatch timer = new Stopwatch();
            timer.Start();
            await ctx.Channel.TriggerTypingAsync();
            var msg = await ctx.Channel.SendMessageAsync(".");
            timer.Stop();
            await msg.DeleteAsync();


            string wsText = _locale.TranslatableText("devbot.modules.utils.ping.wslatency", ctx.User.Id);
            string wsPing = ctx.Client.Ping.ToString();

            string rtText = _locale.TranslatableText("devbot.modules.utils.ping.msglatency", ctx.User.Id);
            string rtPing = timer.ElapsedMilliseconds.ToString();

            var embed = new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.PhthaloBlue)
                        .WithDescription($"🌐 **{wsText}:** ``{wsPing}ms``\n💬 **{rtText}:** ``{rtPing}ms``");

            var response = new DiscordInteractionResponseBuilder() {}
                            .AddEmbed(embed);

            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, response);

        }

        [SlashCommand("locale", "Set the your locale")]
        public async Task LocaleCommand(InteractionContext ctx, [Option("locale", "Choose a language to swtich to")]Language lang) {
            
            string locale;

            switch (lang) {
                case Language.english:
                    locale = "english";
                    break;
                case Language.french:
                    locale = "french";
                    break;
                case Language.spanish:
                    locale = "spanish";
                    break;
                default:
                    locale = "english";
                    break;
            }

            await _dbService.SetLocale(ctx.User.Id, locale);

            string sucessmsg = _locale.TranslatableText("devbot.modules.utils.setlocale.success", ctx.User.Id);

            var embed = new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.PhthaloBlue)
                        .WithDescription($"{sucessmsg}");

            var response = new DiscordInteractionResponseBuilder() {}
                            .AddEmbed(embed);

            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, response);

        }

        [SlashCommand("Uptime", "Get the bot's uptime.")]
        public async Task UptimeCommand(InteractionContext ctx) {

            string uptimeMsg = _locale.TranslatableText("devbot.modules.utils.uptime", ctx.User.Id);
            string uptimeDays = _locale.TranslatableText("devbot.modules.utils.uptime.days", ctx.User.Id);
            string uptimeHours = _locale.TranslatableText("devbot.modules.utils.uptime.hours", ctx.User.Id);
            string uptimeMinutes = _locale.TranslatableText("devbot.modules.utils.uptime.minutes", ctx.User.Id);
            string uptimeSeconds = _locale.TranslatableText("devbot.modules.utils.uptime.seconds", ctx.User.Id);

            var uptime = _uptimeService._uptime.Elapsed;

            var embed = new DiscordEmbedBuilder()
                .WithAuthor(_client.CurrentUser.Username, null, _client.CurrentUser.AvatarUrl)
                .WithColor(DiscordColor.PhthaloBlue)
                .WithDescription($"**{uptimeMsg}:** ``{uptime.Days}{uptimeDays} {uptime.Hours}{uptimeHours} {uptime.Minutes}{uptimeMinutes} {uptime.Seconds}{uptimeSeconds}``");

            var response = new DiscordInteractionResponseBuilder() {}
                            .AddEmbed(embed);

            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, response);
        }

        [SlashCommand("Avatar", "Fetch your own, or another user's avatar.")]
        public async Task AvatarCommand(InteractionContext ctx, [Option("user", "Which user to get the avatar of")]DiscordUser user = null) {
            
            if (user is null) {
                user = ctx.User;
            }

            var selectOptions = new List<DiscordSelectComponentOption>() {
                new DiscordSelectComponentOption("Global Avatar", "global", "Get the user's global avatar", false, new DiscordComponentEmoji("🌐")),
                new DiscordSelectComponentOption("Server Avatar", "server", "Get the user's server avatar", false, new DiscordComponentEmoji("🖥️"))
            };
            
            var selectmenu = new DiscordSelectComponent("custom_id_for_select_menu", "placeholder", selectOptions, false, 1, 1);

            var res = new DiscordInteractionResponseBuilder()
            .WithContent("Hello World")
            .AddComponents(selectmenu);

            try {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, res);
            } catch (BadRequestException ex) {
                Log.Logger.Error(ex.ToString());

                Log.Logger.Information(ex.Errors);
            }
            
        }
        
        [ContextMenu(ApplicationCommandType.MessageContextMenu, "Format JSON")]
        public async Task FormatJson(ContextMenuContext ctx) {
            
            string jsonString;

            if (!(ctx.TargetMessage.Attachments.Count == 0)) {
                foreach (DiscordAttachment att in ctx.TargetMessage.Attachments) {
                    using (WebClient client = new WebClient()) {
                        jsonString = await client.DownloadStringTaskAsync(new System.Uri(att.Url));
                    }
                }
            }

            if (ctx.TargetMessage.Content.ToLower().Contains("```json")) {

                jsonString = ctx.TargetMessage.Content.ToLower().Split("```json")[1];
                jsonString = jsonString.Trim('`');


            } else {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                                                            .WithContent("No Json Detected"));
                return;
            }

            
            var json = JToken.Parse(jsonString);
            jsonString = json.ToString(Newtonsoft.Json.Formatting.Indented);
            
            jsonString = Formatter.BlockCode(jsonString, "json");


            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                                                        .WithContent(jsonString));


        }

    }

}