using Solution7.Models;
using Solution7.Repositories;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;  // Ensure you have this namespace for logging

namespace Solution7.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IProductRepository _productRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<WarehouseService> _logger;  // Logger to help with debugging and error tracking

        public WarehouseService(
            IProductRepository productRepository, 
            IWarehouseRepository warehouseRepository, 
            IOrderRepository orderRepository, 
            ILogger<WarehouseService> logger)  // Inject logger
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _warehouseRepository = warehouseRepository ?? throw new ArgumentNullException(nameof(warehouseRepository));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> ValidateProductAndWarehouse(int productId, int warehouseId)
        {
            try
            {
                bool productExists = await _productRepository.Exists(productId);
                bool warehouseExists = await _warehouseRepository.Exists(warehouseId);
                return productExists && warehouseExists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating product and warehouse IDs");
                return false;
            }
        }

        public async Task<bool> CheckOrderValidity(int productId, int amount, DateTime createdAt)
        {
            try
            {
                return await _orderRepository.IsValidOrder(productId, amount, createdAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking order validity");
                return false;
            }
        }

        public async Task<int> UpdateDatabase(ProductWarehouseDto productWarehouse)
        {
            try
            {
                return await _orderRepository.ProcessOrder(productWarehouse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating database with new product warehouse data");
                return -1; // Indicate failure with a typical error code
            }
        }

        public async Task<int> ExecuteProductWarehouseProcedure(ProductWarehouseDto productWarehouse)
        {
            try
            {
                return await _orderRepository.ExecuteProductWarehouseProcedure(productWarehouse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing product warehouse procedure");
                return -1; // Indicates failure
            }
        }
    }
}
