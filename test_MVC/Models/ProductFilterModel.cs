using test_MVC.Models;

public class ProductFilterwModel
{
    public string ProductName { get; set; }
    public int? CategoryId { get; set; }
    public List<ProductModel> Products { get; set; }
    public List<CategoryModel> Categories { get; set; }
}
