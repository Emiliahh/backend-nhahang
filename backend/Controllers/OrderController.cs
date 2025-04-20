using backend.DTOs;
using backend.DTOs.Order;
using backend.Hubs;
using backend.Services.Implement;
using backend.Services.Interfaces;
using Backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Net.payOS;
using Net.payOS.Types;
using System.Security.Claims;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IOrderService orderService , IHubContext<AdminHub>  hubContext) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;
        private readonly PayOS payOS = new("1c4da33c-6077-486f-8911-54175a573f11", "d28d0082-518f-48da-8dee-1c278cfd609b", "8a45233b7f8bc3ca3f5cc60069b266e33fee79817c4e297291eb2b51235d2450");  
        private readonly IHubContext<AdminHub> _hubContext = hubContext;
        public static string Base64Encode(Guid id)
        {
            byte[] bytes = id.ToByteArray();
            string base64 = Convert.ToBase64String(bytes).TrimEnd('=');
            return base64.PadRight(25, '0');
        }

        public static Guid Base64Decode(string base64EncodedData)
        {
            string base64 = base64EncodedData.TrimEnd('0');
            if (base64.Length % 4 != 0)
            {
                base64 = base64.PadRight(base64.Length + (4 - base64.Length % 4), '=');
            }
            byte[] bytes = Convert.FromBase64String(base64);
            return new Guid(bytes);
        }
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
                var (orderId,totalPrice) = await _orderService.CreteOrder(orderDto,userId);
                if (orderId == null)
                {
                    return BadRequest(new { message = "Order creation failed" });
                }
                await _hubContext.Clients.All.SendAsync("OrderCreated", "hey");
                return Ok(new
                {
                    orderId,
                    totalPrice
                });
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
        [HttpPost]
        public async Task<IActionResult> generatePayLink([FromBody] PayLinkDto dto)
        {
            var domain = "http://localhost:5173/success";
   
            if(dto == null || dto.Amount <= 0)
            {
                return BadRequest(new { message = "Invalid amount" });
            }
            var code = Base64Encode(dto.OrderId);
            Console.WriteLine(code.Length);
            var paymentLinkRequest = new PaymentData(
                orderCode: int.Parse(DateTimeOffset.Now.ToString("ffffff")),
                amount: (int)dto.Amount,
                description: code,
                items: [new("Mì tôm hảo hảo ly", 1, 2000)],
                returnUrl: domain,
                cancelUrl: domain
            );

            var response = await payOS.createPaymentLink(paymentLinkRequest);

            return Ok(new { paymentUrl = response.checkoutUrl });
        }

        //https://18a2-2405-4802-1f02-ce0f-857e-9fdb-9dc7-1817.ngrok-free.app //
        [HttpPost("/receive_hook")]
        public async Task<IActionResult> ReceiveHook([FromBody] WebhookType webhookType)
        {
            try
            {
                var data = payOS.verifyPaymentWebhookData(webhookType);

                if (data == null)
                {
                    return Ok(new { message = "Invalid webhook data" });
                }

                var orderId = Base64Decode(data.description);
                var isPass = await _orderService.UpdatePaymentStatus(orderId);
                await _hubContext.Clients.All.SendAsync("OrderUpdated", "hey");
                return Ok(new { message = "Success" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Ok(ex.Message);
            }
        }

    }
}
