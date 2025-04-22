using backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaticController : ControllerBase
    {
        private readonly IStaticServicecs _staticService;

        public StaticController(IStaticServicecs staticServicecs)
        {
            _staticService = staticServicecs;
        }
        [HttpGet("total-order")]
        public async Task<IActionResult> GetTotalOrder()
        {
            try
            {
                var totalOrder = await _staticService.GetTotalByDay();
                return Ok(new { totalOrder });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenue()
        {
            try
            {
                var revenue = await _staticService.GetRevenue();
                return Ok(revenue);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("product-sale")]
        public async Task<IActionResult> GetProductSale()
        {
            try
            {
                var productSale = await _staticService.GetProductSale();
                return Ok(productSale);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
