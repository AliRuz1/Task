using System.Collections.Generic;
using System.Threading.Tasks;
using tryy.Models;

namespace tryy.DATA
{
    public interface ICategoryRepository
    {
        Task<List<CategoryModel>> GetCategoriesAsync();
        Task<CategoryModel> GetCategoryByIdAsync(int id);
        Task<CategoryModel> CreateCategoryAsync(CategoryModel category);
        Task<CategoryModel> UpdateCategoryAsync(CategoryModel category);
        Task<CategoryModel> DeleteCategoryAsync(int id);
    }
}
