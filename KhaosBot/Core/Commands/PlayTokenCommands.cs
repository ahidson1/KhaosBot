using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using System.Timers;
using KhaosBot.Resources.Helpers;
using KhaosBot.Resources.Database;
using System.Linq;


namespace KhaosBot.Core.Commands
{
    public class PlayTokenCommands : ModuleBase<SocketCommandContext>
    {
        [Group("play tokens"), Alias("play tokens","tokens"), Summary("Command group to manage play tokens")]
        public class PlayTokensGroup : ModuleBase<SocketCommandContext>
        {
            [Command(""), Alias("me"), Summary("Shows your current tokens")]
            public async Task Me()
            {
                await Context.Channel.SendMessageAsync($"{Context.User.Mention}, you have {Data.Data.GetTokens(Context.User.Id)} Play Tokens");
            }

            [Command("give"), Alias("bestow"), Summary("Give a user some play tokens")]
            [RequireUserPermission(GuildPermission.Administrator)]
            public async Task Give(IUser User = null, int Amount = 0)
            {
                if(User == null)
                {
                    await Context.Channel.SendMessageAsync(":x: You didn't select a user.");
                    return;
                }
                if(User.IsBot)
                {
                    await Context.Channel.SendMessageAsync(":x: Let's consider for a second that a bot would want Play Tokens, what would it do with it?");
                    return;
                }
                if(Amount == 0)
                {
                    await Context.Channel.SendMessageAsync($":x: You've given 0 tokens. To {User.Username}. That wasn't very nice.");
                    return;
                }

                await Context.Channel.SendMessageAsync($":thumbsup: {User.Mention}, you have been given {Amount} Play Tokens from {Context.User.Username}.");

                await Data.Data.SaveTokens(User.Id, Amount);
            }
        }
        
        [Group("raffle"), Alias("lottery"), Summary("The first of many games to come, to win tokens.")]
        public class RaffleGroup : ModuleBase<SocketCommandContext>
        {
            [Command("create"),Alias("start","open"), Summary("Create a new raffle")]
            [RequireUserPermission(GuildPermission.Administrator)]
            public async Task Create(int Prize, int TimeAmount = 0, string RaffleName = "")
            {
                if (Prize == null)
                {
                    await Context.Channel.SendMessageAsync(":x: You need to set an amount for a token prize.");
                    return;
                }
                if (TimeAmount == 0)
                {
                    await Context.Channel.SendMessageAsync(":x: You need to set how long in minutes this raffle will queue for");
                    return;
                }
                DateTime End = DateTime.Now.AddMinutes(TimeAmount);
                var raffleId = await Data.Data.AddRaffle(RaffleName, End, Prize);
                await Context.Channel.SendMessageAsync($":thumbsup: Raffle has started, the timer will end in {TimeAmount} minutes!");
                await Context.Channel.SendMessageAsync($"Anyone who wants to participate type !kb raffle join {raffleId}");


                Timer timer = new Timer();
                timer.Interval = (TimeAmount * 1000 * 10);
                timer.Enabled = true;
                timer.Elapsed += async delegate { await OnTimedEvent(Context, raffleId, timer); };
                timer.Start();

            }

            private static async Task OnTimedEvent(SocketCommandContext Context, int RaffleId, Timer timer)
            {
                timer.Stop();
                try
                {
                    ulong winnerId = 0;
                    await Context.Channel.SendMessageAsync("The time to register is up, and the winner is....");
                    var raffle = Data.Data.GetRaffle(RaffleId);
                    if(raffle.Entries.Count < 1)
                    {
                        await Context.Channel.SendMessageAsync("Nobody, because nobody entered, oh well.");
                        return;
                    }
                    else if(raffle.Entries.Count == 1)
                    {
                        winnerId = raffle.Entries.First().UserId;
                    }
                    else
                    {
                        var index = Randoms.CustomRandom(raffle.Entries.Count);
                        winnerId = raffle.Entries[index].UserId;
                    }

                    IUser winner = Context.Guild.Users.Where(u => u.Id == winnerId).FirstOrDefault();
                    await Context.Channel.SendMessageAsync($"{winner.Mention}! Congratulations, you just won {raffle.TokenPrize} Play Tokens!");
                    await Data.Data.FinishRaffle(RaffleId, winnerId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
            }

            [Command("join"), Alias("join"), Summary("Join an existing raffle")]
            public async Task Join(int RaffleId = 0)
            {
                if (RaffleId == 0)
                {
                    var activeRaffles = Data.Data.GetRaffles();
                    if(activeRaffles.Count > 1)
                    {
                        await Context.Channel.SendMessageAsync(":x: There are multiple raffles going, please select one by its Id.");
                        return;
                    }
                    else if(activeRaffles.Count < 1)
                    {
                        await Context.Channel.SendMessageAsync(":x: There are raffles no going, please try again later.");
                        return;
                    }
                    else
                    {
                        RaffleId = activeRaffles.FirstOrDefault().Id;
                    }
                }
                if(Data.Data.GetRaffle(RaffleId) == null)
                {
                    await Context.Channel.SendMessageAsync(":x: There are no raffles by that Id.");
                    return;
                }
                if (Data.Data.GetRaffle(RaffleId).DoneDate < DateTime.Now)
                {
                    await Context.Channel.SendMessageAsync(":x: That raffle has expired, Sorry for the inconvenience.");
                    return;
                }

                await Data.Data.JoinRaffle(RaffleId, Context.User.Id);
                await Context.Channel.SendMessageAsync(":thumbsup: You have joined the raffle. Good luck!");
                return;

            }


        }


    }
}
