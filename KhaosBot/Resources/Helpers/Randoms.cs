using System;
using System.Collections.Generic;
using System.Text;

namespace KhaosBot.Resources.Helpers
{
    public class Randoms
    {
        public static int CustomRandom(int topValue)
        {
            if (topValue == 1) return 1;

            topValue++;
            Random rnd = new Random();
            int ret = rnd.Next(1, topValue);
            return ret;
        }

        public static bool CoinFlip()
        {
            Random rnd = new Random();
            int flip = rnd.Next(1, 3);
            if (flip == 1) return false;
            else return true;
        }
    }
}
