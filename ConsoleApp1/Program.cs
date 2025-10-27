using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static Random random = new Random();
        static AutoServiceAEntities context = new AutoServiceAEntities();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            try
            {
                InitializeDatabase();
                Console.WriteLine("=== АВТОСЕРВИС 'МАСТЕР' ===");
                Console.WriteLine("Добро пожаловать! Ваша задача - чинить машины и зарабатывать деньги!");

                MainGameLoop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка: {ex.Message}");
                Console.WriteLine("Нажмите любую клавишу для выхода...");
                Console.ReadKey();
            }
        }

        static void InitializeDatabase()
        {
            try
            {
                if (!context.Database.Exists())
                {
                    throw new Exception("База данных не доступна. Проверьте подключение.");
                }

                // Создаем запись автосервиса если её нет
                if (!context.AutoService.Any())
                {
                    context.AutoService.Add(new AutoService
                    {
                        AutoServiceID = 1,
                        Balance = 10000,
                        ClientCounter = 0
                    });
                    context.SaveChanges();
                }

                // Добавляем начальные детали и склад если их нет
                if (!context.Parts.Any())
                {
                    InitializePartsAndWarehouse();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка инициализации БД: {ex.Message}");
                throw;
            }
        }

        static void InitializePartsAndWarehouse()
        {
            var parts = new List<Parts>
            {
                new Parts { Name = "Двигатель", BasePrice = 5000 },
                new Parts { Name = "Тормозные колодки", BasePrice = 200 },
                new Parts { Name = "Аккумулятор", BasePrice = 300 },
                new Parts { Name = "Шины", BasePrice = 400 },
                new Parts { Name = "Фары", BasePrice = 150 },
                new Parts { Name = "Тормозные диски", BasePrice = 350 },
                new Parts { Name = "Амортизаторы", BasePrice = 450 },
                new Parts { Name = "Стартер", BasePrice = 280 },
                new Parts { Name = "Генератор", BasePrice = 320 },
                new Parts { Name = "Ремень ГРМ", BasePrice = 180 }
            };

            context.Parts.AddRange(parts);
            context.SaveChanges();

            // Заполняем склад начальными количествами
            var warehouseItems = new List<Warehouse>();
            var quantities = new int[] { 2, 10, 5, 8, 6, 4, 3, 2, 3, 7 };

            for (int i = 0; i < parts.Count; i++)
            {
                warehouseItems.Add(new Warehouse
                {
                    PartID = parts[i].PartID,
                    Quantity = quantities[i]
                });
            }

            context.Warehouse.AddRange(warehouseItems);
            context.SaveChanges();
        }

        static void LogOperation(string operationType, string details, decimal balanceChange,
                               int? orderID = null, int? supplyOrderID = null)
        {
            try
            {
                var log = new OperationHistory
                {
                    AutoServiceID = 1,
                    OrderID = orderID,
                    SupplyOrderID = supplyOrderID,
                    OperationType = operationType,
                    Details = details,
                    BalanceChange = balanceChange,
                    OperationDate = DateTime.Now
                };

                context.OperationHistory.Add(log);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка логирования: {ex.Message}");
            }
        }

        static decimal GetBalance()
        {
            return context.AutoService.First().Balance;
        }

        static void UpdateBalance(decimal amount)
        {
            try
            {
                var service = context.AutoService.First();
                service.Balance += amount;

                if (service.Balance < 0)
                    service.Balance = 0;

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обновления баланса: {ex.Message}");
            }
        }

        static void IncrementClientCounter()
        {
            var service = context.AutoService.First();
            service.ClientCounter++;
            context.SaveChanges();
        }

        static int GetClientCounter()
        {
            return context.AutoService.First().ClientCounter;
        }

        static int GetPartQuantity(int partID)
        {
            var warehouse = context.Warehouse.FirstOrDefault(w => w.PartID == partID);
            return warehouse?.Quantity ?? 0;
        }

        static void UpdatePartQuantity(int partID, int newQuantity)
        {
            var warehouse = context.Warehouse.FirstOrDefault(w => w.PartID == partID);
            if (warehouse != null)
            {
                warehouse.Quantity = newQuantity;
                warehouse.LastRestockDate = DateTime.Now;
                context.SaveChanges();
            }
        }

        static void MainGameLoop()
        {
            bool exit = false;
            while (!exit)
            {
                try
                {
                    Console.Clear();
                    ShowMainMenu();

                    var input = Console.ReadLine();
                    switch (input)
                    {
                        case "1":
                            ProcessNewClient();
                            break;
                        case "2":
                            ShowWarehouse();
                            break;
                        case "3":
                            OrderPartsMenu();
                            break;
                        case "4":
                            ShowBalanceAndHistory();
                            break;
                        case "5":
                            exit = true;
                            Console.WriteLine("Спасибо за игру! До свидания!");
                            break;
                        default:
                            Console.WriteLine("Неверный ввод. Нажмите любую клавишу для продолжения...");
                            Console.ReadKey();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    Console.WriteLine("Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                }
            }
        }

        static void ShowMainMenu()
        {
            decimal balance = GetBalance();
            Console.WriteLine($"\nТекущий баланс: {balance} руб.");
            Console.WriteLine($"Обслужено клиентов: {GetClientCounter()}");
            Console.WriteLine("\nГлавное меню:");
            Console.WriteLine("1. Обработать нового клиента");
            Console.WriteLine("2. Просмотреть склад");
            Console.WriteLine("3. Заказать запчасти");
            Console.WriteLine("4. Баланс и история операций");
            Console.WriteLine("5. Выход");
            Console.Write("Выберите действие: ");
        }

        static void ProcessNewClient()
        {
            Console.Clear();
            Console.WriteLine("=== НОВЫЙ КЛИЕНТ ===");

            var client = GenerateRandomClient();
            var brokenPart = GetRandomPart();

            Console.WriteLine($"Клиент: {client.Name}");
            Console.WriteLine($"Машина: {client.CarModel}");
            Console.WriteLine($"Поломка: {brokenPart.Name}");

            decimal repairCost = CalculateRepairCost(brokenPart);
            int availableQuantity = GetPartQuantity(brokenPart.PartID);

            Console.WriteLine($"Стоимость ремонта: {repairCost} руб.");
            Console.WriteLine($"На складе есть: {availableQuantity} шт.");

            // Создаем заказ в БД
            var serviceOrder = new ServiceOrders
            {
                ClientID = client.ClientID,
                RequiredPartID = brokenPart.PartID,
                RepairCost = repairCost,
                OrderDate = DateTime.Now,
                Status = "Pending"
            };

            context.ServiceOrders.Add(serviceOrder);
            context.SaveChanges();

            Console.WriteLine("\nВаши действия:");
            Console.WriteLine("1. Принять заказ и починить");
            Console.WriteLine("2. Отказать клиенту (штраф 500 руб.)");
            Console.Write("Выберите действие: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    AcceptOrder(client, serviceOrder, brokenPart, repairCost);
                    break;
                case "2":
                    RefuseOrder(client, serviceOrder);
                    break;
                default:
                    Console.WriteLine("Неверный ввод. Отказ от заказа.");
                    RefuseOrder(client, serviceOrder);
                    break;
            }

            UpdateDeliveryProgress();
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static Clients GenerateRandomClient()
        {
            var names = new[] { "Иван Петров", "Мария Сидорова", "Алексей Козлов", "Елена Волкова", "Дмитрий Орлов" };
            var carModels = new[] { "Toyota Camry", "Lada Vesta", "Kia Rio", "Hyundai Solaris", "Volkswagen Polo" };

            var client = new Clients
            {
                Name = names[random.Next(names.Length)],
                CarModel = carModels[random.Next(carModels.Length)],
                VisitDate = DateTime.Now,
                Status = "Pending"
            };

            context.Clients.Add(client);
            context.SaveChanges();

            return client;
        }

        static Parts GetRandomPart()
        {
            var parts = context.Parts.ToList();
            return parts[random.Next(parts.Count)];
        }

        static decimal CalculateRepairCost(Parts part)
        {
            return part.BasePrice + (part.BasePrice * 0.3m);
        }

        static void AcceptOrder(Clients client, ServiceOrders order, Parts brokenPart, decimal repairCost)
        {
            int availableQuantity = GetPartQuantity(brokenPart.PartID);

            if (availableQuantity > 0)
            {
                // Успешный ремонт
                UpdatePartQuantity(brokenPart.PartID, availableQuantity - 1);
                UpdateBalance(repairCost);

                order.Status = "Completed";
                order.UsedPartID = brokenPart.PartID;
                client.Status = "Completed";

                LogOperation("Успешный ремонт",
                    $"Починили {brokenPart.Name} для {client.Name}. Получено: {repairCost} руб.",
                    repairCost, order.OrderID);

                Console.WriteLine($"Ремонт выполнен успешно! Вы заработали {repairCost} руб.");
            }
            else
            {
                // Нет нужной детали
                Console.WriteLine("Нужной детали нет на складе! Пытаемся заменить другой...");
                ReplaceWithRandomPart(client, order, brokenPart, repairCost);
            }

            IncrementClientCounter();
            context.SaveChanges();
        }

        static void ReplaceWithRandomPart(Clients client, ServiceOrders order, Parts originalPart, decimal originalCost)
        {
            var availableParts = context.Warehouse
                .Where(w => w.Quantity > 0 && w.PartID != originalPart.PartID)
                .Select(w => w.Parts)
                .ToList();

            if (availableParts.Any())
            {
                var replacementPart = availableParts[random.Next(availableParts.Count)];
                UpdatePartQuantity(replacementPart.PartID, GetPartQuantity(replacementPart.PartID) - 1);

                decimal penalty = originalCost * 2m;
                UpdateBalance(-penalty);
                order.Status = "Failed";
                order.UsedPartID = replacementPart.PartID;
                order.Notes = $"Заменена на {replacementPart.Name}";
                client.Status = "Failed";

                LogOperation("Неудачный ремонт",
                    $"Заменили {originalPart.Name} на {replacementPart.Name} для {client.Name}. Штраф: {penalty} руб.",
                    -penalty, order.OrderID);

                Console.WriteLine($"Заменили {originalPart.Name} на {replacementPart.Name}");
                Console.WriteLine($"Клиент недоволен! Штраф: {penalty} руб.");
            }
            else
            {
                RefuseOrder(client, order);
            }
        }

        static void RefuseOrder(Clients client, ServiceOrders order)
        {
            decimal penalty = 500;
            UpdateBalance(-penalty);
            order.Status = "Refused";
            client.Status = "Refused";

            LogOperation("Отказ от обслуживания",
                $"Отказали клиенту {client.Name}. Штраф: {penalty} руб.",
                -penalty, order.OrderID);

            Console.WriteLine($"Вы отказали клиенту. Штраф: {penalty} руб.");
            IncrementClientCounter();
            context.SaveChanges();
        }

        static void ShowWarehouse()
        {
            Console.Clear();
            Console.WriteLine("=== СКЛАД ЗАПЧАСТЕЙ ===\n");

            var warehouseData = context.Warehouse
                .Include("Parts")
                .OrderBy(w => w.Parts.Name)
                .ToList();

            Console.WriteLine($"{"Название",-20} {"Цена",-10} {"Количество",-12} {"Стоимость ремонта",-18}");
            Console.WriteLine(new string('-', 60));

            foreach (var item in warehouseData)
            {
                decimal repairCost = CalculateRepairCost(item.Parts);
                Console.WriteLine($"{item.Parts.Name,-20} {item.Parts.BasePrice,-10} {item.Quantity,-12} {repairCost,-18}");
            }

            var pendingOrders = context.SupplyOrders.Where(o => o.Status == "Ordered").ToList();
            if (pendingOrders.Any())
            {
                Console.WriteLine("\n=== ОЖИДАЕМЫЕ ПОСТАВКИ ===");
                foreach (var order in pendingOrders)
                {
                    var part = context.Parts.Find(order.PartID);
                    Console.WriteLine($"{part.Name}: {order.Quantity} шт. (доставка через {2 - order.DeliveryProgress} клиентов)");
                }
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static void OrderPartsMenu()
        {
            Console.Clear();
            Console.WriteLine("=== ЗАКАЗ ЗАПЧАСТЕЙ ===\n");

            var parts = context.Parts.OrderBy(p => p.Name).ToList();

            for (int i = 0; i < parts.Count; i++)
            {
                int currentQuantity = GetPartQuantity(parts[i].PartID);
                Console.WriteLine($"{i + 1}. {parts[i].Name} - {parts[i].BasePrice} руб./шт. (на складе: {currentQuantity} шт.)");
            }

            Console.Write("\nВыберите номер детали для заказа: ");
            if (int.TryParse(Console.ReadLine(), out int partIndex) && partIndex >= 1 && partIndex <= parts.Count)
            {
                var selectedPart = parts[partIndex - 1];

                Console.Write($"Введите количество {selectedPart.Name} для заказа: ");
                if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                {
                    decimal totalCost = selectedPart.BasePrice * quantity;
                    decimal balance = GetBalance();

                    if (totalCost <= balance)
                    {
                        var supplyOrder = new SupplyOrders
                        {
                            PartID = selectedPart.PartID,
                            Quantity = quantity,
                            UnitCost = selectedPart.BasePrice,
                            TotalCost = totalCost,
                            OrderDate = DateTime.Now,
                            DeliveryProgress = 0,
                            Status = "Ordered"
                        };

                        context.SupplyOrders.Add(supplyOrder);
                        UpdateBalance(-totalCost);

                        LogOperation("Заказ запчастей",
                            $"Заказали {quantity} шт. {selectedPart.Name} на сумму {totalCost} руб.",
                            -totalCost, null, supplyOrder.SupplyOrderID);

                        Console.WriteLine($"Заказ оформлен! Детали придут через 2 клиента.");
                    }
                    else
                    {
                        Console.WriteLine("Недостаточно средств для заказа!");
                    }
                }
                else
                {
                    Console.WriteLine("Неверное количество!");
                }
            }
            else
            {
                Console.WriteLine("Неверный выбор детали!");
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static void UpdateDeliveryProgress()
        {
            var pendingOrders = context.SupplyOrders.Where(o => o.Status == "Ordered").ToList();

            foreach (var order in pendingOrders)
            {
                order.DeliveryProgress++;

                if (order.DeliveryProgress >= 2)
                {
                    // Доставка завершена
                    var part = context.Parts.Find(order.PartID);
                    int currentQuantity = GetPartQuantity(order.PartID);
                    UpdatePartQuantity(order.PartID, currentQuantity + order.Quantity);
                    order.Status = "Delivered";

                    LogOperation("Доставка запчастей",
                        $"Получено {order.Quantity} шт. {part.Name}",
                        0, null, order.SupplyOrderID);
                }
            }

            context.SaveChanges();
        }

        static void ShowBalanceAndHistory()
        {
            Console.Clear();
            Console.WriteLine("=== БАЛАНС И ИСТОРИЯ ОПЕРАЦИЙ ===\n");

            decimal balance = GetBalance();
            Console.WriteLine($"Текущий баланс: {balance} руб.\n");

            var history = context.OperationHistory
                .OrderByDescending(h => h.OperationDate)
                .Take(10)
                .ToList();

            if (history.Any())
            {
                Console.WriteLine("Последние 10 операций:");
                Console.WriteLine($"{"Дата",-20} {"Тип операции",-25} {"Сумма",-10} {"Детали"}");
                Console.WriteLine(new string('-', 80));

                foreach (var record in history)
                {
                    string amount = record.BalanceChange != 0 ?
                        (record.BalanceChange > 0 ? $"+{record.BalanceChange}" : record.BalanceChange.ToString()) : "0";

                    Console.WriteLine($"{record.OperationDate:dd.MM.yyyy HH:mm,-20} {record.OperationType,-25} {amount,-10} {record.Details}");
                }
            }
            else
            {
                Console.WriteLine("История операций пуста.");
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }
}