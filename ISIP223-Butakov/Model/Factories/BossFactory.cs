using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP223_Butakov.Model.Factories
{
    public static class BossFactory
    {
        public static Boss CreateBoss()
        {
            int bossType = RandomChoice.GetRandomNumber(4);

            switch (bossType)
            {
                case 0:
                    return new VVG();
                case 1:
                    return new Kovalsky();
                case 2:
                    return new ArchmageCPP();
                case 3:
                    return new PestovC();
                default:
                    return new VVG();
            }
        }
    }
}