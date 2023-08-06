using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tryy.Models;

namespace tryy.DATA
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDBContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(AppDBContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<UserModel>> GetUsersAsync()
        {
            return await _context.Users.Include(u => u.Roles).ToListAsync();
        }

        public async Task<UserModel> GetUserByIdAsync(int id)
        {
            return await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id);
        }


             public async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

public async Task<UserModel> AddUserAsync(UserModel user)
{
    try
    {
        if (await UserExists(user.Email))
        {
            throw new ArgumentException("Пользователь с таким Email уже существует");
        }

        var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Id == 2);
        if (userRole == null)
        {
            throw new InvalidOperationException("Роль с Id = 2 не найдена в базе данных");
        }

        user.Roles = new List<RoleModel> { userRole };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
    catch (Exception ex)
    {
        throw new Exception("Ошибка при добавлении пользователя.", ex);
    }
}

  public void AddUserWithUserRole(UserModel user)
        {
            var userRole = _context.Roles.FirstOrDefault(r => r.Name == "user");

            if (userRole == null)
            {
                userRole = new RoleModel { Name = "user" };
                _context.Roles.Add(userRole);
            }

            user.Roles = new List<RoleModel> { userRole };

            _context.Users.Add(user);
            _context.SaveChanges();
        }


        public async Task<UserModel> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return user;
        }

        public async Task<UserModel> GetUserByEmailAndPasswordAsync(string email, string password)
        {
            var user = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return null; 
            }

            bool isPasswordValid = VerifyPassword(password, user.Password);

            if (!isPasswordValid)
            {
                return null; 
            }

            return user; 
        }

          public async Task<UserModel> UpdateUserAsync(UserModel user)
{
    try
    {
        var existingUser = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == user.Id);
        if (existingUser == null)
        {
            throw new ArgumentException("Пользователь не найден.");
        }

       
        existingUser.Email = user.Email;
        existingUser.Password = user.Password;

        await _context.SaveChangesAsync();

        return existingUser;
    }
    catch (Exception ex)
    {
        throw new Exception("Ошибка при обновлении пользователя.", ex);
    }
}


        private string GetHashedPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            string hashedInput = GetHashedPassword(password);
            return hashedInput == hashedPassword;
        }

        public async Task<List<RoleModel>> GetRolesForUserAsync(int userId)
        {
            
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.Roles ?? new List<RoleModel>();
        }

        public async Task<UserModel> GetUserByEmailAsync(string email)
        {
            return await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserModel> BlockUserAsync(int userId)
{
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    if (user != null)
    {
        user.IsBlocked = true;
        await _context.SaveChangesAsync();
    }
    return user;
}

public async Task<UserModel> UnblockUserAsync(int userId)
{
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    if (user != null)
    {
        user.IsBlocked = false;
        await _context.SaveChangesAsync();
    }
    return user;
}
    }
}
