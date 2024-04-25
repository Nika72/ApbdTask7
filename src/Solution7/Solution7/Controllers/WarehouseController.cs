using Microsoft.AspNetCore.Mvc;
using Solution7.Models;
using Solution7.Services;
using System;
using System.Threading.Tasks; // Ensure this namespace is included for Task

namespace Solution7.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpPost("add-product-to-warehouse")]
        public async Task<IActionResult> AddProductToWarehouse([FromBody] ProductWarehouseDto productWarehouse)
        {
            try
            {
                // Await the asynchronous method and use it in the condition
                if (!await _warehouseService.ValidateProductAndWarehouse(productWarehouse.IdProduct, productWarehouse.IdWarehouse))
                {
                    return BadRequest("Invalid product or warehouse ID.");
                }

                if (!await _warehouseService.CheckOrderValidity(productWarehouse.IdProduct, productWarehouse.Amount, productWarehouse.CreatedAt))
                {
                    return BadRequest("No valid order found for this product.");
                }

                // Await the async method to get the record ID
                int recordId = await _warehouseService.UpdateDatabase(productWarehouse);
                return Ok(new { RecordId = recordId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}