using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP223_Butakov.Model.Enemies
{
    public class VVG : Boss
    {
        public VVG()
        {
            Name = "ВВГ (Гоблин-босс)";
            HP = (int)(BASE_HP * 2.0);
            Attack = (int)(BASE_ATTACK * 1.5);
            Defense = (int)(BASE_DEFENSE * 1.2);
        }

        public override void SpecialAttack(Player player, int blockedDamage, ref bool playerFrozen)
        {
            // 30% шанс критического удара (20% базовый + 10%)
            if (RandomChoice.GetChance(30))
            {
                int critDamage = Attack * 2 - blockedDamage;
                if (critDamage < 0) critDamage = 0;
                Console.WriteLine($"Мощный критический удар! Нанесено {critDamage} урона!");
                player.TakeDamage(critDamage);
            }
            else
            {
                int damage = Attack - blockedDamage;
                if (damage < 0) damage = 0;
                Console.WriteLine($"ВВГ атакует! Нанесено {damage} урона!");
                player.TakeDamage(damage);
            }
        }
    }
}
