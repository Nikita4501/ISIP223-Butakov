using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP223_Butakov.Model.Enemies
{
    public class ArchmageCPP : Boss
    {
        public ArchmageCPP()
        {
            Name = "Архимаг C++";
            HP = (int)(BASE_HP * 1.8);
            Attack = (int)(BASE_ATTACK * 1.6);
            Defense = (int)(BASE_DEFENSE * 1.1);
        }

        public override void SpecialAttack(Player player, int blockedDamage, ref bool playerFrozen)
        {
            int damage = Attack - blockedDamage;
            if (damage < 0) damage = 0;

            // 35% шанс заморозки (25% базовый + 10%)
            if (RandomChoice.GetChance(35))
            {
                Console.WriteLine($"Мощная заморозка! Нанесено {damage} урона, вы пропустите следующий ход!");
                playerFrozen = true;
                player.TakeDamage(damage);
            }
            else
            {
                Console.WriteLine($"Архимаг C++ атакует! Нанесено {damage} урона!");
                player.TakeDamage(damage);
            }
        }
    }
}