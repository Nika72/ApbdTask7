using Solution7.Models;

namespace Solution7.Repositories
{
    public interface IOrderRepository
    {
        Task<bool> IsValidOrder(int productId, int amount, DateTime createdAt);
        Task<int> ProcessOrder(ProductWarehouseDto productWarehouse);
    }
}