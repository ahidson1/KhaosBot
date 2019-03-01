using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace KhaosBot.Resources.Database
{
    public class PlayToken
    {
        [Key]
        public ulong UserId { get; set; }
        public int Amount { get; set; }
    }
}
