using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using KhaosBot.Resources.Database;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace KhaosBot.Core.Data
{
    public class Data
    {
        public static int GetTokens(ulong UserId)
        {
            using (var DbContext = new SQLiteDbContext())
            {
                var token = DbContext.PlayTokens.Find(UserId);
                if (token == null)
                    return 0;
                return token.Amount;
            }
        }
        public static async Task SaveTokens(ulong UserId, int Amount)
        {
            using (var DbContext = new SQLiteDbContext())
            {
                var token = DbContext.PlayTokens.Find(UserId);
                if(token == null)
                {
                    DbContext.PlayTokens.Add(new PlayToken
                    {
                        UserId = UserId,
                        Amount = Amount
                    });
                }
                else
                {
                    token.Amount += Amount;
                    DbContext.PlayTokens.Update(token);
                }
                await DbContext.SaveChangesAsync();
            }
        }

        public static List<Raffle> GetRaffles(DateTime? EndDate = null)
        {
            // we only want the raffles that have not yet expired by default but we may want historical raffles later
            if (EndDate == null)
                EndDate = DateTime.Now;

            using (var DbContext = new SQLiteDbContext())
            {
                var raffles = DbContext.Raffles.Where(r => r.DoneDate >= EndDate);
               
                return raffles.ToList();
            }
        }

        public static Raffle GetRaffle(int RaffleId)
        {
            using (var DbContext = new SQLiteDbContext())
            {
                var raffle = DbContext.Raffles.Where(r=> r.Id == RaffleId).FirstOrDefault();
                // HACK: Lazy Loading and Include does not seem to be working, load the entries manually, research later.
                var entries = DbContext.RaffleEntries.Where(e => e.RaffleId == raffle.Id).ToList();
                raffle.Entries = entries;
                return raffle;
            }
        }

        public static async Task<int> AddRaffle(string Name, DateTime End, int Prize)
        {
            Raffle raffle = new Raffle { CreateDate = DateTime.Now, DoneDate = End, RaffleName = Name, TokenPrize = Prize };
            using (var DbContext = new SQLiteDbContext())
            {
                DbContext.Raffles.Add(raffle);
                await DbContext.SaveChangesAsync();
                return raffle.Id;
            }
        }

        public static async Task JoinRaffle(int RaffleId, ulong UserId)
        {
            RaffleEntries entry = new RaffleEntries { RaffleId = RaffleId, UserId = UserId };
            using (var DbContext = new SQLiteDbContext())
            {
                DbContext.RaffleEntries.Add(entry);
                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task FinishRaffle(int RaffleId, ulong UserId)
        {
            using (var DbContext = new SQLiteDbContext())
            {
                var raffle = DbContext.Raffles.Find(RaffleId);
                raffle.WinnerId = UserId;
                await SaveTokens(UserId, raffle.TokenPrize);
                await DbContext.SaveChangesAsync();
            }
        }
    }
}
