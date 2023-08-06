using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using test_MVC.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json;

namespace test_MVC.Controllers
{
    public class CategoryController: Controller
    {
        private readonly HttpClient _httpClient;

        public CategoryController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new System.Uri("http://localhost:5052");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<CategoryModel> categoryList = new List<CategoryModel>();
            var response = await _httpClient.GetAsync("/api/Categories");
            
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                categoryList = JsonConvert.DeserializeObject<List<CategoryModel>>(data);
                ViewBag.Categories = categoryList;
            }
            return View();
        }

        [HttpGet("editcategory/{id}")]
        public async Task<IActionResult> EditCategory(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/Categories/{id}");
            string content = await response.Content.ReadAsStringAsync();
            CategoryModel category = JsonConvert.DeserializeObject<CategoryModel>(content);
            return View(category);
        }

        [HttpPost("editcategory/{id}")]
        public async Task<IActionResult> EditCategory(int id, CategoryModel model)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"/api/Categories", model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }

        [HttpGet("deletecategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"/api/Categories/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        [HttpGet("addcategory")]
        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost("addcategory")]
        public async Task<IActionResult> AddCategory(CategoryModel model)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/Categories/", model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }
    }
}
