using ADONET_Final_project.Data;
using ADONET_Final_project.Models;

namespace ADONET_Final_project.UI;

public class ConsoleUI
{
    private readonly UserRepository _userRepo;
    private readonly GroupRepository _groupRepo;
    private readonly AddressRepository _addressRepo;

    public ConsoleUI(UserRepository userRepo, GroupRepository groupRepo, AddressRepository addressRepo)
    {
        _userRepo = userRepo;
        _groupRepo = groupRepo;
        _addressRepo = addressRepo;
    }

    public void ShowMenu()
    {
        bool exit = false;

        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("1. Все пользователи");
            Console.WriteLine("2. Администраторы");
            Console.WriteLine("3. Модераторы");
            Console.WriteLine("4. Обычные пользователи");
            Console.WriteLine("5. Добавить пользователя");
            Console.WriteLine("6. Редактировать пользователя");
            Console.WriteLine("7. Удалить пользователя");
            Console.WriteLine("8. Выход");
            Console.Write("\nВыберите опцию: ");

            string choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1": DisplayUsers(_userRepo.GetAll()); break;
                case "2": DisplayUsers(_userRepo.GetByGroupFilter("g.name LIKE '%admin%' OR g.name LIKE '%админ%'")); break;
                case "3": DisplayUsers(_userRepo.GetByGroupFilter("g.name LIKE '%moder%' OR g.name LIKE '%модер%'")); break;
                case "4": DisplayUsers(_userRepo.GetByGroupFilter("(g.name NOT LIKE '%admin%' AND g.name NOT LIKE '%админ%' AND g.name NOT LIKE '%moder%' AND g.name NOT LIKE '%модер%') OR g.name IS NULL")); break;
                case "5": AddUser(); break;
                case "6": EditUser(); break;
                case "7": DeleteUser(); break;
                case "8": exit = true; break;
                default:
                    Console.WriteLine("Неверный выбор!");
                    WaitForKey();
                    break;
            }
        }

        Console.WriteLine("Программа завершена.");
    }

    private void DisplayUsers(List<User> users)
    {
        Console.Clear();
        Console.WriteLine("Результаты:");
        Console.WriteLine(new string('=', 130));

        if (users.Count == 0)
        {
            Console.WriteLine("Пользователи не найдены.");
        }
        else
        {
            Console.WriteLine($"{"Login",-15} {"Name",-15} {"Surname",-15} {"Group",-20} {"Address",-50}");
            Console.WriteLine(new string('-', 130));

            foreach (var user in users)
            {
                string groupName = string.IsNullOrEmpty(user.GroupName) ? "—" : user.GroupName;
                string address = FormatAddress(user);

                Console.WriteLine($"{user.Login,-15} {user.Name,-15} {user.Surname,-15} {groupName,-20} {address,-50}");
            }

            Console.WriteLine(new string('-', 130));
            Console.WriteLine($"Total: {users.Count}");
        }

        WaitForKey();
    }

    private string FormatAddress(User user)
    {
        if (string.IsNullOrEmpty(user.City))
            return "—";

        string address = $"{user.City}, {user.Street}, д. {user.House}";
        if (!string.IsNullOrEmpty(user.Apartment))
            address += $", кв. {user.Apartment}";

        return address;
    }

    private void AddUser()
    {
        Console.Clear();
        Console.WriteLine("=== ДОБАВЛЕНИЕ ПОЛЬЗОВАТЕЛЯ ===");

        var user = new User();

        user.Login = ReadRequired("Логин");
        user.Name = ReadOptional("Имя");
        user.Surname = ReadOptional("Фамилия");
        user.Patronomic = ReadOptional("Отчество");
        user.Password = ReadRequired("Пароль");
        user.BirthDate = ReadDate("Дата рождения (ГГГГ-ММ-ДД)");

        string groupName = ReadOptional("Название группы");
        user.GroupId = _groupRepo.GetOrCreateId(groupName);

        Console.WriteLine("--- Адрес ---");
        string city = ReadOptional("Город");
        string street = ReadOptional("Улица");
        string house = ReadOptional("Дом");
        string apartment = ReadOptional("Квартира");
        user.AddressId = _addressRepo.GetOrCreateId(city, street, house, apartment);

        try
        {
            _userRepo.Add(user);
            Console.WriteLine("\nПользователь добавлен!");
        }
        catch (MySql.Data.MySqlClient.MySqlException ex) when (ex.Number == 1062)
        {
            Console.WriteLine($"\nОшибка: пользователь с логином '{user.Login}' уже существует!");
        }

        WaitForKey();
    }

    private void EditUser()
    {
        Console.Clear();
        Console.WriteLine("=== РЕДАКТИРОВАНИЕ ПОЛЬЗОВАТЕЛЯ ===");
        DisplayUsersSimple();

        string login = ReadRequired("Логин пользователя");
        var user = _userRepo.GetByLogin(login);

        if (user == null)
        {
            Console.WriteLine("Пользователь не найден!");
            WaitForKey();
            return;
        }

        Console.Clear();
        Console.WriteLine($"Редактирование: {login}");
        Console.WriteLine("(Оставьте пустым, чтобы не менять)");
        Console.WriteLine(new string('-', 50));

        user.Name = ReadOrKeep("Имя", user.Name);
        user.Surname = ReadOrKeep("Фамилия", user.Surname);
        user.Patronomic = ReadOrKeep("Отчество", user.Patronomic ?? "");
        user.Password = ReadOrKeep("Пароль", user.Password);
        user.BirthDate = ReadDateOrKeep("Дата рождения", user.BirthDate);

        string currentGroup = user.GroupName ?? "—";
        string groupInput = ReadOrKeep($"Группа ({currentGroup})", currentGroup);
        if (groupInput != currentGroup)
            user.GroupId = _groupRepo.GetOrCreateId(groupInput);

        Console.WriteLine($"Текущий адрес: {FormatAddress(user)}");
        string city = ReadOrKeep("Новый город", user.City ?? "");
        string street = ReadOrKeep("Новая улица", user.Street ?? "");
        string house = ReadOrKeep("Новый дом", user.House ?? "");
        string apartment = ReadOrKeep("Новая квартира", user.Apartment ?? "");

        if (!string.IsNullOrWhiteSpace(city) || !string.IsNullOrWhiteSpace(street) || !string.IsNullOrWhiteSpace(house))
            user.AddressId = _addressRepo.GetOrCreateId(city, street, house, apartment);

        _userRepo.Update(user);
        Console.WriteLine("\nПользователь обновлён!");
        WaitForKey();
    }

    private void DeleteUser()
    {
        Console.Clear();
        Console.WriteLine("=== УДАЛЕНИЕ ПОЛЬЗОВАТЕЛЯ ===");
        DisplayUsersSimple();

        string login = ReadRequired("Логин пользователя");

        Console.Write($"Удалить пользователя '{login}'? (y/n): ");
        string confirm = Console.ReadLine()?.Trim().ToLower();

        if (confirm != "y")
        {
            Console.WriteLine("Удаление отменено.");
            WaitForKey();
            return;
        }

        _userRepo.Delete(login);
        Console.WriteLine("Пользователь удалён!");
        WaitForKey();
    }

    private void DisplayUsersSimple()
    {
        var users = _userRepo.GetAll();
        Console.WriteLine($"{"Login",-20} {"Name",-20} {"Surname",-20}");
        Console.WriteLine(new string('-', 60));
        foreach (var user in users)
            Console.WriteLine($"{user.Login,-20} {user.Name,-20} {user.Surname,-20}");
        Console.WriteLine();
    }

    private string ReadRequired(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            string value = Console.ReadLine()?.Trim() ?? "";
            if (!string.IsNullOrEmpty(value))
                return value;
            Console.WriteLine("Значение не может быть пустым!");
        }
    }

    private string ReadOptional(string prompt)
    {
        Console.Write($"{prompt}: ");
        return Console.ReadLine()?.Trim() ?? "";
    }

    private string ReadOrKeep(string prompt, string currentValue)
    {
        Console.Write($"{prompt} ({currentValue}): ");
        string input = Console.ReadLine()?.Trim() ?? "";
        return string.IsNullOrEmpty(input) ? currentValue : input;
    }

    private DateTime? ReadDate(string prompt)
    {
        Console.Write($"{prompt}: ");
        string input = Console.ReadLine()?.Trim() ?? "";
        
        if (string.IsNullOrEmpty(input))
            return null;

        if (DateTime.TryParse(input, out DateTime date))
            return date;

        Console.WriteLine("Неверный формат даты, пропускаем");
        return null;
    }

    private DateTime? ReadDateOrKeep(string prompt, DateTime? currentValue)
    {
        string currentStr = currentValue?.ToString("yyyy-MM-dd") ?? "—";
        Console.Write($"{prompt} ({currentStr}): ");
        string input = Console.ReadLine()?.Trim() ?? "";

        if (string.IsNullOrEmpty(input))
            return currentValue;

        if (DateTime.TryParse(input, out DateTime date))
            return date;

        Console.WriteLine("Неверный формат даты, оставляем старое значение");
        return currentValue;
    }

    private void WaitForKey()
    {
        Console.WriteLine("\nНажмите Enter для продолжения...");
        Console.ReadLine();
    }
}