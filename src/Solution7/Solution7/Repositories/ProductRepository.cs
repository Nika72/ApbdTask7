using Microsoft.Data.SqlClient;
using Solution7.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<bool> Exists(int productId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new SqlCommand("SELECT COUNT(1) FROM Products WHERE Id = @productId", connection);
        command.Parameters.AddWithValue("@productId", productId);

        int result = (int)await command.ExecuteScalarAsync();
        return result > 0;
    }
}