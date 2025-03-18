using backend.DTOs.Product;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddProduct([FromBody] CartItemDto cartItemDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartService.addProduct(userId, cartItemDto);
            return Ok(cart);
        }
        [Authorize]
        [HttpGet("get")]
        public async Task<IActionResult> GetCartItem()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartService.getCartItem(userId);
            return Ok(cart);
        }
        [Authorize]
        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> Remove(string productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _cartService.Remove(userId, productId);
            if (result)
            {
                return Ok(new { message = "Product removed successfully" });
            }
            return BadRequest(new { message = "Product not found" });
        }
        [Authorize]
        [HttpPut("update/{productId}")]
        public async Task<IActionResult> UpdateQuantity(string productId, [FromQuery] int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartService.updateQuantity(userId, productId, quantity);
            return Ok(cart);
        }
    }
}
