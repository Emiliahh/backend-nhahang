using backend.DTOs.Category;
using backend.DTOs.Product;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using static backend.Exceptions.ProductException;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProductService service) : ControllerBase
    {
        private readonly IProductService _service = service;

        [HttpGet]
        public async Task<IActionResult> GetProductsAsync(
            [FromQuery] string? search,
            [FromQuery] string? categoryId,
            [FromQuery] decimal? from,
            [FromQuery] decimal? to,
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
        //[Authorize(Roles ="Admin")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto productDto)
        {
            try
            {
                var product = await _service.CreateProduct(productDto);
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
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string search)
        {
            try
            {
                var result = await _service.Search(search);
                return Ok(result);
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductAsync(Guid id)
        {
            try
            {
                var product = await _service.GetProductAsync(id);
                if (product == null)
                {
                    return NotFound(new { message = "Product not found" });
                }
                return Ok(product);
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

            var validGuids = new List<Guid>();
            foreach (var item in list)
            {
                if (Guid.TryParse(item, out var guid))
                {
                    validGuids.Add(guid);
                }
            }
            return await _service.GetCartItemsAsync(validGuids);
        }
        [HttpPut("update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> upgradeProduct([FromForm] CreateProductDto pd,Guid id)
        {
            try
            {
                var product = await _service.UpdateProduct(pd,id);
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
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
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