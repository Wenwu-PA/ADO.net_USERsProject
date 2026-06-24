using MySql.Data.MySqlClient;

namespace ADONET_Final_project.Data;

public class AddressRepository
{
    private readonly string _connectionString;

    public AddressRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public int GetOrCreateId(string city, string street, string house, string apartment)
    {
        if (string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(street) || string.IsNullOrWhiteSpace(house))
            return 0;

        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        string query = @"
            INSERT INTO addresses (city, street, house, apartment)
            VALUES (@city, @street, @house, @apartment)
            ON DUPLICATE KEY UPDATE id = LAST_INSERT_ID(id);
            SELECT LAST_INSERT_ID();";

        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@city", city.Trim());
        cmd.Parameters.AddWithValue("@street", street.Trim());
        cmd.Parameters.AddWithValue("@house", house.Trim());
        cmd.Parameters.AddWithValue("@apartment", string.IsNullOrWhiteSpace(apartment) ? "" : apartment.Trim());

        object result = cmd.ExecuteScalar();
        return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
    }
}