using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP223_Butakov.Model.Enemies
{
    public class Mage : Enemy
    {
        public Mage()
        {
            Name = "Маг";
            HP = 20;
            Attack = 18;
            Defense = 1;
        }

        public override void SpecialAttack(Player player, int blockedDamage, ref bool playerFrozen)
        {
            int damage = Attack - blockedDamage;
            if (damage < 0) damage = 0;

            // 25% шанс заморозки
            if (RandomChoice.GetChance(25))
            {
                Console.WriteLine($"Маг замораживает вас! Нанесено {damage} урона, вы пропустите следующий ход!");
                playerFrozen = true;
                player.TakeDamage(damage);
            }
            else
            {
                Console.WriteLine($"Маг атакует! Нанесено {damage} урона!");
                player.TakeDamage(damage);
            }
        }
    }
}
