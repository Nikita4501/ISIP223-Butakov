using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagement
{
    public enum Genre
    {
        Fiction,
        Science,
        Fantasy,
        Mystery,
        Romance,
        Biography
    }

    public class Book
    {
        private static int _nextId = 1;

        public int Id { get; private set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public Genre Genre { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }

        public Book()
        {
            Id = _nextId++;
        }

        public Book(string title, string author, Genre genre, int year, decimal price) : this()
        {
            Title = title;
            Author = author;
            Genre = genre;
            Year = year;
            Price = price;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Название: {Title}, Автор: {Author}, Жанр: {Genre}, Год: {Year}, Цена: {Price:C}";
        }
    }

    public class Library
    {
        private List<Book> _books;

        public Library()
        {
            _books = new List<Book>();
            InitializeTestData();
        }

        private void InitializeTestData()
        {
            _books.AddRange(new[]
            {
                new Book("Война и мир", "Лев Толстой", Genre.Fiction, 1869, 1200m),
                new Book("1984", "Джордж Оруэлл", Genre.Fiction, 1949, 850m),
                new Book("Краткая история времени", "Стивен Хокинг", Genre.Science, 1988, 950m),
                new Book("Гарри Поттер и философский камень", "Джоан Роулинг", Genre.Fantasy, 1997, 700m),
                new Book("Убийство в Восточном экспрессе", "Агата Кристи", Genre.Mystery, 1934, 600m)
            });
        }

        public static bool ValidateBook(string title, string author, int year, decimal price, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(title))
            {
                errorMessage = "Название книги не может быть пустым.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(author))
            {
                errorMessage = "Автор не может быть пустым.";
                return false;
            }

            if (year < 1000 || year > DateTime.Now.Year)
            {
                errorMessage = $"Год издания должен быть между 1000 и {DateTime.Now.Year}.";
                return false;
            }

            if (price < 0)
            {
                errorMessage = "Цена не может быть отрицательной.";
                return false;
            }

            return true;
        }

        public void AddBook(Book book)
        {
            _books.Add(book);
        }

        public bool RemoveBook(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                _books.Remove(book);
                return true;
            }
            return false;
        }

        public List<Book> FindBooksByTitle(string title)
        {
            return _books.Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<Book> FindBooksByAuthor(string author)
        {
            return _books.Where(b => b.Author.Contains(author, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<Book> FindBooksByGenre(Genre genre)
        {
            return _books.Where(b => b.Genre == genre).ToList();
        }

        public List<Book> FindBooks(string searchTerm)
        {
            return _books.Where(b =>
                b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                b.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                b.Genre.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Book> SortBooksByTitle()
        {
            return _books.OrderBy(b => b.Title).ToList();
        }

        public List<Book> SortBooksByYear()
        {
            return _books.OrderBy(b => b.Year).ToList();
        }

        public Book GetMostExpensiveBook()
        {
            return _books.OrderByDescending(b => b.Price).FirstOrDefault();
        }

        public Book GetCheapestBook()
        {
            return _books.OrderBy(b => b.Price).FirstOrDefault();
        }

        public Dictionary<string, int> GetBooksCountByAuthor()
        {
            return _books.GroupBy(b => b.Author)
                        .ToDictionary(g => g.Key, g => g.Count());
        }

        public List<Book> GetAllBooks()
        {
            return new List<Book>(_books);
        }

        public bool BookExists(int id)
        {
            return _books.Any(b => b.Id == id);
        }
    }

    class Program
    {
        private static Library _library = new Library();

        static void Main(string[] args)
        {
            Console.WriteLine("=== Система управления библиотекой ===");

            while (true)
            {
                ShowMenu();
                var choice = GetUserChoice();
                ProcessChoice(choice);
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nВыберите действие:");
            Console.WriteLine("1. Добавить книгу");
            Console.WriteLine("2. Удалить книгу по ID");
            Console.WriteLine("3. Найти книги");
            Console.WriteLine("4. Отсортировать книги");
            Console.WriteLine("5. Самая дорогая/дешёвая книга");
            Console.WriteLine("6. Статистика по авторам");
            Console.WriteLine("7. Показать все книги");
            Console.WriteLine("0. Выход");
            Console.Write("Ваш выбор: ");
        }

        static int GetUserChoice()
        {
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                return choice;
            }
            return -1;
        }

        static void ProcessChoice(int choice)
        {
            HandleException(() =>
            {
                switch (choice)
                {
                    case 1: AddBook(); break;
                    case 2: RemoveBook(); break;
                    case 3: FindBooks(); break;
                    case 4: SortBooks(); break;
                    case 5: ShowPriceExtremes(); break;
                    case 6: ShowAuthorStatistics(); break;
                    case 7: ShowAllBooks(); break;
                    case 0:
                        Console.WriteLine("До свидания!");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Неверный выбор! Пожалуйста, выберите действие из меню.");
                        break;
                }
            });
        }

        static void HandleException(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }

        static void AddBook()
        {
            Console.WriteLine("\n--- Добавление новой книги ---");

            Console.Write("Введите название: ");
            string title = Console.ReadLine();

            Console.Write("Введите автора: ");
            string author = Console.ReadLine();

            Console.WriteLine("Доступные жанры:");
            foreach (var genre in Enum.GetValues(typeof(Genre)))
            {
                Console.WriteLine($"{(int)genre}. {genre}");
            }

            Console.Write("Выберите жанр (номер): ");
            if (!int.TryParse(Console.ReadLine(), out int genreIndex) || !Enum.IsDefined(typeof(Genre), genreIndex))
            {
                Console.WriteLine("Неверный выбор жанра!");
                return;
            }

            Console.Write("Введите год издания: ");
            if (!int.TryParse(Console.ReadLine(), out int year))
            {
                Console.WriteLine("Неверный формат года!");
                return;
            }

            Console.Write("Введите цену: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.WriteLine("Неверный формат цены!");
                return;
            }

            if (Library.ValidateBook(title, author, year, price, out string errorMessage))
            {
                var book = new Book(title, author, (Genre)genreIndex, year, price);
                _library.AddBook(book);
                Console.WriteLine("Книга успешно добавлена!");
                Console.WriteLine(book);
            }
            else
            {
                Console.WriteLine($"Ошибка: {errorMessage}");
            }
        }

        static void RemoveBook()
        {
            Console.WriteLine("\n--- Удаление книги ---");
            Console.Write("Введите ID книги для удаления: ");

            if (int.TryParse(Console.ReadLine(), out int id))
            {
                if (_library.RemoveBook(id))
                {
                    Console.WriteLine("Книга успешно удалена!");
                }
                else
                {
                    Console.WriteLine("Книга с указанным ID не найдена!");
                }
            }
            else
            {
                Console.WriteLine("Неверный формат ID!");
            }
        }

        static void FindBooks()
        {
            Console.WriteLine("\n--- Поиск книг ---");
            Console.WriteLine("1. По названию");
            Console.WriteLine("2. По автору");
            Console.WriteLine("3. По жанру");
            Console.WriteLine("4. Общий поиск");
            Console.Write("Выберите тип поиска: ");

            if (int.TryParse(Console.ReadLine(), out int searchType))
            {
                List<Book> results = new List<Book>();

                switch (searchType)
                {
                    case 1:
                        Console.Write("Введите название для поиска: ");
                        string title = Console.ReadLine();
                        results = _library.FindBooksByTitle(title);
                        break;
                    case 2:
                        Console.Write("Введите автора для поиска: ");
                        string author = Console.ReadLine();
                        results = _library.FindBooksByAuthor(author);
                        break;
                    case 3:
                        Console.WriteLine("Доступные жанры:");
                        foreach (var genre in Enum.GetValues(typeof(Genre)))
                        {
                            Console.WriteLine($"{(int)genre}. {genre}");
                        }
                        Console.Write("Выберите жанр: ");
                        if (int.TryParse(Console.ReadLine(), out int genreIndex) && Enum.IsDefined(typeof(Genre), genreIndex))
                        {
                            results = _library.FindBooksByGenre((Genre)genreIndex);
                        }
                        else
                        {
                            Console.WriteLine("Неверный выбор жанра!");
                            return;
                        }
                        break;
                    case 4:
                        Console.Write("Введите текст для поиска: ");
                        string searchTerm = Console.ReadLine();
                        results = _library.FindBooks(searchTerm);
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        return;
                }

                if (results.Any())
                {
                    Console.WriteLine($"Найдено {results.Count} книг:");
                    foreach (var book in results)
                    {
                        Console.WriteLine(book);
                    }
                }
                else
                {
                    Console.WriteLine("Книги не найдены!");
                }
            }
            else
            {
                Console.WriteLine("Неверный формат выбора!");
            }
        }

        static void SortBooks()
        {
            Console.WriteLine("\n--- Сортировка книг ---");
            Console.WriteLine("1. По названию");
            Console.WriteLine("2. По году издания");
            Console.Write("Выберите тип сортировки: ");

            if (int.TryParse(Console.ReadLine(), out int sortType))
            {
                List<Book> sortedBooks = new List<Book>();

                switch (sortType)
                {
                    case 1:
                        sortedBooks = _library.SortBooksByTitle();
                        Console.WriteLine("Книги отсортированы по названию:");
                        break;
                    case 2:
                        sortedBooks = _library.SortBooksByYear();
                        Console.WriteLine("Книги отсортированы по году издания:");
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        return;
                }

                foreach (var book in sortedBooks)
                {
                    Console.WriteLine(book);
                }
            }
            else
            {
                Console.WriteLine("Неверный формат выбора!");
            }
        }

        static void ShowPriceExtremes()
        {
            Console.WriteLine("\n--- Самая дорогая и дешёвая книги ---");

            var mostExpensive = _library.GetMostExpensiveBook();
            var cheapest = _library.GetCheapestBook();

            if (mostExpensive != null && cheapest != null)
            {
                Console.WriteLine($"Самая дорогая книга: {mostExpensive}");
                Console.WriteLine($"Самая дешёвая книга: {cheapest}");
            }
            else
            {
                Console.WriteLine("В библиотеке нет книг!");
            }
        }

        static void ShowAuthorStatistics()
        {
            Console.WriteLine("\n--- Статистика по авторам ---");

            var stats = _library.GetBooksCountByAuthor();

            if (stats.Any())
            {
                foreach (var stat in stats)
                {
                    Console.WriteLine($"{stat.Key}: {stat.Value} книг(и)");
                }
            }
            else
            {
                Console.WriteLine("В библиотеке нет книг!");
            }
        }

        static void ShowAllBooks()
        {
            Console.WriteLine("\n--- Все книги в библиотеке ---");

            var books = _library.GetAllBooks();

            if (books.Any())
            {
                foreach (var book in books)
                {
                    Console.WriteLine(book);
                }
                Console.WriteLine($"\nВсего книг: {books.Count}");
            }
            else
            {
                Console.WriteLine("В библиотеке нет книг!");
            }
        }
    }
}