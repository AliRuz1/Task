using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using test_MVC.Models;

namespace test_MVC.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new System.Uri("http://localhost:5052");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<UserModel> ValidateUserCredentialsAsync(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("/login", new { email, password });

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserModel>();
            }

            return null;
        }
    }
}
