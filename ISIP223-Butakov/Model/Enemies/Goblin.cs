using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP223_Butakov.Model.Enemies
{
    public class Goblin : Enemy
    {
        public Goblin()
        {
            Name = "Гоблин";
            HP = 30;
            Attack = 12;
            Defense = 3;
        }

        public override void SpecialAttack(Player player, int blockedDamage, ref bool playerFrozen)
        {
            // 20% шанс критического удара
            if (RandomChoice.GetChance(20))
            {
                int critDamage = Attack * 2 - blockedDamage;
                if (critDamage < 0) critDamage = 0;
                Console.WriteLine($"Критический удар! Нанесено {critDamage} урона!");
                player.TakeDamage(critDamage);
            }
            else
            {
                int damage = Attack - blockedDamage;
                if (damage < 0) damage = 0;
                Console.WriteLine($"Гоблин атакует! Нанесено {damage} урона!");
                player.TakeDamage(damage);
            }
        }
    }
}

