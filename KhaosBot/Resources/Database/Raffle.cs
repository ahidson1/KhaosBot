using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhaosBot.Resources.Database
{
    public class Raffle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime DoneDate { get; set; }
        public string RaffleName { get; set; }
        public int TokenPrize { get; set; }
        public ulong? WinnerId { get; set; }

        public virtual List<RaffleEntries> Entries { get;set; }

    }
}
