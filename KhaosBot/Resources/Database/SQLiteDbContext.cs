using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace KhaosBot.Resources.Database
{
    public class SQLiteDbContext : DbContext
    {
        public DbSet<PlayToken> PlayTokens { get; set; }
        public DbSet<Raffle> Raffles { get; set; }
        public DbSet<RaffleEntries> RaffleEntries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder Options)
        {
            string DbLocation = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1\KhaosBot.dll", @"Data\");
            Options.UseSqlite($"Data Source={DbLocation}Database.sqlite");
        }
    }
}
