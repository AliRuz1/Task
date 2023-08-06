using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using test_MVC.Models;

namespace test_MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<UserController> _logger;

        public UserController(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<UserController> logger)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login([FromForm] UserModel model)
        {
            try
            {
        using (var client = _httpClientFactory.CreateClient())
        {
            client.BaseAddress = new Uri("http://localhost:5052");
            var response = await client.GetAsync($"api/Users/email/{model.Email}");

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserModel>();

                _logger.LogInformation("API Response: {0}", await response.Content.ReadAsStringAsync());

                _logger.LogInformation("User: {0}, IsBlocked: {1}", user.Email, user.IsBlocked);
                    if (user.IsBlocked)
                    {
                        ViewData["ErrorMessage"] = "Вы заблокированы. Обратитесь к администратору для разблокировки.";
                        return View("Login");
                    }
                if (user != null && user.Roles != null && user.Roles.Count > 0)
                {


                    _logger.LogInformation("User Roles: {0}", string.Join(",", user.Roles.Select(r => r.Name)));

                    await AuthenticateUser(user.Email, user.Roles);
                }
                else
                {
                    await AuthenticateUser(user.Email, new List<RoleModel>());
                }

                _logger.LogInformation("Ответ API");
                return RedirectToAction("Index", "Product");
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Response status code: {response.StatusCode}");
                _logger.LogInformation($"Response content: {responseContent}");
            }
        }
    }
    catch (Exception ex)
    {
        _logger.LogError("Error: " + ex.Message);
        return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
    }

    ViewData["ErrorMessage"] = "Неверный email или пароль.";
    return View("Login");
}



        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("ApplicationCookie");
            return RedirectToAction("Index");
        }


        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ManageUsers()
        {
            try
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5052"); 
                    var response = await client.GetAsync("api/Users");
                    if (response.IsSuccessStatusCode)
                    {
                        var users = await response.Content.ReadFromJsonAsync<List<UserModel>>();
                        return View(users);
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = "Ошибка при получении списка пользователей.";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "Ошибка при получении списка пользователей: " + ex.Message;
                return View();
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddUser()
        {
            try
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5052");
                    var response = await client.GetAsync("api/Roles");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var roles = JsonConvert.DeserializeObject<List<RoleModel>>(responseContent);

                        var model = new UserModel
                        {
                            Roles = roles 
                        };

                        ViewBag.RolesList = new SelectList(roles, "Id", "Name");
                        return View(model);
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = "Ошибка при получении списка ролей.";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "Ошибка при получении списка ролей: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddUser(UserModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var client = _httpClientFactory.CreateClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:5052"); 
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var response = await client.PostAsync("api/Users", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
                        if (response.IsSuccessStatusCode)
                        {
                            return RedirectToAction("ManageUsers");
                        }
                        else
                        {
                            ViewData["ErrorMessage"] = "Ошибка при создании пользователя.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] = "Ошибка при создании пользователя: " + ex.Message;
                    _logger.LogError(ex, "Ошибка при создании пользователя: " + ex.Message);
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EditUser(int id)
        {
            try
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5052"); 
                    var response = await client.GetAsync($"api/Users/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        var user = await response.Content.ReadFromJsonAsync<UserModel>();
                        if (user != null)
                        {
                            return View(user);
                        }
                        else
                        {
                            ViewData["ErrorMessage"] = "Пользователь не найден.";
                            return View();
                        }
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = "Ошибка при получении данных пользователя.";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "Ошибка при получении данных пользователя: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EditUser(UserModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var client = _httpClientFactory.CreateClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5052"); 
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.PutAsync("api/Users", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("ManageUsers");
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = "Ошибка при обновлении данных пользователя.";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "Ошибка при обновлении данных пользователя: " + ex.Message;
                _logger.LogError(ex, "Ошибка при обновлении данных пользователя: " + ex.Message);
            }
        }

        return View(model);
    }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5052"); 
                    var response = await client.DeleteAsync($"api/Users/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                       
                        return RedirectToAction("ManageUsers");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Ошибка при удалении пользователя.";
                        return RedirectToAction("ManageUsers");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка при удалении пользователя: " + ex.Message;
                return RedirectToAction("ManageUsers");
            }
        }

        private async Task AuthenticateUser(string userEmail, List<RoleModel> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userEmail),
                new Claim(ClaimTypes.Email, userEmail),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var identity = new ClaimsIdentity(claims, "ApplicationCookie");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("ApplicationCookie", principal);
            _logger.LogInformation("User authenticated successfully.");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> BlockUser(int id)
        {
            try
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5052"); 
                    var response = await client.PutAsync($"api/Users/{id}/block", null);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("ManageUsers");
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = "Ошибка при блокировке пользователя.";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "Ошибка при блокировке пользователя: " + ex.Message;
                _logger.LogError(ex, "Ошибка при блокировке пользователя: " + ex.Message);
            return View();
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UnblockUser(int id)
        {
            try
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5052"); 
                    var response = await client.PutAsync($"api/Users/{id}/unblock", null);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("ManageUsers");
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = "Ошибка при разблокировке пользователя.";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "Ошибка при разблокировке пользователя: " + ex.Message;
                _logger.LogError(ex, "Ошибка при разблокировке пользователя: " + ex.Message);
                return View();
            }
        }

    }
}
