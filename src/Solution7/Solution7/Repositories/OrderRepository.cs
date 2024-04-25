using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using Solution7.Models;

namespace Solution7.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or whitespace.", nameof(connectionString));

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

            object result = await command.ExecuteScalarAsync();
            return result != DBNull.Value && Convert.ToInt32(result) > 0;
        }

        public async Task<int> ProcessOrder(ProductWarehouseDto productWarehouse)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            decimal price = await CalculatePrice(productWarehouse.IdProduct, productWarehouse.Amount);
            var command = new SqlCommand(@"INSERT INTO Product_Warehouse (IdProduct, IdWarehouse, Amount, Price, CreatedAt) 
                                           OUTPUT INSERTED.Id
                                           VALUES (@IdProduct, @IdWarehouse, @Amount, @Price, @CreatedAt)", connection);
            command.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
            command.Parameters.AddWithValue("@IdWarehouse", productWarehouse.IdWarehouse);
            command.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
            command.Parameters.AddWithValue("@Price", price);
            command.Parameters.AddWithValue("@CreatedAt", productWarehouse.CreatedAt);

            object insertedId = await command.ExecuteScalarAsync();
            return insertedId != DBNull.Value ? Convert.ToInt32(insertedId) : -1;
        }

        public async Task<int> ExecuteProductWarehouseProcedure(ProductWarehouseDto productWarehouse)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var command = new SqlCommand("sp_AddProductToWarehouse", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@ProductId", productWarehouse.IdProduct);
            command.Parameters.AddWithValue("@WarehouseId", productWarehouse.IdWarehouse);
            command.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
            command.Parameters.AddWithValue("@CreatedAt", productWarehouse.CreatedAt);

            object result = await command.ExecuteScalarAsync();
            return result != DBNull.Value ? Convert.ToInt32(result) : -1;
        }

        public async Task<bool> UpdateOrderFulfilledAt(int orderId, DateTime fulfilledAt)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var command = new SqlCommand(@"UPDATE Orders SET FulfilledAt = @FulfilledAt WHERE Id = @OrderId AND FulfilledAt IS NULL", connection);
            command.Parameters.AddWithValue("@OrderId", orderId);
            command.Parameters.AddWithValue("@FulfilledAt", fulfilledAt);

            int result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        private async Task<decimal> CalculatePrice(int productId, int amount)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var command = new SqlCommand(@"SELECT Price FROM ProductPricing WHERE ProductId = @ProductId", connection);
            command.Parameters.AddWithValue("@ProductId", productId);

            object pricePerUnit = await command.ExecuteScalarAsync();
            return pricePerUnit != DBNull.Value ? Convert.ToDecimal(pricePerUnit) * amount : throw new InvalidOperationException("Price per unit is not found.");
        }
    }
}
