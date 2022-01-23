using DSharpPlus.SlashCommands;
using DSharpPlus;
using System.Threading.Tasks;
using System.Diagnostics;
using DSharpPlus.Entities;
using DevBot.Services;

namespace DevBot.Modules
{
    class Utils : ApplicationCommandModule {

        public DbService _dbService {get; set;}
        public DiscordClient _client {get; set;}
        public LocaleService _locale {get; set;}
        
        [SlashCommand("ping", "Find out the bot's latency")]
        public async Task PingCommand(InteractionContext ctx) {

            Stopwatch timer = new Stopwatch();
            timer.Start();
            await ctx.Channel.TriggerTypingAsync();
            var msg = await ctx.Channel.SendMessageAsync(".");
            timer.Stop();
            await msg.DeleteAsync();


            string wsText = _locale.TranslatableText("devbot.modules.utils.ping.wslatency", ctx.Guild.Id);
            string wsPing = ctx.Client.Ping.ToString();

            string rtText = _locale.TranslatableText("devbot.modules.utils.ping.msglatency", ctx.Guild.Id);
            string rtPing = timer.ElapsedMilliseconds.ToString();

            var embed = new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.PhthaloBlue)
                        .WithDescription($"🌐 **{wsText}:** ``{wsPing}ms``\n💬 **{rtText}:** ``{rtPing}ms``");

            var response = new DiscordInteractionResponseBuilder() {}
                            .AddEmbed(embed);

            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, response);

        }

    }

}