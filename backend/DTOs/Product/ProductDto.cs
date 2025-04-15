using FluentValidation;

namespace backend.DTOs.Product
{
    public class CreateProductDto
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public IFormFile? Image { get; set; }  
        public string CategoryId { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class ProductDto
    {

        public  Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public string? Image { get; set; }

        public string? CategoryId { get; set; }

        public string? Description { get; set; }
    }
    public class ProductValidator : AbstractValidator<ProductDto>
    {

        public ProductValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.CategoryId).NotEmpty();
        }
    }
}