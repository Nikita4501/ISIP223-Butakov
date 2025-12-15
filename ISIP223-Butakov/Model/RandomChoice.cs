using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP223_Butakov.Model
{
    public static class RandomChoice
    {
        private static Random random = new Random();

        public static int GetRandomNumber(int min, int max)
        {
            return random.Next(min, max);
        }

        public static int GetRandomNumber(int max)
        {
            return random.Next(max);
        }

        public static bool GetChance(int percentage)
        {
            return random.Next(100) < percentage;
        }

        public static double GetRandomDouble()
        {
            return random.NextDouble();
        }
    }
}
