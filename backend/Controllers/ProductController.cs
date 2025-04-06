using backend.DTOs.Category;
using backend.DTOs.Product;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using static backend.Exceptions.ProductException;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetProductsAsync(
            [FromQuery] string? search,
            [FromQuery] string? categoryId,
            [FromQuery] float? from,
            [FromQuery] float? to,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool desc = false
            )
        {
            var (product,totalPage)= await _service.GetProductsAsync(page,pageSize, desc,search, categoryId, from, to);
            return Ok(new
            {
                currentPage = page,
                totalPage,
                pageSize,
                data = product
            });
        }
        [HttpPost("add")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto productDto)
        {
            try
            {
                await _service.CreateProduct(productDto);
                return Ok(new { message = "Product created successfully" });
            }
            catch (ProductAlreadyExistException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (CategoryNotExistException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
        [HttpPost("category")]
        public async Task<IActionResult> CreateCategory([FromBody] CateogryDto cateogryDto)
        {
            try
            {
                await _service.CreateCategory(cateogryDto);
                return Ok(new { message = "Category created successfully" });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("category")]
        public async Task<IEnumerable<CateogryDto>> GetCateogry()
        {
            return await _service.GetCateogry();
        }
        [HttpPost("cart-display")]
        public async Task<IEnumerable<CartDisplayDto>> GetCartItemsAsync([FromBody] IEnumerable<string> list)
        {
            return await _service.GetCartItemsAsync(list);
        }
        [HttpPut("update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> upgradeProduct([FromBody] ProductDto pd)
        {
            try
            {
                var product = await _service.UpdateProduct(pd);
                return Ok(product);
            }
            catch (ProductNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception E)
            {
                throw new Exception(E.Message);
            }
        }
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var result = await _service.Delete(id);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
    }
}