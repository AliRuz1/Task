using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using test_MVC.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace test_MVC.Controllers
{
    [Authorize] 
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new System.Uri("http://localhost:5052");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<ProductModel> productList = new List<ProductModel>();
            var response = await _httpClient.GetAsync("/api/Products");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                productList = JsonConvert.DeserializeObject<List<ProductModel>>(data);
                ViewBag.Products = productList;
            }

            List<CategoryModel> categoryList = new List<CategoryModel>();
            var response1 = await _httpClient.GetAsync("/api/Categories");

            if (response1.IsSuccessStatusCode)
            {
                string data = await response1.Content.ReadAsStringAsync();
                categoryList = JsonConvert.DeserializeObject<List<CategoryModel>>(data);
                ViewBag.Categories = categoryList;
            }

            return View();
        }

        [HttpGet("editproduct/{id}")]
        public async Task<IActionResult> EditProduct(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/Products/{id}");
            HttpResponseMessage responseCat = await _httpClient.GetAsync($"/api/Categories");

            string content = await response.Content.ReadAsStringAsync();
            string contentCat = await responseCat.Content.ReadAsStringAsync();

            ProductModel product = JsonConvert.DeserializeObject<ProductModel>(content);
            List<CategoryModel> categories = JsonConvert.DeserializeObject<List<CategoryModel>>(contentCat);

            ViewBag.Product = product;
            ViewBag.Categories = categories;

            return View(product);
        }

        [HttpPost("editproduct/{id}")]
        public async Task<IActionResult> EditProduct(ProductModel model)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/Categories/{model.CategoryId}");
            string content = await response.Content.ReadAsStringAsync();

            CategoryModel category = JsonConvert.DeserializeObject<CategoryModel>(content);
            model.Category = category;

            HttpResponseMessage response1 = await _httpClient.PutAsync("/api/Products", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

            if (response1.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet("delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"api/Products/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            List<CategoryModel> categoryList = await GetAllCategories();

            var product = new ProductModel();
            ViewBag.Categories = new SelectList(categoryList, "Id", "Name");

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductModel product)
        {
            try
            {
                 _logger.LogInformation("Инфа из формы: {ProductData}", JsonConvert.SerializeObject(product));
                List<CategoryModel> categoryList = await GetAllCategories();

                if (!categoryList.Any(c => c.Id == product.CategoryId))
                {
                    ModelState.AddModelError("CategoryId", "Недопустимая категория");
                     ModelState.AddModelError("NoteSpecial", "Поле Примечание специальное обязательно для заполнения");
                    ViewBag.Categories = new SelectList(categoryList, "Id", "Name");
                    return View(product);
                }

                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/Products/add", product);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Product"); 
                }
                else
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Ошибка при добавлении продукта. Код статуса: {StatusCode}, Ответ: {Response}", response.StatusCode, responseContent);
                    ModelState.AddModelError("", "Ошибка при добавлении продукта");
                    ViewBag.Categories = new SelectList(categoryList, "Id", "Name");
                    return View(product);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка в методе CreateProduct: {ex.Message}");
                ModelState.AddModelError("", "Ошибка при добавлении продукта");
                return View(product);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Search(string productName, int? categoryId)
        {
            try
            {
                string url = $"/api/Products/search?productName={productName}";
                if (categoryId.HasValue)
                {
                    url += $"&categoryId={categoryId.Value}";
                }

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var products = await response.Content.ReadFromJsonAsync<List<ProductModel>>();
                    return View(products);
                }
                else
                {
                    return BadRequest("ошибка при получении данных из API");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "ошибка сервера: " + ex.Message });
            }
        }

       [HttpGet]
        public async Task<IActionResult> SearchProductsByCategory()
        {
            var categoryList = await GetAllCategories();
            ViewBag.Categories = new SelectList(categoryList, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchProductsByCategory(int categoryId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Products/ByCategory/{categoryId}");
                if (response.IsSuccessStatusCode)
                {
                    var products = await response.Content.ReadFromJsonAsync<List<ProductModel>>();
                    return View("Filtered", products);
                }
                else
                {
                    return BadRequest("ошбка при получении данных из API");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Filter(int? categoryId)
        {
            try
            {
                List<ProductModel> products;
                if (categoryId.HasValue)
                {
                    var response = await _httpClient.GetAsync($"/api/Products/ByCategory/{categoryId}");
                    if (response.IsSuccessStatusCode)
                    {
                        products = await response.Content.ReadFromJsonAsync<List<ProductModel>>();
                    }
                    else
                    {
                        return BadRequest("Ошибка при получении данных из API");
                    }
                }
                else
                {
                    var response = await _httpClient.GetAsync("/api/Products");
                    if (response.IsSuccessStatusCode)
                    {
                        products = await response.Content.ReadFromJsonAsync<List<ProductModel>>();
                    }
                    else
                    {
                        return BadRequest("Ошибка при получении данных из API");
                    }
                }

                List<CategoryModel> categoryList = await GetAllCategories();
                ViewBag.Categories = categoryList;

                return View(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }


        private async Task<List<CategoryModel>> GetAllCategories()
        {
            List<CategoryModel> categoryList = new List<CategoryModel>();
            HttpResponseMessage response = await _httpClient.GetAsync("/api/Categories");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                categoryList = JsonConvert.DeserializeObject<List<CategoryModel>>(data);
            }

            return categoryList;
        }
    }
}
