using ISIP223_Butakov.Model.Enemies;
using ISIP223_Butakov.Model.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP223_Butakov.Model
{
    public class Game
    {
        private Player player;
        private int turnCount;

        public Game()
        {
            player = new Player();
            turnCount = 0;
        }

        public void Start()
        {
            Console.WriteLine("=== ТЕКСТОВЫЙ РОГАЛИК ===");
            Console.WriteLine("Добро пожаловать в игру!");
            Console.WriteLine("Каждый ход вас ждет либо сундук, либо враг.");
            Console.WriteLine("Каждые 10 ходов - босс!\n");

            while (player.IsAlive)
            {
                turnCount++;
                Console.WriteLine($"\n--- Ход {turnCount} ---");
                Console.WriteLine($"Здоровье игрока: {player.HP}");

                // Каждые 10 ходов - босс
                if (turnCount % 10 == 0)
                {
                    EncounterBoss();
                }
                else
                {
                    // 50/50 шанс на сундук или врага
                    if (RandomChoice.GetRandomNumber(2) == 0)
                    {
                        EncounterEnemy();
                    }
                    else
                    {
                        OpenChest();
                    }
                }

                if (player.IsAlive)
                {
                    Console.WriteLine("Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                }
            }

            Console.WriteLine("\n=== ИГРА ОКОНЧЕНА ===");
            Console.WriteLine($"Вы продержались {turnCount} ходов!");
        }

        private void EncounterEnemy()
        {
            Enemy enemy = EnemyFactory.CreateEnemy();

            Console.WriteLine($"Вы встретили {enemy.Name}!");
            Console.WriteLine($"Здоровье врага: {enemy.HP}, Атака: {enemy.Attack}, Защита: {enemy.Defense}");

            StartCombat(enemy);
        }

        private void EncounterBoss()
        {
            Boss boss = BossFactory.CreateBoss();

            Console.WriteLine($"!!! ВНИМАНИЕ БОСС !!!");
            Console.WriteLine($"Вы встретили {boss.Name}!");
            Console.WriteLine($"Здоровье босса: {boss.HP}, Атака: {boss.Attack}, Защита: {boss.Defense}");

            StartCombat(boss);
        }

        private void StartCombat(Enemy enemy)
        {
            bool playerFrozen = false;

            while (enemy.IsAlive && player.IsAlive)
            {
                if (!playerFrozen)
                {
                    PlayerTurn(enemy);
                }
                else
                {
                    Console.WriteLine("Вы заморожены и пропускаете ход!");
                    playerFrozen = false;
                }

                if (enemy.IsAlive)
                {
                    EnemyTurn(enemy, ref playerFrozen);
                }
            }

            if (player.IsAlive)
            {
                Console.WriteLine($"Вы победили {enemy.Name}!");
            }
        }

        private void PlayerTurn(Enemy enemy)
        {
            Console.WriteLine("\nВаш ход:");
            Console.WriteLine("1 - Атаковать");
            Console.WriteLine("2 - Защищаться");

            int choice = GetChoice(1, 2);

            if (choice == 1)
            {
                int damage = player.Attack;
                Console.WriteLine($"Вы атакуете и наносите {damage} урона!");
                enemy.TakeDamage(damage);
            }
            else
            {
                player.IsDefending = true;
                Console.WriteLine("Вы готовитесь к защите!");
            }
        }

        private void EnemyTurn(Enemy enemy, ref bool playerFrozen)
        {
            Console.WriteLine($"\nХод {enemy.Name}:");

            if (player.IsDefending)
            {
                // Шанс уклонения 40%
                if (RandomChoice.GetChance(40))
                {
                    Console.WriteLine("Вы успешно уклонились от атаки!");
                    player.IsDefending = false;
                    return;
                }
                else
                {
                    // Блок: уменьшение урона на 70-100% от защиты
                    double blockPercentage = 0.7 + (RandomChoice.GetRandomDouble() * 0.3);
                    int blockedDamage = (int)(player.Defense * blockPercentage);
                    Console.WriteLine($"Вы блокируете {blockedDamage} урона!");
                    enemy.SpecialAttack(player, blockedDamage, ref playerFrozen);
                    player.IsDefending = false;
                }
            }
            else
            {
                enemy.SpecialAttack(player, 0, ref playerFrozen);
            }
        }

        private void OpenChest()
        {
            Console.WriteLine("Вы нашли сундук!");
            int itemType = RandomChoice.GetRandomNumber(3);

            switch (itemType)
            {
                case 0:
                    Console.WriteLine("В сундуке лечебное зелье!");
                    player.HP = 100;
                    Console.WriteLine("Ваше здоровье полностью восстановлено!");
                    break;
                case 1:
                    GetNewWeapon();
                    break;
                case 2:
                    GetNewArmor();
                    break;
            }
        }

        private void GetNewWeapon()
        {
            int attack = RandomChoice.GetRandomNumber(15, 31);
            Weapon newWeapon = new Weapon(attack);

            Console.WriteLine($"В сундуке новое оружие с атакой: {newWeapon.Attack}");
            Console.WriteLine($"Ваше текущее оружие: {player.Weapon.Attack} атаки");

            Console.WriteLine("1 - Взять новое оружие");
            Console.WriteLine("2 - Выбросить");

            int choice = GetChoice(1, 2);

            if (choice == 1)
            {
                player.EquipWeapon(newWeapon);
                Console.WriteLine("Вы экипировали новое оружие!");
            }
        }

        private void GetNewArmor()
        {
            int defense = RandomChoice.GetRandomNumber(8, 16);
            Armor newArmor = new Armor(defense);

            Console.WriteLine($"В сундуке новые доспехи с защитой: {newArmor.Defense}");
            Console.WriteLine($"Ваши текущие доспехи: {player.Armor.Defense} защиты");

            Console.WriteLine("1 - Взять новые доспехи");
            Console.WriteLine("2 - Выбросить");

            int choice = GetChoice(1, 2);

            if (choice == 1)
            {
                player.EquipArmor(newArmor);
                Console.WriteLine("Вы экипировали новые доспехи!");
            }
        }

        private int GetChoice(int min, int max)
        {
            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < min || choice > max)
            {
                Console.WriteLine($"Пожалуйста, введите число от {min} до {max}");
            }
            return choice;
        }
    }
}
