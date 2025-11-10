using System;
using System.Linq;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Онлайн маркетплейс GMWOG";

            Users currentUser = null;
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== Добро пожаловать в GMWOG ===");
                Console.WriteLine("1. Вход");
                Console.WriteLine("2. Регистрация");
                Console.WriteLine("3. Просмотр товаров (гость)");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите пункт: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        currentUser = Login();
                        if (currentUser != null)
                            UserMenu(currentUser);
                        break;
                    case "2":
                        Register();
                        break;
                    case "3":
                        ShowProducts();
                        Pause();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Некорректный выбор!");
                        Pause();
                        break;
                }
            }
        }

        static void Register()
        {
            Console.Clear();
            Console.WriteLine("=== Регистрация ===");
            Console.Write("Введите логин: ");
            string username = Console.ReadLine();

            if (Core.Context.Users.Any(u => u.Username == username))
            {
                Console.WriteLine("Пользователь с таким логином уже существует!");
                Pause();
                return;
            }

            Console.Write("Введите пароль: ");
            string password1 = Console.ReadLine();
            Console.Write("Подтвердите пароль: ");
            string password2 = Console.ReadLine();

            if (password1 != password2)
            {
                Console.WriteLine("Пароли не совпадают!");
                Pause();
                return;
            }

            Console.Write("Введите имя: ");
            string fullname = Console.ReadLine();
            Console.Write("Введите email: ");
            string email = Console.ReadLine();

            var newUser = new Users
            {
                Username = username,
                PasswordHash = System.Text.Encoding.UTF8.GetBytes(password1),
                PasswordSalt = new byte[0],
                FullName = fullname,
                Email = email,
                CreatedAt = DateTime.Now
            };

            Core.Context.Users.Add(newUser);
            Core.Context.SaveChanges();

            Console.WriteLine("Регистрация успешно завершена!");
            Pause();
        }

        static Users Login()
        {
            Console.Clear();
            Console.WriteLine("=== Вход ===");
            Console.Write("Введите логин: ");
            string username = Console.ReadLine();
            Console.Write("Введите пароль: ");
            string password = Console.ReadLine();

            var user = Core.Context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null || System.Text.Encoding.UTF8.GetString(user.PasswordHash) != password)
            {
                Console.WriteLine("Неверный логин или пароль!");
                Pause();
                return null;
            }

            Console.WriteLine($"Добро пожаловать, {user.FullName}!");
            Pause();
            return user;
        }

        static void UserMenu(Users currentUser)
        {
            bool logout = false;

            while (!logout)
            {
                Console.Clear();
                Console.WriteLine($"=== Личный кабинет ({currentUser.Username}) ===");
                Console.WriteLine("1. Просмотреть товары");
                Console.WriteLine("2. Добавить товар в корзину");
                Console.WriteLine("3. Просмотреть корзину");
                Console.WriteLine("4. Купить один товар");
                Console.WriteLine("5. Купить все товары из корзины");
                Console.WriteLine("6. Мои заказы");
                Console.WriteLine("0. Выйти из аккаунта");
                Console.Write("Ваш выбор: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowProducts();
                        Pause();
                        break;
                    case "2":
                        AddToCart(currentUser);
                        break;
                    case "3":
                        ShowCart(currentUser);
                        Pause();
                        break;
                    case "4":
                        BuySingleItem(currentUser);
                        break;
                    case "5":
                        BuyAllCart(currentUser);
                        break;
                    case "6":
                        ShowOrders(currentUser);
                        Pause();
                        break;
                    case "0":
                        logout = true;
                        break;
                    default:
                        Console.WriteLine("Некорректный выбор!");
                        Pause();
                        break;
                }
            }
        }

        