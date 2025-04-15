using backend.DTOs.Order;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IOrderService orderService) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;
        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto orderDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return BadRequest(new { message = "User not found" });
                }
                var order = await _orderService.CreteOrder(orderDto,userId);
                return Ok(new { message = "Order created successfully", order });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var res = await _orderService.GetOrders();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });

            }
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusRequest rq, Guid id )
        {
            try
            {
                var res = await _orderService.UpdateOrderStatus(id, rq.Status);
                return Ok(res);
            }catch(ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
