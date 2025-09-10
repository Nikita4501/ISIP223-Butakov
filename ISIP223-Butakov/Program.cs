using System;

class Program
{
    static void Main()
    {
        Console.Write("Сколько операций записать? (2-40): ");
        int count = int.Parse(Console.ReadLine());

        if (count < 2 || count > 40)
        {
            Console.WriteLine("Ошибка! Должно быть от 2 до 40 операций");
            return;
        }

        string[] names = new string[count];
        double[] prices = new double[count];

        Console.WriteLine("\nВводите траты в формате: Название; Сумма");
        for (int i = 0; i < count; i++)
        {
            Console.Write($"Трата {i + 1}: ");
            string input = Console.ReadLine();

            string[] parts = input.Split(';');

            if (parts.Length == 2)
            {
                names[i] = parts[0].Trim();
                prices[i] = double.Parse(parts[1].Trim());
            }
            else
            {
                Console.WriteLine("Ошибка формата! Используйте: Название; Сумма");
                i--;
            }
        }

        while (true)
        {
            Console.WriteLine("\n=== МЕНЮ ===");
            Console.WriteLine("1 - Показать все траты");
            Console.WriteLine("2 - Статистика");
            Console.WriteLine("3 - Сортировка по цене");
            Console.WriteLine("4 - Конвертация валюты");
            Console.WriteLine("5 - Поиск по названию");
            Console.WriteLine("0 - Выход");

            Console.Write("Выберите: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ShowAll(names, prices);
                    break;
                case "2":
                    ShowStats(prices);
                    break;
                case "3":
                    SortPrices(names, prices);
                    break;
                case "4":
                    ConvertCurrency(prices);
                    break;
                case "5":
                    SearchName(names, prices);
                    break;
                case "0":
                    Console.WriteLine("До свидания!");
                    return;
                default:
                    Console.WriteLine("Неверный выбор!");
                    break;
            }
        }
    }

    static void ShowAll(string[] names, double[] prices)
    {
        Console.WriteLine("\n=== ВСЕ ТРАТЫ ===");
        for (int i = 0; i < names.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {names[i]} - {prices[i]} руб.");
        }
    }

    static void ShowStats(double[] prices)
    {
        double sum = 0;
        double max = prices[0];
        double min = prices[0];

        foreach (double price in prices)
        {
            sum += price;
            if (price > max) max = price;
            if (price < min) min = price;
        }

        double average = sum / prices.Length;

        Console.WriteLine("\n=== СТАТИСТИКА ===");
        Console.WriteLine($"Общая сумма: {sum} руб.");
        Console.WriteLine($"Средняя трата: {average:F2} руб.");
        Console.WriteLine($"Самая большая: {max} руб.");
        Console.WriteLine($"Самая маленькая: {min} руб.");
    }

    static void SortPrices(string[] names, double[] prices)
    {
        for (int i = 0; i < prices.Length - 1; i++)
        {
            for (int j = 0; j < prices.Length - 1; j++)
            {
                if (prices[j] > prices[j + 1])
                {
                    double tempPrice = prices[j];
                    prices[j] = prices[j + 1];
                    prices[j + 1] = tempPrice;

                    string tempName = names[j];
                    names[j] = names[j + 1];
                    names[j + 1] = tempName;
                }
            }
        }
        Console.WriteLine("Отсортировано по цене!");
    }

    static void ConvertCurrency(double[] prices)
    {
        Console.Write("Введите курс (1 рубль = X валюты): ");
        double rate = double.Parse(Console.ReadLine());

        Console.WriteLine("\n=== КОНВЕРТАЦИЯ ===");
        foreach (double price in prices)
        {
            double converted = price * rate;
            Console.WriteLine($"{price} руб. = {converted:F2} у.е.");
        }
    }

    static void SearchName(string[] names, double[] prices)
    {
        Console.Write("Введите название для поиска: ");
        string search = Console.ReadLine().ToLower();

        Console.WriteLine("\n=== РЕЗУЛЬТАТЫ ===");
        bool found = false;

        for (int i = 0; i < names.Length; i++)
        {
            if (names[i].ToLower().Contains(search))
            {
                Console.WriteLine($"{names[i]} - {prices[i]} руб.");
                found = true;
            }
        }

        if (!found) Console.WriteLine("Ничего не найдено!");
    }
}