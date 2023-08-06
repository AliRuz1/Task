using System.Collections.Generic;
using System.Threading.Tasks;
using tryy.Models;
namespace tryy.DATA
{
    public interface IProductRepository
    {
        Task<List<ProductModel>> GetProductsAsync();
        Task<ProductModel> GetProductByIdAsync(int id);
        Task<ProductModel> AddProductAsync(ProductModel product);
        Task<ProductModel> UpdateProductAsync(ProductModel product);
        Task<ProductModel> DeleteProductAsync(int id);
    }
}
