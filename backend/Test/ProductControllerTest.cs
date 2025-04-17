using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using backend.Controllers;
using backend.DTOs.Category;
using backend.Services.Interfaces;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;

public class ProductControllerTests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly ProductController _productController;

    public ProductControllerTests()
    {
        _mockProductService = new Mock<IProductService>();
        _productController = new ProductController(_mockProductService.Object);
    }

    [Fact]
    public async Task CreateCategory_ValidCategory_ReturnsOk()
    {
        // Arrange
        var categoryDto = new CateogryDto { Id = "1", Name = "Electronics" };
        _mockProductService.Setup(service => service.CreateCategory(It.IsAny<CateogryDto>()))
                          .ReturnsAsync(categoryDto);

        // Act
        var result = await _productController.CreateCategory(categoryDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        dynamic returnValue = okResult.Value;
        Assert.Equal("Category created successfully", returnValue.message);
    }

    [Fact]
    public async Task CreateCategory_InvalidCategory_ReturnsBadRequest()
    {
        // Arrange
        var categoryDto = new CateogryDto { Id = "", Name = "" };
        _mockProductService.Setup(service => service.CreateCategory(It.IsAny<CateogryDto>()))
                          .ThrowsAsync(new ValidationException("Invalid category data"));

        // Act
        var result = await _productController.CreateCategory(categoryDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        dynamic returnValue = badRequestResult.Value;
        Assert.Equal("Invalid category data", returnValue.message);
    }

    [Fact]
    public async Task CreateCategory_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var categoryDto = new CateogryDto { Id = "1", Name = "Phones" };
        _mockProductService.Setup(service => service.CreateCategory(It.IsAny<CateogryDto>()))
                          .ThrowsAsync(new Exception("Server error occurred"));

        // Act
        var result = await _productController.CreateCategory(categoryDto);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        dynamic returnValue = statusCodeResult.Value;
        Assert.Equal("Server error occurred", returnValue.message);
    }
}
