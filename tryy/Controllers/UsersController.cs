using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using tryy.Models;
using tryy.DATA;
using Microsoft.AspNetCore.Authorization;

namespace tryy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserRepository userRepository, ILogger<UsersController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _userRepository.GetUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new { message = "Пользователь не найден" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddUser(UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data");
            }

            _userRepository.AddUserWithUserRole(user);

            return Ok("Пользователь добавлен");
        }


        [HttpPut]
        public async Task<IActionResult> UpdateUser(UserModel user)
        {
            try
            {
                var updatedUser = await _userRepository.UpdateUserAsync(user);
                if (updatedUser == null)
                return NotFound(new { message = "Пользователь не найден" });

                _logger.LogInformation("Пользователь обновлен: {UserId}", updatedUser.Id);

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
            _logger.LogError(ex, "Ошибка при обновлении пользователя: {ErrorMessage}", ex.Message);

            return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var deletedUser = await _userRepository.DeleteUserAsync(id);
                if (deletedUser == null)
                    return NotFound(new { message = "Пользователь не найден" });

                return Ok(deletedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(email);
                if (user == null)
                    return NotFound(new { message = "Пользователь не найден" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserModel model)
        {
            try
            {
                _logger.LogInformation("Попытка входа пользователя с email: {0}", model.Email);

                var user = await _userRepository.GetUserByEmailAndPasswordAsync(model.Email, model.Password);

                if (user != null)
                {

                    if (user.IsBlocked)
                    {
                        return BadRequest("Вы заблокированы.");
                    }
                    _logger.LogInformation("Пользователь с email {0} успешно аутентифицирован.", model.Email);

                    var roles = await _userRepository.GetRolesForUserAsync(user.Id);
                    user.Roles = roles ?? new List<RoleModel>(); 

                    return Ok(user);
                }
                else
                {
                    _logger.LogWarning("Попытка аутентификации с неверным email или паролем: {0}", model.Email);
                    return BadRequest("Неверный логин или пароль");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при аутентификации пользователя с email: {0}", model.Email);
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPut("{id:int}/block")]
        public async Task<IActionResult> BlockUser(int id)
        {
            try
            {
                var blockedUser = await _userRepository.BlockUserAsync(id);
                if (blockedUser == null)
                return NotFound(new { message = "Пользователь не найден" });

                return Ok(blockedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpPut("{id:int}/unblock")]
        public async Task<IActionResult> UnblockUser(int id)
        {
            try
            {
                var unblockedUser = await _userRepository.UnblockUserAsync(id);
                if (unblockedUser == null)
                return NotFound(new { message = "Пользователь не найден" });

                return Ok(unblockedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
            }
        }

    }   
}
