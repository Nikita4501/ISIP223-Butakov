using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP223_Butakov.Model.Enemies
{
    public class Skeleton : Enemy
    {
        public Skeleton()
        {
            Name = "Скелет";
            HP = 25;
            Attack = 15;
            Defense = 2;
        }

        public override void SpecialAttack(Player player, int blockedDamage, ref bool playerFrozen)
        {
            // Игнорирует защиту игрока
            Console.WriteLine($"Скелет атакует, игнорируя защиту! Нанесено {Attack} урона!");
            player.TakeDamage(Attack);
        }
    }
}
