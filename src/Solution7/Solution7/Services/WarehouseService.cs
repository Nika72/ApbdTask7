using Solution7.Models;
using Solution7.Repositories;
using System.Threading.Tasks;

namespace Solution7.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IProductRepository _productRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IOrderRepository _orderRepository;

        public WarehouseService(IProductRepository productRepository, IWarehouseRepository warehouseRepository, IOrderRepository orderRepository)
        {
            _productRepository = productRepository;
            _warehouseRepository = warehouseRepository;
            _orderRepository = orderRepository;
        }

        public async Task<bool> ValidateProductAndWarehouse(int productId, int warehouseId)
        {
            bool productExists = await _productRepository.Exists(productId);
            bool warehouseExists = await _warehouseRepository.Exists(warehouseId);
            return productExists && warehouseExists;
        }

        public async Task<bool> CheckOrderValidity(int productId, int amount, DateTime createdAt)
        {
            return await _orderRepository.IsValidOrder(productId, amount, createdAt);
        }

        public async Task<int> UpdateDatabase(ProductWarehouseDto productWarehouse)
        {
            return await _orderRepository.ProcessOrder(productWarehouse);
        }
    }
}