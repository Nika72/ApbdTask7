using Microsoft.AspNetCore.Mvc;
using Solution7.Models;
using Solution7.Services;
using System;

namespace Solution7.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WarehouseController : ControllerBase
    {
        private readonly WarehouseService _warehouseService;

        public WarehouseController(WarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpPost("add-product-to-warehouse")]
        public IActionResult AddProductToWarehouse([FromBody] ProductWarehouseDto productWarehouse)
        {
            try
            {
                if (!_warehouseService.ValidateProductAndWarehouse(productWarehouse.IdProduct, productWarehouse.IdWarehouse))
                {
                    return BadRequest("Invalid product or warehouse ID.");
                }

                if (!_warehouseService.CheckOrderValidity(productWarehouse.IdProduct, productWarehouse.Amount, productWarehouse.CreatedAt))
                {
                    return BadRequest("No valid order found for this product.");
                }

                int recordId = _warehouseService.UpdateDatabase(productWarehouse);
                return Ok(new { RecordId = recordId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}