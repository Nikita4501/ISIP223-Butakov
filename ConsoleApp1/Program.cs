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

        static void ShowProducts()
        {
            Console.Clear();
            Console.WriteLine("=== Список товаров ===");
            var products = Core.Context.Products.ToList();
            foreach (var p in products)
            {
                Console.WriteLine($"{p.ProductId}. {p.Name} — {p.Price}₽ (в наличии: {p.Stock})");
            }
        }

        static void AddToCart(Users user)
        {
            ShowProducts();
            Console.Write("Введите ID товара для добавления в корзину: ");
            if (int.TryParse(Console.ReadLine(), out int productId))
            {
                var product = Core.Context.Products.FirstOrDefault(p => p.ProductId == productId);
                if (product == null)
                {
                    Console.WriteLine("Товар не найден!");
                }
                else
                {
                    var existing = Core.Context.CartItems.FirstOrDefault(c => c.UserId == user.UserId && c.ProductId == productId);
                    if (existing != null)
                    {
                        existing.Quantity++;
                    }
                    else
                    {
                        Core.Context.CartItems.Add(new CartItems
                        {
                            UserId = user.UserId,
                            ProductId = productId,
                            Quantity = 1,
                            AddedAt = DateTime.Now
                        });
                    }
                    Core.Context.SaveChanges();
                    Console.WriteLine("Товар добавлен в корзину!");
                }
            }
            else Console.WriteLine("Некорректный ввод!");
            Pause();
        }

        static void ShowCart(Users user)
        {
            Console.Clear();
            Console.WriteLine("=== Ваша корзина ===");
            var cart = Core.Context.CartItems.Where(c => c.UserId == user.UserId).ToList();
            if (!cart.Any())
            {
                Console.WriteLine("Корзина пуста!");
                return;
            }

            foreach (var item in cart)
            {
                var product = Core.Context.Products.First(p => p.ProductId == item.ProductId);
                Console.WriteLine($"{product.Name} — {product.Price}₽ × {item.Quantity}");
            }
        }

        static void BuySingleItem(Users user)
        {
            ShowProducts();
            Console.Write("Введите ID товара для покупки: ");
            if (!int.TryParse(Console.ReadLine(), out int productId)) return;

            var product = Core.Context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                Console.WriteLine("Товар не найден!");
                Pause();
                return;
            }

            var pvz = ChoosePickupPoint();
            if (pvz == null) return;

            var order = new Orders
            {
                UserId = user.UserId,
                PickupPointId = pvz.PickupPointId,
                CreatedAt = DateTime.Now,
                TotalAmount = product.Price,
                Status = "Создан"
            };
            Core.Context.Orders.Add(order);
            Core.Context.SaveChanges();

            Core.Context.OrderItems.Add(new OrderItems
            {
                OrderId = order.OrderId,
                ProductId = product.ProductId,
                UnitPrice = product.Price,
                Quantity = 1
            });
            Core.Context.SaveChanges();

            Console.WriteLine($"Товар '{product.Name}' успешно куплен! Пункт выдачи: {pvz.Address}");
            Pause();
        }

        static void BuyAllCart(Users user)
        {
            var cart = Core.Context.CartItems.Where(c => c.UserId == user.UserId).ToList();
            if (!cart.Any())
            {
                Console.WriteLine("Корзина пуста!");
                Pause();
                return;
            }

            var pvz = ChoosePickupPoint();
            if (pvz == null) return;

            decimal total = 0;
            foreach (var item in cart)
            {
                var product = Core.Context.Products.First(p => p.ProductId == item.ProductId);
                total += product.Price * item.Quantity;
            }

            var order = new Orders
            {
                UserId = user.UserId,
                PickupPointId = pvz.PickupPointId,
                CreatedAt = DateTime.Now,
                TotalAmount = total,
                Status = "Создан"
            };
            Core.Context.Orders.Add(order);
            Core.Context.SaveChanges();

            foreach (var item in cart)
            {
                var product = Core.Context.Products.First(p => p.ProductId == item.ProductId);
                Core.Context.OrderItems.Add(new OrderItems
                {
                    OrderId = order.OrderId,
                    ProductId = product.ProductId,
                    UnitPrice = product.Price,
                    Quantity = item.Quantity
                });
            }
            Core.Context.CartItems.RemoveRange(cart);
            Core.Context.SaveChanges();

            Console.WriteLine("Все товары из корзины куплены!");
            Pause();
        }

        static void ShowOrders(Users user)
        {
            Console.Clear();
            Console.WriteLine("=== Ваши заказы ===");
            var orders = Core.Context.Orders
                .Where(o => o.UserId == user.UserId)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            if (!orders.Any())
            {
                Console.WriteLine("У вас нет заказов!");
                return;
            }

            foreach (var order in orders)
            {
                Console.WriteLine($"Заказ #{order.OrderId} — {order.TotalAmount}₽, статус: {order.Status}, дата: {order.CreatedAt}");
            }
        }

        static PickupPoints ChoosePickupPoint()
        {
            Console.Clear();
            Console.WriteLine("=== Выберите пункт выдачи ===");
            var points = Core.Context.PickupPoints.ToList();
            foreach (var p in points)
            {
                Console.WriteLine($"{p.PickupPointId}. {p.City}, {p.Address}");
            }
            Console.Write("Введите ID пункта выдачи: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var pvz = Core.Context.PickupPoints.FirstOrDefault(p => p.PickupPointId == id);
                if (pvz != null) return pvz;
            }
            Console.WriteLine("Некорректный выбор!");
            Pause();
            return null;
        }

        static void Pause()
        {
            Console.WriteLine("\nНажмите любую клавишу, чтобы продолжить...");
            Console.ReadKey();
        }
    }
}
