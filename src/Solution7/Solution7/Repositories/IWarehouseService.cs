using Solution7.Models;

namespace Solution7.Services
{
    public interface IWarehouseService
    {
        Task<bool> ValidateProductAndWarehouse(int productId, int warehouseId);
        Task<bool> CheckOrderValidity(int productId, int amount, DateTime createdAt);
        Task<int> UpdateDatabase(ProductWarehouseDto productWarehouse);
    }
}