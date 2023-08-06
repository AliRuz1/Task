using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using tryy.Models;
using tryy.DATA;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace tryy.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly ProductRepository _productRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ProductRepository productRepository, CategoryRepository categoryRepository, ILogger<ProductsController> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _productRepository.GetProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal Server Error: " + ex.Message);
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productRepository.GetProductByIdAsync(id);
                if (product == null)
                    return NotFound(new { message = "Продукт не найден" });

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal Server Error: " + ex.Message);
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddProduct(ProductModel product)
        {
        try
        {
            if (!await _categoryRepository.CategoryExists(product.CategoryId))
            {
                return BadRequest("Указанной категории не существует");
            }

            var newProduct = new ProductModel
            {
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                NoteGeneral = product.NoteGeneral,
                NoteSpecial = product.NoteSpecial,
                CategoryId = product.CategoryId 
            };

            _logger.LogInformation("Попытка добавления нового продукта: {ProductName}", newProduct.Name);

            var createdProduct = await _productRepository.AddProductAsync(newProduct);

            _logger.LogInformation("Продукт успешно добавлен. Id: {ProductId}", createdProduct.Id);

        return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
        }
        catch (Exception ex)
        {
        _logger.LogError(ex, "Ошибка при добавлении продукта: {ErrorMessage}", ex.Message);

        return StatusCode(500, new { message = "Internal Server error " + ex.Message });
        }
        }



        [HttpPut]
        public async Task<IActionResult> UpdateProduct(ProductModel product)
        {
            try
            {
                var _product = await _productRepository.GetProductByIdAsync(product.Id);
                if (_product == null)
                    return NotFound(new { message = "Продукт не найден" });

                _product.Name = product.Name;
                _product.Price = product.Price;
                _product.Description = product.Description;
                _product.NoteGeneral = product.NoteGeneral;
                _product.NoteSpecial = product.NoteSpecial;

                var updatedProduct = await _productRepository.UpdateProductAsync(_product);
                return Ok(updatedProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal Server Error: " + ex.Message);
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var deletedProduct = await _productRepository.DeleteProductAsync(id);
                if (deletedProduct == null)
                    return NotFound(new { message = "Продукт не найден" });

                return Ok(deletedProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal Server Error: " + ex.Message);
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProductByName(string productName)
        {
            try
            {
                var products = await _productRepository.GetProductsAsync();
                var foundProducts = products.Where(p => p.Name.Contains(productName)).ToList();
                return Ok(foundProducts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal Server Error: " + ex.Message);
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpGet("ByCategory/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = await _productRepository.GetProductsByCategoryIdAsync(categoryId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal Server Error: " + ex.Message);
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

        private async Task<List<CategoryModel>> GetAllCategories()
        {
            List<CategoryModel> categoryList = new List<CategoryModel>();
            var categories = await _categoryRepository.GetAllAsync();
            categoryList = categories.ToList();

            return categoryList;
        }
    }
}
