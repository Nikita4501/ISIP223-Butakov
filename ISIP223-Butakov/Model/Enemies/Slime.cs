using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP223_Butakov.Model.Enemies
{
    public class Slime : Enemy
    {
        public Slime()
        {
            Name = "Слизень";
            HP = 35;
            Attack = 10;
            Defense = 0;
        }

        public override void SpecialAttack(Player player, int blockedDamage, ref bool playerFrozen)
        {
            // Слизень уменьшает входящий урон на 2
            int damage = Attack - blockedDamage;
            if (damage < 0) damage = 0;

            Console.WriteLine($"Слизень атакует! Нанесено {damage} урона!");
            player.TakeDamage(damage);
        }

        // Переопределяем метод получения урона для слизня
        public new void TakeDamage(int damage)
        {
            int reducedDamage = damage - 2;
            if (reducedDamage < 0) reducedDamage = 0;

            HP -= reducedDamage;
            Console.WriteLine($"Слизень уменьшает урон на 2 единицы!");
            if (HP < 0) HP = 0;
        }
    }
}
