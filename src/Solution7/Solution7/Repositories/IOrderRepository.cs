using Solution7.Models;
using System.Threading.Tasks;

namespace Solution7.Repositories
{
    public interface IOrderRepository
    {
        Task<bool> IsValidOrder(int productId, int amount, DateTime createdAt);
        Task<int> ProcessOrder(ProductWarehouseDto productWarehouse);
        Task<int> ExecuteProductWarehouseProcedure(ProductWarehouseDto productWarehouse); // Add this line
    }
}