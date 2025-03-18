using backend.Data;
using backend.DTOs.Category;
using backend.DTOs.Product;
using backend.Models;
using backend.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Implement
{
    public class ProductService : IProductService
    {
        private readonly NhahangContext _context;
        private readonly IValidator<ProductDto> _validator;
        public ProductService(NhahangContext context, IValidator<ProductDto> validator)
        {
            _context = context;
            _validator = validator;
        }

        public Task<CateogryDto> CreateCategory(CateogryDto cateogryDto)
        {
            try
            {
                var existing = _context.Categories.FirstOrDefault(x => x.Id == cateogryDto.Id);
                if (existing != null)
                {
                    throw new ValidationException("Category already exists");
                }
                var category = new Category
                {
                    Id = cateogryDto.Id,
                    Name = cateogryDto.Name
                };
                _context.Categories.Add(category);
                _context.SaveChanges();
                return Task.FromResult(cateogryDto);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<ProductDto> CreateProduct(ProductDto productDto)
        {

            try
            {
                var validate = await _validator.ValidateAsync(productDto);
                if (!validate.IsValid)
                {
                    throw new ValidationException(validate.Errors);
                }
                var category = await _context.Categories.FindAsync(productDto.CategoryId);
                var existingproduct = await _context.Products.FirstOrDefaultAsync(x => x.Id == productDto.Id);
                if (category == null)
                {
                    throw new ValidationException("Category not found");

                }
                if (existingproduct != null)
                {
                    throw new ValidationException("Product already exists");
                }
                var product = new Product
                {
                    Id = productDto.Id,
                    Name = productDto.Name,
                    Price = productDto.Price,
                    Image = productDto.Image,
                    CategoryId = productDto.CategoryId,
                    Description = productDto.Description ?? string.Empty
                };
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return productDto;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> Delete(string id)
        {
            try
            {
                var existing = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
                if (existing == null)
                {
                    throw new ValidationException("Product not found");
                }
                existing.isDeleted = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public async Task<IEnumerable<CartDisplayDto>> GetCartItemsAsync(IEnumerable<string> list)
        {
            var query = _context.Products
                 .Where(x => list.Contains(x.Id))
                 .Select(x => new CartDisplayDto
                 {
                     id = x.Id,
                     name = x.Name ?? string.Empty,
                     price = x.Price ?? 0f
                 });
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<CateogryDto>> GetCateogry()
        {
            return await _context.Categories.Select(x => new CateogryDto
            {
                Id = x.Id,
                Name = x.Name ?? string.Empty
            }).ToListAsync();
        }

        public async Task<(IEnumerable<ProductDto>,int totalPages)> GetProductsAsync(int page, int pageSize, string? search, string? categoryId, float? from, float? to)
        {
            var query = _context.Products
                .Include(x => x.Category)
                .AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.Contains(search));
            }

            if (!string.IsNullOrEmpty(categoryId))
            {
                query = query.Where(x => x.CategoryId == categoryId);
            }

            if (from.HasValue)
            {
                query = query.Where(x => x.Price >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(x => x.Price <= to.Value);
            }
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var products= await query.
            Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name ?? string.Empty,
                Price = x.Price ?? 0f,
                CategoryId = x.CategoryId,
                Description = x.Description,
                Image = x.Image
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            return (products, totalPages);
        }


        public async Task<Product> UpdateProduct(ProductDto pd)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == pd.Id) ?? throw new ValidationException("Product not found");
            product.Name = pd.Name;
            product.Price = pd.Price;
            product.CategoryId = pd.CategoryId;
            product.Description = pd.Description;
            product.Image = pd.Image;
            await _context.SaveChangesAsync();
            return product;
        }
    }

}
