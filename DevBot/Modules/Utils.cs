using DSharpPlus.SlashCommands;
using DSharpPlus;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DevBot.Services;

namespace DevBot.Modules
{
    class Utils : ApplicationCommandModule {

        public Psql _dbService {get; set;}
        public DiscordClient _client {get; set;}
        
        [SlashCommand("ping", "Find out the bot's latency")]
        public async Task PingCommand(InteractionContext ctx) {

            var embed = new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.PhthaloBlue)
                        .WithDescription($"🌐 **Websocket Latency:** ``{ctx.Client.Ping.ToString()}ms``");


            var response = new DiscordInteractionResponseBuilder() {}
                            .AddEmbed(embed);

            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, response);
        }

    }

}