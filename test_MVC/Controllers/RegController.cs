using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using test_MVC.Models;

namespace test_MVC.Controllers
{
    public class RegController : Controller
    {
        private readonly HttpClient _httpClient;

        public RegController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new System.Uri("http://localhost:5207/"); 
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate(UserModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Login/login", model);

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserModel>();
                if (user != null)
                {
                    return RedirectToAction("Index", "Product");
                }
            }

            ViewBag.ErrorMessage = "Некорректные данные";
            return View("Login");
        }
    }
}
