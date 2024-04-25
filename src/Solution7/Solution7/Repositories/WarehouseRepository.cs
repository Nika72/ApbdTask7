using Microsoft.Data.SqlClient;
using Solution7.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly string _connectionString;

    public WarehouseRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<bool> Exists(int warehouseId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new SqlCommand("SELECT COUNT(1) FROM Warehouses WHERE Id = @warehouseId", connection);
        command.Parameters.AddWithValue("@warehouseId", warehouseId);

        int result = (int)await command.ExecuteScalarAsync();
        return result > 0;
    }
}