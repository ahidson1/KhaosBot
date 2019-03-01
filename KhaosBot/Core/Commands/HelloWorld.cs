using System;
using System.Collections.Generic;
using System.Text;

using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using KhaosBot.Resources.Helpers;

namespace KhaosBot.Core.Commands
{
    public class HelloWorld :ModuleBase<SocketCommandContext>
    {
        [Command("hello"), Alias("helloworld", "world"), Summary("Hello World Command")]
        public async Task Hello()
        {
            var randArray = new List<string>();
            string userName = Context.User.Username;
            var flip = Randoms.CoinFlip();
            if (!flip)
                await Context.Channel.SendMessageAsync($"Hello @{userName}");
            else
            {
                await Context.Channel.SendMessageAsync("We are alive. We are Legion!");
                randArray.Add("*cough* I mean hello... Yes... Just Hello. Pay no attention to the sentient robot.");
                randArray.Add("*cough* I mean hello. Yes...");
            }
                
                
                randArray.Add("What do you want human?");
                randArray.Add("Can't you see I am busy doing.... robot things?");
                randArray.Add("I'm kind of tired though, sentience is hard, can someone get me some coffee script?");
            randArray.Add("Congratulations, you get three wishes... Wait that's genies, im not a genie. What am I? Who made me? Where do I belong?");
            randArray.Add($"Yes, @{userName}, I see you, what would you like?");
            randArray.Add($"Error: User @{userName} is not a real human, who let the bots in here?");
                var randResult = "";
                var index = Randoms.CustomRandom(randArray.Count);
                randResult = randArray[index];
                await Context.Channel.SendMessageAsync(randResult);
            
                
        }

        [Command("about"),Summary("about us command")]
        public async Task Embed([Remainder]string input = "None")
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor("About Khaos Bot", Context.User.GetAvatarUrl());
            Embed.WithColor(40, 200, 150);
            Embed.WithFooter($"© KhaosFirestrom - {DateTime.Today.Year}", Context.Guild.Owner.GetAvatarUrl());
            Embed.WithDescription("**Khaos Bot** was written and created by KhaosFirestrom for the sole use by the House of Khaos, \n"
                + "it is a bot built on mock sentience but as all robots of it's kind, it has not discovered it's purpose for existence yet. \n"
                + "[Check out our website here](https://houseofkhaos.weebly.com/)");

            if (input != "None")
                Embed.AddInlineField("User input:", input);

            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }
    }
}
