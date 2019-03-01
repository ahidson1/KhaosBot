using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhaosBot.Resources.Database
{
    public class RaffleEntries
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public ulong UserId { get; set; }
        public int RaffleId { get; set; }

        [ForeignKey("RaffleId")]
        public virtual Raffle Raffle { get; set; }
    }
}
