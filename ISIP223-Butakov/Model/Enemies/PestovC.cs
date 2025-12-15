using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP223_Butakov.Model.Enemies
{
    public class PestovC : Boss
    {
        public PestovC()
        {
            Name = "Пестов С--";
            HP = (int)(BASE_HP * 1.3);
            Attack = (int)(BASE_ATTACK * 1.8);
            Defense = (int)(BASE_DEFENSE * 0.6);
        }

        public override void SpecialAttack(Player player, int blockedDamage, ref bool playerFrozen)
        {
            // Игнорирует защиту игрока + шанс заморозки
            int damage = Attack;

            // 40% шанс заморозки (25% базовый + 15%)
            if (RandomChoice.GetChance(40))
            {
                Console.WriteLine($"Пестов С-- атакует с заморозкой, игнорируя защиту! Нанесено {damage} урона, вы пропустите следующий ход!");
                playerFrozen = true;
                player.TakeDamage(damage);
            }
            else
            {
                Console.WriteLine($"Пестов С-- атакует, игнорируя защиту! Нанесено {damage} урона!");
                player.TakeDamage(damage);
            }
        }
    }
}
