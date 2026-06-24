using MySql.Data.MySqlClient;

namespace ADONET_Final_project.Data;

public class GroupRepository
{
    private readonly string _connectionString;

    public GroupRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public int GetOrCreateId(string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName)) return 0;

        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        string query = @"
            INSERT INTO `groups` (name) VALUES (@name)
            ON DUPLICATE KEY UPDATE id = LAST_INSERT_ID(id);
            SELECT LAST_INSERT_ID();";

        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@name", groupName.Trim());
        
        object result = cmd.ExecuteScalar();
        return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
    }
}