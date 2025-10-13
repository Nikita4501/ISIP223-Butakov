using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversityManagementSystem
{
    // Абстрактный базовый класс для всех людей в университете
    public abstract class Person
    {
        private string _name;
        private int _age;
        private string _email;
        private string _phone;
        private string _id;

        protected Person(string name, int age, string email, string phone)
        {
            _name = name;
            _age = age;
            _email = email;
            _phone = phone;
            _id = GenerateId();
        }

        private string GenerateId()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }

        public abstract string GetInfo();

        // Свойства с инкапсуляцией
        public string Name => _name;
        public int Age => _age;
        public string Email => _email;
        public string Phone => _phone;
        public string Id => _id;
    }

    // Класс Студент
    public class Student : Person
    {
        private string _major;
        private Dictionary<string, double> _courses; // курс: оценка
        private double _gpa;

        public Student(string name, int age, string email, string phone, string major)
            : base(name, age, email, phone)
        {
            _major = major;
            _courses = new Dictionary<string, double>();
            _gpa = 0.0;
        }

        public override string GetInfo()
        {
            return $"Студент: {Name} ({Id})\n" +
                   $"Возраст: {Age}, Специальность: {_major}\n" +
                   $"Email: {Email}, Телефон: {Phone}\n" +
                   $"Средний балл: {_gpa:F2}";
        }

        public bool EnrollInCourse(string courseCode)
        {
            if (!_courses.ContainsKey(courseCode))
            {
                _courses[courseCode] = 0.0; // 0.0 означает отсутствие оценки
                return true;
            }
            return false;
        }

        public bool AddGrade(string courseCode, double grade)
        {
            if (_courses.ContainsKey(courseCode))
            {
                _courses[courseCode] = grade;
                CalculateGpa();
                return true;
            }
            return false;
        }

        private void CalculateGpa()
        {
            var gradedCourses = _courses.Values.Where(grade => grade > 0).ToList();
            if (gradedCourses.Count > 0)
            {
                _gpa = gradedCourses.Average();
            }
            else
            {
                _gpa = 0.0;
            }
        }

        public List<string> GetCourses()
        {
            return _courses.Keys.ToList();
        }

        public Dictionary<string, double> GetGrades()
        {
            return new Dictionary<string, double>(_courses);
        }

        public string Major => _major;
        public double Gpa => _gpa;
    }

    // Класс Преподаватель
    public class Professor : Person
    {
        private string _department;
        private List<string> _courses;

        public Professor(string name, int age, string email, string phone, string department)
            : base(name, age, email, phone)
        {
            _department = department;
            _courses = new List<string>();
        }

        public override string GetInfo()
        {
            string coursesInfo = _courses.Count > 0 ? string.Join(", ", _courses) : "нет";
            return $"Преподаватель: {Name} ({Id})\n" +
                   $"Возраст: {Age}, Кафедра: {_department}\n" +
                   $"Email: {Email}, Телефон: {Phone}\n" +
                   $"Курсы: {coursesInfo}";
        }

        public void AssignToCourse(string courseCode)
        {
            if (!_courses.Contains(courseCode))
            {
                _courses.Add(courseCode);
            }
        }

        public void RemoveFromCourse(string courseCode)
        {
            _courses.Remove(courseCode);
        }

        public List<string> GetCourses()
        {
            return new List<string>(_courses);
        }

        public string Department => _department;
    }

    // Класс Курс
    public class Course
    {
        private string _code;
        private string _name;
        private string _description;
        private Professor _professor;
        private List<Student> _students;
        private int _maxStudents;

        public Course(string code, string name, string description, int maxStudents = 30)
        {
            _code = code;
            _name = name;
            _description = description;
            _students = new List<Student>();
            _maxStudents = maxStudents;
        }

        public bool AddStudent(Student student)
        {
            if (_students.Count < _maxStudents && !_students.Contains(student))
            {
                _students.Add(student);
                student.EnrollInCourse(_code);
                return true;
            }
            return false;
        }

        public bool RemoveStudent(Student student)
        {
            return _students.Remove(student);
        }

        public void AssignProfessor(Professor professor)
        {
            _professor = professor;
            professor.AssignToCourse(_code);
        }

        public string GetInfo()
        {
            string professorInfo = _professor != null ? _professor.Name : "не назначен";
            return $"Курс: {_name} ({_code})\n" +
                   $"Описание: {_description}\n" +
                   $"Преподаватель: {professorInfo}\n" +
                   $"Студентов: {_students.Count}/{_maxStudents}";
        }

        public string GetStudentsInfo()
        {
            if (_students.Count == 0)
            {
                return "На курсе нет студентов";
            }

            string result = $"Студенты курса {_name}:\n";
            for (int i = 0; i < _students.Count; i++)
            {
                result += $"{i + 1}. {_students[i].Name} ({_students[i].Id})\n";
            }
            return result;
        }

        public string Code => _code;
        public string Name => _name;
        public Professor Professor => _professor;
        public List<Student> Students => new List<Student>(_students);
        public int MaxStudents => _maxStudents;
        public int AvailableSlots => _maxStudents - _students.Count;
    }

    // Основная система управления университетом
    public class UniversitySystem
    {
        private Dictionary<string, Student> _students;
        private Dictionary<string, Professor> _professors;
        private Dictionary<string, Course> _courses;

        public UniversitySystem()
        {
            _students = new Dictionary<string, Student>();
            _professors = new Dictionary<string, Professor>();
            _courses = new Dictionary<string, Course>();
        }

        // Методы для работы со студентами
        public string AddStudent(string name, int age, string email, string phone, string major)
        {
            var student = new Student(name, age, email, phone, major);
            _students[student.Id] = student;
            return student.Id;
        }

        public Student GetStudent(string studentId)
        {
            return _students.ContainsKey(studentId) ? _students[studentId] : null;
        }

        public List<Student> GetAllStudents()
        {
            return _students.Values.ToList();
        }

        // Методы для работы с преподавателями
        public string AddProfessor(string name, int age, string email, string phone, string department)
        {
            var professor = new Professor(name, age, email, phone, department);
            _professors[professor.Id] = professor;
            return professor.Id;
        }

        public Professor GetProfessor(string professorId)
        {
            return _professors.ContainsKey(professorId) ? _professors[professorId] : null;
        }

        public List<Professor> GetAllProfessors()
        {
            return _professors.Values.ToList();
        }

        // Методы для работы с курсами
        public bool AddCourse(string code, string name, string description, int maxStudents = 30)
        {
            if (!_courses.ContainsKey(code))
            {
                _courses[code] = new Course(code, name, description, maxStudents);
                return true;
            }
            return false;
        }

        public Course GetCourse(string courseCode)
        {
            return _courses.ContainsKey(courseCode) ? _courses[courseCode] : null;
        }

        public List<Course> GetAllCourses()
        {
            return _courses.Values.ToList();
        }

        // Методы для связывания сущностей
        public bool EnrollStudentInCourse(string studentId, string courseCode)
        {
            var student = GetStudent(studentId);
            var course = GetCourse(courseCode);

            if (student != null && course != null)
            {
                return course.AddStudent(student);
            }
            return false;
        }

        public bool AssignProfessorToCourse(string professorId, string courseCode)
        {
            var professor = GetProfessor(professorId);
            var course = GetCourse(courseCode);

            if (professor != null && course != null)
            {
                course.AssignProfessor(professor);
                return true;
            }
            return false;
        }

        public bool AddGradeToStudent(string studentId, string courseCode, double grade)
        {
            var student = GetStudent(studentId);
            if (student != null)
            {
                return student.AddGrade(courseCode, grade);
            }
            return false;
        }
    }

    // Консольный интерфейс
    public class UniversityConsole
    {
        private UniversitySystem _system;

        public UniversityConsole()
        {
            _system = new UniversitySystem();
            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            // Добавляем примеры данных для демонстрации
            _system.AddCourse("CS101", "Введение в программирование", "Основы программирования на C#", 25);
            _system.AddCourse("MATH201", "Высшая математика", "Математический анализ и линейная алгебра", 30);
            _system.AddCourse("ENG101", "Английский язык", "Академический английский", 20);

            _system.AddProfessor("Иван Петров", 45, "i.petrov@university.ru", "+79161234567", "Компьютерные науки");
            _system.AddProfessor("Мария Сидорова", 38, "m.sidorova@university.ru", "+79167654321", "Математика");
            _system.AddProfessor("Анна Козлова", 42, "a.kozlova@university.ru", "+79169998877", "Иностранные языки");

            var professors = _system.GetAllProfessors();
            if (professors.Count >= 3)
            {
                _system.AssignProfessorToCourse(professors[0].Id, "CS101");
                _system.AssignProfessorToCourse(professors[1].Id, "MATH201");
                _system.AssignProfessorToCourse(professors[2].Id, "ENG101");
            }
        }

        public void DisplayMenu()
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("СИСТЕМА УПРАВЛЕНИЯ УНИВЕРСИТЕТОМ");
            Console.WriteLine(new string('=', 50));
            Console.WriteLine("1. Управление студентами");
            Console.WriteLine("2. Управление преподавателями");
            Console.WriteLine("3. Управление курсами");
            Console.WriteLine("4. Просмотр всех данных");
            Console.WriteLine("5. Запись студента на курс");
            Console.WriteLine("6. Назначение преподавателя на курс");
            Console.WriteLine("7. Выставление оценок");
            Console.WriteLine("8. Выход");
            Console.WriteLine(new string('=', 50));
        }

        public void Run()
        {
            while (true)
            {
                DisplayMenu();
                Console.Write("Выберите действие: ");
                string choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        ManageStudents();
                        break;
                    case "2":
                        ManageProfessors();
                        break;
                    case "3":
                        ManageCourses();
                        break;
                    case "4":
                        ViewAllData();
                        break;
                    case "5":
                        EnrollStudent();
                        break;
                    case "6":
                        AssignProfessor();
                        break;
                    case "7":
                        AddGrade();
                        break;
                    case "8":
                        Console.WriteLine("Выход из системы...");
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }

        private void ManageStudents()
        {
            while (true)
            {
                Console.WriteLine("\n--- Управление студентами ---");
                Console.WriteLine("1. Добавить студента");
                Console.WriteLine("2. Просмотреть информацию о студенте");
                Console.WriteLine("3. Список всех студентов");
                Console.WriteLine("4. Назад");

                Console.Write("Выберите действие: ");
                string choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        AddStudent();
                        break;
                    case "2":
                        ViewStudent();
                        break;
                    case "3":
                        ListAllStudents();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        break;
                }
            }
        }

        private void AddStudent()
        {
            try
            {
                Console.WriteLine("\n--- Добавление нового студента ---");
                Console.Write("Имя: ");
                string name = Console.ReadLine();
                Console.Write("Возраст: ");
                int age = int.Parse(Console.ReadLine());
                Console.Write("Email: ");
                string email = Console.ReadLine();
                Console.Write("Телефон: ");
                string phone = Console.ReadLine();
                Console.Write("Специальность: ");
                string major = Console.ReadLine();

                string studentId = _system.AddStudent(name, age, email, phone, major);
                Console.WriteLine($"Студент добавлен. ID: {studentId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении студента: {ex.Message}");
            }
        }

        private void ViewStudent()
        {
            Console.Write("Введите ID студента: ");
            string studentId = Console.ReadLine();
            var student = _system.GetStudent(studentId);

            if (student != null)
            {
                Console.WriteLine("\n" + student.GetInfo());
                var courses = student.GetCourses();
                if (courses.Count > 0)
                {
                    Console.WriteLine("\nКурсы студента:");
                    foreach (string courseCode in courses)
                    {
                        var course = _system.GetCourse(courseCode);
                        if (course != null)
                        {
                            var grades = student.GetGrades();
                            double grade = grades.ContainsKey(courseCode) ? grades[courseCode] : 0.0;
                            string gradeInfo = grade > 0 ? $" - Оценка: {grade}" : " - Оценка не выставлена";
                            Console.WriteLine($"  {course.Name} ({courseCode}){gradeInfo}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Студент не записан на курсы");
                }
            }
            else
            {
                Console.WriteLine("Студент не найден");
            }
        }

        private void ListAllStudents()
        {
            var students = _system.GetAllStudents();
            if (students.Count == 0)
            {
                Console.WriteLine("Студентов нет в системе");
                return;
            }

            Console.WriteLine("\n--- Все студенты ---");
            foreach (var student in students)
            {
                Console.WriteLine($"{student.Name} (ID: {student.Id}, GPA: {student.Gpa:F2})");
            }
        }

        private void ManageProfessors()
        {
            while (true)
            {
                Console.WriteLine("\n--- Управление преподавателями ---");
                Console.WriteLine("1. Добавить преподавателя");
                Console.WriteLine("2. Просмотреть информацию о преподавателе");
                Console.WriteLine("3. Список всех преподавателей");
                Console.WriteLine("4. Назад");

                Console.Write("Выберите действие: ");
                string choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        AddProfessor();
                        break;
                    case "2":
                        ViewProfessor();
                        break;
                    case "3":
                        ListAllProfessors();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        break;
                }
            }
        }

        private void AddProfessor()
        {
            try
            {
                Console.WriteLine("\n--- Добавление нового преподавателя ---");
                Console.Write("Имя: ");
                string name = Console.ReadLine();
                Console.Write("Возраст: ");
                int age = int.Parse(Console.ReadLine());
                Console.Write("Email: ");
                string email = Console.ReadLine();
                Console.Write("Телефон: ");
                string phone = Console.ReadLine();
                Console.Write("Кафедра: ");
                string department = Console.ReadLine();

                string professorId = _system.AddProfessor(name, age, email, phone, department);
                Console.WriteLine($"Преподаватель добавлен. ID: {professorId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении преподавателя: {ex.Message}");
            }
        }

        private void ViewProfessor()
        {
            Console.Write("Введите ID преподавателя: ");
            string professorId = Console.ReadLine();
            var professor = _system.GetProfessor(professorId);

            if (professor != null)
            {
                Console.WriteLine("\n" + professor.GetInfo());
            }
            else
            {
                Console.WriteLine("Преподаватель не найден");
            }
        }

        private void ListAllProfessors()
        {
            var professors = _system.GetAllProfessors();
            if (professors.Count == 0)
            {
                Console.WriteLine("Преподавателей нет в системе");
                return;
            }

            Console.WriteLine("\n--- Все преподаватели ---");
            foreach (var professor in professors)
            {
                Console.WriteLine($"{professor.Name} (ID: {professor.Id}, Кафедра: {professor.Department})");
            }
        }

        private void ManageCourses()
        {
            while (true)
            {
                Console.WriteLine("\n--- Управление курсами ---");
                Console.WriteLine("1. Добавить курс");
                Console.WriteLine("2. Просмотреть информацию о курсе");
                Console.WriteLine("3. Список всех курсов");
                Console.WriteLine("4. Список студентов на курсе");
                Console.WriteLine("5. Назад");

                Console.Write("Выберите действие: ");
                string choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        AddCourse();
                        break;
                    case "2":
                        ViewCourse();
                        break;
                    case "3":
                        ListAllCourses();
                        break;
                    case "4":
                        ViewCourseStudents();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        break;
                }
            }
        }

        private void AddCourse()
        {
            try
            {
                Console.WriteLine("\n--- Добавление нового курса ---");
                Console.Write("Код курса: ");
                string code = Console.ReadLine();
                Console.Write("Название курса: ");
                string name = Console.ReadLine();
                Console.Write("Описание курса: ");
                string description = Console.ReadLine();
                Console.Write("Максимальное количество студентов: ");
                int maxStudents = int.Parse(Console.ReadLine());

                if (_system.AddCourse(code, name, description, maxStudents))
                {
                    Console.WriteLine("Курс добавлен");
                }
                else
                {
                    Console.WriteLine("Курс с таким кодом уже существует");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении курса: {ex.Message}");
            }
        }

        private void ViewCourse()
        {
            Console.Write("Введите код курса: ");
            string courseCode = Console.ReadLine();
            var course = _system.GetCourse(courseCode);

            if (course != null)
            {
                Console.WriteLine("\n" + course.GetInfo());
            }
            else
            {
                Console.WriteLine("Курс не найден");
            }
        }

        private void ListAllCourses()
        {
            var courses = _system.GetAllCourses();
            if (courses.Count == 0)
            {
                Console.WriteLine("Курсов нет в системе");
                return;
            }

            Console.WriteLine("\n--- Все курсы ---");
            foreach (var course in courses)
            {
                string professorName = course.Professor != null ? course.Professor.Name : "не назначен";
                Console.WriteLine($"{course.Name} ({course.Code}) - Преподаватель: {professorName}");
            }
        }

        private void ViewCourseStudents()
        {
            Console.Write("Введите код курса: ");
            string courseCode = Console.ReadLine();
            var course = _system.GetCourse(courseCode);

            if (course != null)
            {
                Console.WriteLine("\n" + course.GetStudentsInfo());
            }
            else
            {
                Console.WriteLine("Курс не найден");
            }
        }

        private void ViewAllData()
        {
            Console.WriteLine("\n--- Все данные университета ---");

            Console.WriteLine("\nСТУДЕНТЫ:");
            var students = _system.GetAllStudents();
            foreach (var student in students)
            {
                Console.WriteLine($"  {student.Name} (ID: {student.Id}, GPA: {student.Gpa:F2})");
            }

            Console.WriteLine("\nПРЕПОДАВАТЕЛИ:");
            var professors = _system.GetAllProfessors();
            foreach (var professor in professors)
            {
                Console.WriteLine($"  {professor.Name} (ID: {professor.Id})");
            }

            Console.WriteLine("\nКУРСЫ:");
            var courses = _system.GetAllCourses();
            foreach (var course in courses)
            {
                string professorName = course.Professor != null ? course.Professor.Name : "не назначен";
                Console.WriteLine($"  {course.Name} ({course.Code}) - {professorName}");
            }
        }

        private void EnrollStudent()
        {
            try
            {
                Console.WriteLine("\n--- Запись студента на курс ---");
                Console.Write("ID студента: ");
                string studentId = Console.ReadLine();
                Console.Write("Код курса: ");
                string courseCode = Console.ReadLine();

                if (_system.EnrollStudentInCourse(studentId, courseCode))
                {
                    Console.WriteLine("Студент успешно записан на курс");
                }
                else
                {
                    Console.WriteLine("Ошибка: студент или курс не найден, либо курс переполнен");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи на курс: {ex.Message}");
            }
        }

        private void AssignProfessor()
        {
            try
            {
                Console.WriteLine("\n--- Назначение преподавателя на курс ---");
                Console.Write("ID преподавателя: ");
                string professorId = Console.ReadLine();
                Console.Write("Код курса: ");
                string courseCode = Console.ReadLine();

                if (_system.AssignProfessorToCourse(professorId, courseCode))
                {
                    Console.WriteLine("Преподаватель назначен на курс");
                }
                else
                {
                    Console.WriteLine("Ошибка: преподаватель или курс не найден");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при назначении преподавателя: {ex.Message}");
            }
        }

        private void AddGrade()
        {
            try
            {
                Console.WriteLine("\n--- Выставление оценки ---");
                Console.Write("ID студента: ");
                string studentId = Console.ReadLine();
                Console.Write("Код курса: ");
                string courseCode = Console.ReadLine();
                Console.Write("Оценка (0-100): ");
                double grade = double.Parse(Console.ReadLine());

                if (_system.AddGradeToStudent(studentId, courseCode, grade))
                {
                    Console.WriteLine("Оценка выставлена");
                }
                else
                {
                    Console.WriteLine("Ошибка: студент не найден или не записан на этот курс");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выставлении оценки: {ex.Message}");
            }
        }
    }

    // Главный класс программы
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            UniversityConsole console = new UniversityConsole();
            console.Run();
        }
    }
}