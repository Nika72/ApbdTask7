using Microsoft.Data.SqlClient; 
using System.Threading.Tasks;
using Solution7.Models;

namespace Solution7.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> IsValidOrder(int productId, int amount, DateTime createdAt)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var command = new SqlCommand(@"SELECT COUNT(*) FROM Orders 
                                           WHERE IdProduct = @ProductId AND Amount >= @Amount 
                                           AND CreatedAt < @CreatedAt AND FulfilledAt IS NULL", connection);
            command.Parameters.AddWithValue("@ProductId", productId);
            command.Parameters.AddWithValue("@Amount", amount);
            command.Parameters.AddWithValue("@CreatedAt", createdAt);

            int result = (int)await command.ExecuteScalarAsync();
            return result > 0;
        }

        public async Task<int> ProcessOrder(ProductWarehouseDto productWarehouse)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var command = new SqlCommand(@"INSERT INTO Product_Warehouse (IdProduct, IdWarehouse, Amount, Price, CreatedAt) 
                                           OUTPUT INSERTED.Id
                                           VALUES (@IdProduct, @IdWarehouse, @Amount, @Price, @CreatedAt)", connection);
            command.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
            command.Parameters.AddWithValue("@IdWarehouse", productWarehouse.IdWarehouse);
            command.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
            command.Parameters.AddWithValue("@Price", CalculatePrice(productWarehouse.Amount)); 
            command.Parameters.AddWithValue("@CreatedAt", productWarehouse.CreatedAt);

            int insertedId = (int)await command.ExecuteScalarAsync();
            return insertedId;
        }

        private decimal CalculatePrice(int amount)
        {
            // Placeholder price calculation, replace with actual logic as needed
            return amount * 10m; // Assuming each unit costs 10
        }
    }
}
