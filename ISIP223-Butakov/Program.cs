using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class TextAnalyzer
{
    static List<Dictionary<string, object>> allStatistics = new List<Dictionary<string, object>>();

    static void Main()
    {
        Console.WriteLine("=== Анализатор текста ===");

        while (true)
        {
            Console.WriteLine("\nВыберите действие:");
            Console.WriteLine("1 - Анализ нового текста");
            Console.WriteLine("2 - Просмотр статистики по прошлым текстам");
            Console.WriteLine("3 - Выход");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AnalyzeNewText();
                    break;
                case "2":
                    ShowPreviousStatistics();
                    break;
                case "3":
                    Console.WriteLine("До свидания!");
                    return;
                default:
                    Console.WriteLine("Неверный выбор. Попробуйте снова.");
                    break;
            }
        }
    }

    static void AnalyzeNewText()
    {
        string text = GetValidText();
        Dictionary<string, object> statistics = new Dictionary<string, object>();

        // Основной анализ текста
        AnalyzeText(text, statistics);

        // Дополнительные функции (задание со звездочкой)
        ProcessAdvancedFeatures(text, statistics);

        // Сохраняем статистику
        allStatistics.Add(statistics);

        // Выводим результаты
        DisplayStatistics(statistics);
    }

    static string GetValidText()
    {
        string text;
        do
        {
            Console.WriteLine("\nВведите текст (минимум 100 символов):");
            text = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(text) || text.Length < 100)
            {
                Console.WriteLine("Текст должен содержать минимум 100 символов. Попробуйте снова.");
            }

        } while (string.IsNullOrWhiteSpace(text) || text.Length < 100);

        return text;
    }

    static void AnalyzeText(string text, Dictionary<string, object> statistics)
    {
        // Подсчет количества слов (всех)
        int totalWords = CountAllWords(text);
        statistics["Всего слов"] = totalWords;

        // Подсчет слов без союзов и чисел (задание со звездочкой)
        int wordsWithoutConjunctions = CountWordsWithoutConjunctionsAndNumbers(text);
        statistics["Слов без союзов и чисел"] = wordsWithoutConjunctions;

        // Поиск самого короткого слова
        string shortestWord = FindShortestWord(text);
        statistics["Самое короткое слово"] = shortestWord;

        // Поиск самого длинного слова
        string longestWord = FindLongestWord(text);
        statistics["Самое длинное слово"] = longestWord;

        // Подсчет количества предложений
        int sentenceCount = CountSentences(text);
        statistics["Количество предложений"] = sentenceCount;

        // Подсчет гласных и согласных
        var (vowels, consonants) = CountVowelsAndConsonants(text);
        statistics["Гласные буквы"] = vowels;
        statistics["Согласные буквы"] = consonants;

        // Статистика по буквам
        Dictionary<char, int> letterFrequency = GetLetterFrequency(text);
        statistics["Частота букв"] = letterFrequency;

        // Сохраняем исходный текст
        statistics["Исходный текст"] = text;
    }

    static int CountAllWords(string text)
    {
        // Разделяем текст на слова, игнорируя пустые элементы
        string[] words = text.Split(new[] { ' ', '\t', '\n', '\r', '.', ',', '!', '?', ';', ':', '(', ')', '[', ']', '{', '}', '"', '\'' },
                                  StringSplitOptions.RemoveEmptyEntries);
        return words.Length;
    }

    static int CountWordsWithoutConjunctionsAndNumbers(string text)
    {
        // Список распространенных союзов (можно расширить)
        string[] conjunctions = { "и", "а", "но", "да", "или", "либо", "то", "не", "ни", "что", "чтобы", "как", "когда", "пока", "если", "хотя", "потому", "так", "же", "ведь", "вот", "мол", "дескать", "то есть", "как будто" };

        string[] words = text.Split(new[] { ' ', '\t', '\n', '\r', '.', ',', '!', '?', ';', ':', '(', ')', '[', ']', '{', '}', '"', '\'' },
                                  StringSplitOptions.RemoveEmptyEntries);

        int count = 0;

        foreach (string word in words)
        {
            string cleanWord = word.ToLower().Trim();

            // Пропускаем союзы
            if (Array.Exists(conjunctions, c => c == cleanWord))
                continue;

            // Пропускаем числа (проверяем, можно ли преобразовать в число)
            if (double.TryParse(cleanWord, out _))
                continue;

            count++;
        }

        return count;
    }

    static string FindShortestWord(string text)
    {
        string[] words = text.Split(new[] { ' ', '\t', '\n', '\r', '.', ',', '!', '?', ';', ':', '(', ')', '[', ']', '{', '}', '"', '\'' },
                                  StringSplitOptions.RemoveEmptyEntries);

        if (words.Length == 0) return "";

        string shortest = words[0];
        foreach (string word in words)
        {
            if (word.Length < shortest.Length && word.Length > 0)
            {
                shortest = word;
            }
        }

        return shortest;
    }

    static string FindLongestWord(string text)
    {
        string[] words = text.Split(new[] { ' ', '\t', '\n', '\r', '.', ',', '!', '?', ';', ':', '(', ')', '[', ']', '{', '}', '"', '\'' },
                                  StringSplitOptions.RemoveEmptyEntries);

        if (words.Length == 0) return "";

        string longest = words[0];
        foreach (string word in words)
        {
            if (word.Length > longest.Length)
            {
                longest = word;
            }
        }

        return longest;
    }

    static int CountSentences(string text)
    {
        // Простой подсчет предложений по знакам препинания
        int count = 0;
        foreach (char c in text)
        {
            if (c == '.' || c == '!' || c == '?')
            {
                count++;
            }
        }
        return count;
    }

    static (int vowels, int consonants) CountVowelsAndConsonants(string text)
    {
        // Русские гласные буквы (строчные и прописные)
        string vowels = "аеёиоуыэюяaeiouyАЕЁИОУЫЭЮЯAEIOUY";
        // Русские согласные буквы
        string consonants = "бвгджзйклмнпрстфхцчшщbcdfghjklmnpqrstvwxzБВГДЖЗЙКЛМНПРСТФХЦЧШЩBCDFGHJKLMNPQRSTVWXZ";

        int vowelCount = 0;
        int consonantCount = 0;

        foreach (char c in text)
        {
            if (char.IsLetter(c))
            {
                if (vowels.Contains(c))
                {
                    vowelCount++;
                }
                else if (consonants.Contains(c))
                {
                    consonantCount++;
                }
            }
        }

        return (vowelCount, consonantCount);
    }

    static Dictionary<char, int> GetLetterFrequency(string text)
    {
        Dictionary<char, int> frequency = new Dictionary<char, int>();

        foreach (char c in text)
        {
            if (char.IsLetter(c))
            {
                char lowerChar = char.ToLower(c);
                if (frequency.ContainsKey(lowerChar))
                {
                    frequency[lowerChar]++;
                }
                else
                {
                    frequency[lowerChar] = 1;
                }
            }
        }

        return frequency;
    }

    static void ProcessAdvancedFeatures(string originalText, Dictionary<string, object> statistics)
    {
        Console.WriteLine("\nХотите удалить определенные буквы из текста? (y/n)");
        string response = Console.ReadLine()?.ToLower();

        if (response == "y" || response == "да")
        {
            Console.WriteLine("Введите буквы для удаления (без пробелов, например: аеиоу):");
            string lettersToRemove = Console.ReadLine();

            if (!string.IsNullOrEmpty(lettersToRemove))
            {
                string modifiedText = RemoveLetters(originalText, lettersToRemove);
                Console.WriteLine($"\nТекст после удаления букв '{lettersToRemove}':");
                Console.WriteLine(modifiedText);

                // Повторный анализ модифицированного текста
                Dictionary<string, object> modifiedStats = new Dictionary<string, object>();
                AnalyzeText(modifiedText, modifiedStats);

                // Сохраняем модифицированную статистику
                statistics["Модифицированный текст"] = modifiedText;
                statistics["Статистика после удаления букв"] = modifiedStats;

                Console.WriteLine("\nСтатистика после удаления букв:");
                DisplayStatistics(modifiedStats);
            }
        }
    }

    static string RemoveLetters(string text, string lettersToRemove)
    {
        StringBuilder result = new StringBuilder();

        foreach (char c in text)
        {
            // Если символ не входит в список для удаления, добавляем его
            if (lettersToRemove.IndexOf(char.ToLower(c)) == -1 &&
                lettersToRemove.IndexOf(char.ToUpper(c)) == -1)
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }

    static void DisplayStatistics(Dictionary<string, object> statistics)
    {
        Console.WriteLine("\n=== РЕЗУЛЬТАТЫ АНАЛИЗА ===");

        foreach (var item in statistics)
        {
            if (item.Key == "Частота букв")
            {
                Console.WriteLine("\nЧастота встречаемости букв:");
                var frequency = (Dictionary<char, int>)item.Value;

                // Сортируем буквы по частоте (по убыванию)
                var sortedLetters = frequency.OrderByDescending(x => x.Value);

                foreach (var letter in sortedLetters)
                {
                    Console.WriteLine($"{letter.Key}: {letter.Value}");
                }
            }
            else if (item.Key == "Статистика после удаления букв")
            {
                // Пропускаем, так как уже выводили
            }
            else
            {
                Console.WriteLine($"{item.Key}: {item.Value}");
            }
        }
    }

    static void ShowPreviousStatistics()
    {
        if (allStatistics.Count == 0)
        {
            Console.WriteLine("Статистика по предыдущим текстам отсутствует.");
            return;
        }

        Console.WriteLine($"\n=== СТАТИСТИКА ПО ПРОШЛЫМ ТЕКСТАМ (всего: {allStatistics.Count}) ===");

        for (int i = 0; i < allStatistics.Count; i++)
        {
            Console.WriteLine($"\n--- Текст #{i + 1} ---");

            // Выводим только основные метрики для обзора
            var stats = allStatistics[i];
            Console.WriteLine($"Слов: {stats["Всего слов"]}, Предложений: {stats["Количество предложений"]}");
            Console.WriteLine($"Гласные: {stats["Гласные буквы"]}, Согласные: {stats["Согласные буквы"]}");
            Console.WriteLine($"Самое короткое слово: {stats["Самое короткое слово"]}");
            Console.WriteLine($"Самое длинное слово: {stats["Самое длинное слово"]}");

            if (stats.ContainsKey("Модифицированный текст"))
            {
                Console.WriteLine("(Включает модифицированную версию)");
            }

            Console.WriteLine("----------------------");
        }

        Console.WriteLine("\nХотите посмотреть детальную статистику по конкретному тексту? (y/n)");
        string response = Console.ReadLine()?.ToLower();

        if (response == "y" || response == "да")
        {
            Console.WriteLine($"Введите номер текста (1-{allStatistics.Count}):");
            if (int.TryParse(Console.ReadLine(), out int textNumber) && textNumber >= 1 && textNumber <= allStatistics.Count)
            {
                DisplayStatistics(allStatistics[textNumber - 1]);
            }
            else
            {
                Console.WriteLine("Неверный номер текста.");
            }
        }
    }
}