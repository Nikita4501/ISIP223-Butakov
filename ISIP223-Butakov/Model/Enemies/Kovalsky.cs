using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP223_Butakov.Model.Enemies
{
    public class Kovalsky : Boss
    {
        public Kovalsky()
        {
            Name = "Ковальский (Скелет-босс)";
            HP = (int)(BASE_HP * 2.5);
            Attack = (int)(BASE_ATTACK * 1.3);
            Defense = (int)(BASE_DEFENSE * 1.4);
        }

        public override void SpecialAttack(Player player, int blockedDamage, ref bool playerFrozen)
        {
            // Игнорирует защиту игрока
            Console.WriteLine($"Ковальский атакует, полностью игнорируя защиту! Нанесено {Attack} урона!");
            player.TakeDamage(Attack);
        }
    }
}