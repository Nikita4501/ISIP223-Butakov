using System;
using System.Collections.Generic;

namespace TextRoguelike
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Start();
        }
    }

    public class Game
    {
        private Player player;
        private Random random;
        private int turnCount;

        public Game()
        {
            random = new Random();
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
                    if (random.Next(2) == 0)
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
            Enemy enemy;
            int enemyType = random.Next(3);

            switch (enemyType)
            {
                case 0:
                    enemy = new Goblin();
                    break;
                case 1:
                    enemy = new Skeleton();
                    break;
                case 2:
                    enemy = new Mage();
                    break;
                default:
                    enemy = new Goblin();
                    break;
            }

            Console.WriteLine($"Вы встретили {enemy.Name}!");
            Console.WriteLine($"Здоровье врага: {enemy.HP}, Атака: {enemy.Attack}, Защита: {enemy.Defense}");

            StartCombat(enemy);
        }

        private void EncounterBoss()
        {
            Boss boss;
            int bossType = random.Next(4);

            switch (bossType)
            {
                case 0:
                    boss = new VVG();
                    break;
                case 1:
                    boss = new Kovalsky();
                    break;
                case 2:
                    boss = new ArchmageCPP();
                    break;
                case 3:
                    boss = new PestovC();
                    break;
                default:
                    boss = new VVG();
                    break;
            }

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
                if (random.Next(100) < 40)
                {
                    Console.WriteLine("Вы успешно уклонились от атаки!");
                    player.IsDefending = false;
                    return;
                }
                else
                {
                    // Блок: уменьшение урона на 70-100% от защиты
                    double blockPercentage = 0.7 + (random.NextDouble() * 0.3);
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
            int itemType = random.Next(3);

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
            int attack = random.Next(15, 31);
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
            int defense = random.Next(8, 16);
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

    public class Player
    {
        public int HP { get; set; }
        public Weapon Weapon { get; private set; }
        public Armor Armor { get; private set; }
        public int Attack => Weapon.Attack;
        public int Defense => Armor.Defense;
        public bool IsDefending { get; set; }
        public bool IsAlive => HP > 0;

        public Player()
        {
            HP = 100;
            Weapon = new Weapon(10);
            Armor = new Armor(5);
            IsDefending = false;
        }

        public void EquipWeapon(Weapon weapon)
        {
            Weapon = weapon;
        }

        public void EquipArmor(Armor armor)
        {
            Armor = armor;
        }

        public void TakeDamage(int damage)
        {
            HP -= damage;
            if (HP < 0) HP = 0;
        }
    }

    public abstract class Enemy
    {
        public string Name { get; protected set; }
        public int HP { get; protected set; }
        public int Attack { get; protected set; }
        public int Defense { get; protected set; }
        public bool IsAlive => HP > 0;

        public void TakeDamage(int damage)
        {
            HP -= damage;
            if (HP < 0) HP = 0;
        }

        public abstract void SpecialAttack(Player player, int blockedDamage, ref bool playerFrozen);
    }

    public class Goblin : Enemy
    {
        private Random random;

        public Goblin()
        {
            Name = "Гоблин";
            HP = 30;
            Attack = 12;
            Defense = 3;
            random = new Random();
        }

        public override void SpecialAttack(Player player, int blockedDamage, ref bool playerFrozen)
        {
            // 20% шанс критического удара
            if (random.Next(100) < 20)
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

    public class Mage : Enemy
    {
        private Random random;

        public Mage()
        {
            Name = "Маг";
            HP = 20;
            Attack = 18;
            Defense = 1;
            random = new Random();
        }

        public override void SpecialAttack(Player player, int blockedDamage, ref bool playerFrozen)
        {
            int damage = Attack - blockedDamage;
            if (damage < 0) damage = 0;

            // 25% шанс заморозки
            if (random.Next(100) < 25)
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

    public abstract class Boss : Enemy
    {
        // Базовые характеристики для расчета множителей
        protected const int BASE_HP = 30;
        protected const int BASE_ATTACK = 12;
        protected const int BASE_DEFENSE = 3;
    }

    public class VVG : Boss
    {
        private Random random;

        public VVG()
        {
            Name = "ВВГ (Гоблин-босс)";
            HP = (int)(BASE_HP * 2.0);
            Attack = (int)(BASE_ATTACK * 1.5);
            Defense = (int)(BASE_DEFENSE * 1.2);
            random = new Random();
        }

        public override void SpecialAttack(Player player, int blockedDamage, ref bool playerFrozen)
        {
            // 30% шанс критического удара (20% базовый + 10%)
            if (random.Next(100) < 30)
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

    public class ArchmageCPP : Boss
    {
        private Random random;

        public ArchmageCPP()
        {
            Name = "Архимаг C++";
            HP = (int)(BASE_HP * 1.8);
            Attack = (int)(BASE_ATTACK * 1.6);
            Defense = (int)(BASE_DEFENSE * 1.1);
            random = new Random();
        }

        public override void SpecialAttack(Player player, int blockedDamage, ref bool playerFrozen)
        {
            int damage = Attack - blockedDamage;
            if (damage < 0) damage = 0;

            // 35% шанс заморозки (25% базовый + 10%)
            if (random.Next(100) < 35)
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

    public class PestovC : Boss
    {
        private Random random;

        public PestovC()
        {
            Name = "Пестов С--";
            HP = (int)(BASE_HP * 1.3);
            Attack = (int)(BASE_ATTACK * 1.8);
            Defense = (int)(BASE_DEFENSE * 0.6);
            random = new Random();
        }

        public override void SpecialAttack(Player player, int blockedDamage, ref bool playerFrozen)
        {
            // Игнорирует защиту игрока + шанс заморозки
            int damage = Attack;

            // 40% шанс заморозки (25% базовый + 15%)
            if (random.Next(100) < 40)
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

    public class Weapon
    {
        public int Attack { get; }

        public Weapon(int attack)
        {
            Attack = attack;
        }
    }

    public class Armor
    {
        public int Defense { get; }

        public Armor(int defense)
        {
            Defense = defense;
        }
    }
}