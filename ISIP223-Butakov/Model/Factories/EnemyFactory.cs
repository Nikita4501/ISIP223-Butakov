using ISIP223_Butakov.Model.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP223_Butakov.Model.Factories
{
    public static class EnemyFactory
    {
        public static Enemy CreateEnemy()
        {
            int enemyType = RandomChoice.GetRandomNumber(4); // Теперь 4 типа монстров

            switch (enemyType)
            {
                case 0:
                    return new Goblin();
                case 1:
                    return new Skeleton();
                case 2:
                    return new Mage();
                case 3:
                    return new Slime();
                default:
                    return new Goblin();
            }
        }
    }
}