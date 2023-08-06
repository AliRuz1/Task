using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using tryy.Models;
using tryy.DATA;
namespace tryy.Controllers
{
//    [ApiController]
  //  [Route("api/[controller]")]
    public class CategoriesController : BaseApiController
    {
        private readonly CategoryRepository _categoryRepository;

      public CategoriesController(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _categoryRepository.GetCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryRepository.GetCategoryByIdAsync(id);
                if (category == null)
                    return NotFound(new { message = "Категория не найдена" });

                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryModel category)
        {
            try
            {
                var createdCategory = await _categoryRepository.CreateCategoryAsync(category);
                return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

       [HttpPut]
        public async Task<IActionResult> UpdateCategory(CategoryModel category)
        {
            try
            {
                bool categoryExists = await _categoryRepository.CategoryExists(category.Id);
                if (!categoryExists)
                return NotFound(new { message = "Категория не найдена" });

                var updatedCategory = await _categoryRepository.UpdateCategoryAsync(category);
                return Ok(updatedCategory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var deletedCategory = await _categoryRepository.DeleteCategoryAsync(id);
                if (deletedCategory == null)
                    return NotFound(new { message = "Категория не найдена" });

                return Ok(deletedCategory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

        
    }
}
