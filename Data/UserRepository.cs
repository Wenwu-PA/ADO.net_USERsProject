using MySql.Data.MySqlClient;
using ADONET_Final_project.Models;

namespace ADONET_Final_project.Data;

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<User> GetAll()
    {
        return ExecuteQuery(@"
            SELECT 
                u.login, u.name, u.surname, u.patronomic, u.password, u.birth_date,
                u.id_group, u.id_address,
                g.name AS group_name,
                a.city, a.street, a.house, a.apartment
            FROM users u
            LEFT JOIN `groups` g ON u.id_group = g.id
            LEFT JOIN addresses a ON u.id_address = a.id");
    }

    public List<User> GetByGroupFilter(string filter)
    {
        return ExecuteQuery($@"
            SELECT 
                u.login, u.name, u.surname, u.patronomic, u.password, u.birth_date,
                u.id_group, u.id_address,
                g.name AS group_name,
                a.city, a.street, a.house, a.apartment
            FROM users u
            LEFT JOIN `groups` g ON u.id_group = g.id
            LEFT JOIN addresses a ON u.id_address = a.id
            WHERE {filter}");
    }

    public User? GetByLogin(string login)
    {
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        string query = @"
            SELECT 
                u.login, u.name, u.surname, u.patronomic, u.password, u.birth_date,
                u.id_group, u.id_address,
                g.name AS group_name,
                a.city, a.street, a.house, a.apartment
            FROM users u
            LEFT JOIN `groups` g ON u.id_group = g.id
            LEFT JOIN addresses a ON u.id_address = a.id
            WHERE u.login = @login";

        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@login", login);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
            return MapUser(reader);
        
        return null;
    }

    public void Add(User user)
    {
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        string query = @"
            INSERT INTO users (login, name, surname, patronomic, password, birth_date, id_group, id_address)
            VALUES (@login, @name, @surname, @patronomic, @password, @birthDate, @groupId, @addressId)";

        using var cmd = new MySqlCommand(query, connection);
        AddUserParameters(cmd, user);
        cmd.ExecuteNonQuery();
    }

    public void Update(User user)
    {
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        string query = @"
            UPDATE users
            SET name = @name, surname = @surname, patronomic = @patronomic,
                password = @password, birth_date = @birthDate,
                id_group = @groupId, id_address = @addressId
            WHERE login = @login";

        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@login", user.Login);
        AddUserParameters(cmd, user);
        cmd.ExecuteNonQuery();
    }

    public void Delete(string login)
    {
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        string query = "DELETE FROM users WHERE login = @login";
        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@login", login);
        cmd.ExecuteNonQuery();
    }

    private List<User> ExecuteQuery(string sql)
    {
        var users = new List<User>();

        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        using var cmd = new MySqlCommand(sql, connection);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
            users.Add(MapUser(reader));

        return users;
    }

    private User MapUser(MySqlDataReader reader)
    {
        return new User
        {
            Login = reader["login"]?.ToString() ?? "",
            Name = reader["name"]?.ToString() ?? "",
            Surname = reader["surname"]?.ToString() ?? "",
            Patronomic = reader["patronomic"]?.ToString(),
            Password = reader["password"]?.ToString() ?? "",
            BirthDate = reader["birth_date"] == DBNull.Value ? null : (DateTime?)reader.GetDateTime("birth_date"),
            GroupId = reader["id_group"] == DBNull.Value ? null : (int?)reader.GetInt32("id_group"),
            AddressId = reader["id_address"] == DBNull.Value ? null : (int?)reader.GetInt32("id_address"),
            GroupName = reader["group_name"]?.ToString(),
            City = reader["city"]?.ToString(),
            Street = reader["street"]?.ToString(),
            House = reader["house"]?.ToString(),
            Apartment = reader["apartment"]?.ToString()
        };
    }

    private void AddUserParameters(MySqlCommand cmd, User user)
    {
        cmd.Parameters.AddWithValue("@login", user.Login);
        cmd.Parameters.AddWithValue("@name", string.IsNullOrEmpty(user.Name) ? (object)DBNull.Value : user.Name);
        cmd.Parameters.AddWithValue("@surname", string.IsNullOrEmpty(user.Surname) ? (object)DBNull.Value : user.Surname);
        cmd.Parameters.AddWithValue("@patronomic", string.IsNullOrEmpty(user.Patronomic) ? (object)DBNull.Value : user.Patronomic);
        cmd.Parameters.AddWithValue("@password", user.Password);
        cmd.Parameters.AddWithValue("@birthDate", user.BirthDate.HasValue ? (object)user.BirthDate.Value.ToString("yyyy-MM-dd") : DBNull.Value);
        cmd.Parameters.AddWithValue("@groupId", user.GroupId.HasValue ? (object)user.GroupId.Value : DBNull.Value);
        cmd.Parameters.AddWithValue("@addressId", user.AddressId.HasValue ? (object)user.AddressId.Value : DBNull.Value);
    }
}