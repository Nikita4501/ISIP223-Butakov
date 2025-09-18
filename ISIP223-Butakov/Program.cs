using System;
using System.Collections.Generic;
using System.Linq;

namespace ShopInventory
{
    public enum ProductCategory
    {
        Electronics,
        Clothing,
        Food,
        Books,
        Sports
    }

    public class Product
    {
        public string Code { get; private set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool InStock => Quantity > 0;
        public ProductCategory Category { get; set; }

        public Product(string name, decimal price, int quantity, ProductCategory category)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название товара не может быть пустым");

            if (price < 0)
                throw new ArgumentException("Цена не может быть отрицательной");

            if (quantity < 0)
                throw new ArgumentException("Количество не может быть отрицательным");

            Name = name;
            Price = price;
            Quantity = quantity;
            Category = category;
        }

        public void SetCode(string code)
        {
            Code = code;
        }

        public override string ToString()
        {
            return $"Код: {Code}, Название: {Name}, Цена: {Price:C}, " +
                   $"Количество: {Quantity}, В наличии: {(InStock ? "Да" : "Нет")}, " +
                   $"Категория: {Category}";
        }
    }

    public class SaleRecord
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime SaleDate { get; set; }

        public SaleRecord(string productCode, string productName, int quantity, decimal totalAmount)
        {
            ProductCode = productCode;
            ProductName = productName;
            Quantity = quantity;
            TotalAmount = totalAmount;
            SaleDate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Товар: {ProductName} (код: {ProductCode}), " +
                   $"Количество: {Quantity}, Сумма: {TotalAmount:C}, " +
                   $"Дата: {SaleDate:yyyy-MM-dd HH:mm}";
        }
    }

    public class Shop
    {
        private List<Product> products;
        private Stack<SaleRecord> salesHistory;
        private List<SaleRecord> allSales;
        private int productCounter;

        public Shop()
        {
            products = new List<Product>();
            salesHistory = new Stack<SaleRecord>();
            allSales = new List<SaleRecord>();
            productCounter = 1;
            InitializeTestData();
        }

        private void InitializeTestData()
        {
            AddProduct("Смартфон", 29999.99m, 10, ProductCategory.Electronics);
            AddProduct("Футболка", 1999.50m, 25, ProductCategory.Clothing);
            AddProduct("Шоколад", 89.90m, 100, ProductCategory.Food);
            AddProduct("Программирование на C#", 1500.00m, 15, ProductCategory.Books);
            AddProduct("Футбольный мяч", 2499.00m, 8, ProductCategory.Sports);
        }

        private string GenerateProductCode()
        {
            return $"1{productCounter++:D6}";
        }

        public void AddProduct(string name, decimal price, int quantity, ProductCategory category)
        {
            try
            {
                var product = new Product(name, price, quantity, category);
                product.SetCode(GenerateProductCode());
                products.Add(product);
                Console.WriteLine($"Товар '{name}' успешно добавлен с кодом {product.Code}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ошибка при добавлении товара: {ex.Message}");
            }
        }

        public void RemoveProduct(string code)
        {
            var product = products.FirstOrDefault(p => p.Code == code);
            if (product != null)
            {
                products.Remove(product);
                Console.WriteLine($"Товар с кодом {code} успешно удален");
            }
            else
            {
                Console.WriteLine($"Товар с кодом {code} не найден");
            }
        }

        public void OrderSupply(string code, int quantity)
        {
            if (quantity <= 0)
            {
                Console.WriteLine("Количество поставки должно быть положительным");
                return;
            }

            var product = products.FirstOrDefault(p => p.Code == code);
            if (product != null)
            {
                product.Quantity += quantity;
                Console.WriteLine($"Поставка товара {product.Name} выполнена. Новое количество: {product.Quantity}");
            }
            else
            {
                Console.WriteLine($"Товар с кодом {code} не найден");
            }
        }

        public void SellProduct(string code, int quantity)
        {
            if (quantity <= 0)
            {
                Console.WriteLine("Количество продажи должно быть положительным");
                return;
            }

            var product = products.FirstOrDefault(p => p.Code == code);
            if (product != null)
            {
                if (product.Quantity < quantity)
                {
                    Console.WriteLine($"Недостаточно товара на складе. Доступно: {product.Quantity}");
                    return;
                }

                product.Quantity -= quantity;
                decimal totalAmount = product.Price * quantity;

                var saleRecord = new SaleRecord(product.Code, product.Name, quantity, totalAmount);
                salesHistory.Push(saleRecord);
                allSales.Add(saleRecord);

                Console.WriteLine($"Продажа выполнена: {quantity} шт. товара {product.Name}");
                Console.WriteLine($"Общая сумма: {totalAmount:C}");
            }
            else
            {
                Console.WriteLine($"Товар с кодом {code} не найден");
            }
        }

        public void UndoLastSale()
        {
            if (salesHistory.Count == 0)
            {
                Console.WriteLine("История продаж пуста");
                return;
            }

            var lastSale = salesHistory.Pop();
            var product = products.FirstOrDefault(p => p.Code == lastSale.ProductCode);

            if (product != null)
            {
                product.Quantity += lastSale.Quantity;
                allSales.Remove(lastSale);
                Console.WriteLine($"Последняя продажа отменена: {lastSale.Quantity} шт. товара {product.Name} возвращено на склад");
            }
            else
            {
                Console.WriteLine("Товар из последней продажи не найден в каталоге");
            }
        }

        public void PrintSalesReport()
        {
            if (allSales.Count == 0)
            {
                Console.WriteLine("Продаж еще не было");
                return;
            }

            Console.WriteLine("\n=== ОТЧЕТ О ПРОДАЖАХ ===");
            Console.WriteLine($"Всего продаж: {allSales.Count}");
            Console.WriteLine("----------------------------------------");

            decimal totalRevenue = 0;
            int totalItemsSold = 0;

            foreach (var sale in allSales)
            {
                Console.WriteLine(sale);
                totalRevenue += sale.TotalAmount;
                totalItemsSold += sale.Quantity;
            }

            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"Итого продано товаров: {totalItemsSold} шт.");
            Console.WriteLine($"Общая выручка: {totalRevenue:C}");
            Console.WriteLine("========================================");
        }

        public void SearchProducts(string searchTerm)
        {
            var results = products.Where(p =>
                p.Code.Contains(searchTerm) ||
                p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.Category.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            ).ToList();

            if (results.Count == 0)
            {
                Console.WriteLine("Товары не найдены");
                return;
            }

            Console.WriteLine($"\nНайдено товаров: {results.Count}");
            Console.WriteLine("========================================");

            foreach (var product in results)
            {
                Console.WriteLine(product);
            }
        }

        public void DisplayAllProducts()
        {
            if (products.Count == 0)
            {
                Console.WriteLine("Т товары отсутствуют");
                return;
            }

            Console.WriteLine("\n=== ВСЕ ТОВАРЫ ===");
            foreach (var product in products)
            {
                Console.WriteLine(product);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Shop shop = new Shop();
            bool running = true;

            while (running)
            {
                Console.WriteLine("\n=== СИСТЕМА УЧЕТА ТОВАРОВ ===");
                Console.WriteLine("1. Показать все товары");
                Console.WriteLine("2. Добавить товар");
                Console.WriteLine("3. Удалить товар");
                Console.WriteLine("4. Заказать поставку");
                Console.WriteLine("5. Продать товар");
                Console.WriteLine("6. Поиск товаров");
                Console.WriteLine("7. История продаж (отчет)");
                Console.WriteLine("8. Отменить последнюю продажу");
                Console.WriteLine("9. Выход");
                Console.Write("Выберите действие: ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            shop.DisplayAllProducts();
                            break;
                        case 2:
                            AddProductMenu(shop);
                            break;
                        case 3:
                            RemoveProductMenu(shop);
                            break;
                        case 4:
                            OrderSupplyMenu(shop);
                            break;
                        case 5:
                            SellProductMenu(shop);
                            break;
                        case 6:
                            SearchProductsMenu(shop);
                            break;
                        case 7:
                            shop.PrintSalesReport();
                            break;
                        case 8:
                            shop.UndoLastSale();
                            break;
                        case 9:
                            running = false;
                            break;
                        default:
                            Console.WriteLine("Неверный выбор");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Пожалуйста, введите число");
                }
            }
        }

        static void AddProductMenu(Shop shop)
        {
            Console.Write("Введите название товара: ");
            string name = Console.ReadLine();

            Console.Write("Введите цену: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price < 0)
            {
                Console.WriteLine("Неверная цена");
                return;
            }

            Console.Write("Введите количество: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity < 0)
            {
                Console.WriteLine("Неверное количество");
                return;
            }

            Console.WriteLine("Выберите категорию:");
            foreach (var category in Enum.GetValues(typeof(ProductCategory)))
            {
                Console.WriteLine($"{(int)category}. {category}");
            }

            if (int.TryParse(Console.ReadLine(), out int categoryIndex) &&
                Enum.IsDefined(typeof(ProductCategory), categoryIndex))
            {
                shop.AddProduct(name, price, quantity, (ProductCategory)categoryIndex);
            }
            else
            {
                Console.WriteLine("Неверная категория");
            }
        }

        static void RemoveProductMenu(Shop shop)
        {
            Console.Write("Введите код товара для удаления: ");
            string code = Console.ReadLine();
            shop.RemoveProduct(code);
        }

        static void OrderSupplyMenu(Shop shop)
        {
            Console.Write("Введите код товара: ");
            string code = Console.ReadLine();

            Console.Write("Введите количество для поставки: ");
            if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
            {
                shop.OrderSupply(code, quantity);
            }
            else
            {
                Console.WriteLine("Неверное количество");
            }
        }

        static void SellProductMenu(Shop shop)
        {
            Console.Write("Введите код товара: ");
            string code = Console.ReadLine();

            Console.Write("Введите количество для продажи: ");
            if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
            {
                shop.SellProduct(code, quantity);
            }
            else
            {
                Console.WriteLine("Неверное количество");
            }
        }

        static void SearchProductsMenu(Shop shop)
        {
            Console.Write("Введите код, название или категорию для поиска: ");
            string searchTerm = Console.ReadLine();
            shop.SearchProducts(searchTerm);
        }
    }
}