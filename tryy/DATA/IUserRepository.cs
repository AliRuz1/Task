using System.Collections.Generic;
using System.Threading.Tasks;
using tryy.Models;
namespace tryy.DATA
{
    public interface IUserRepository
    {
        Task<List<UserModel>> GetUsersAsync();
        Task<UserModel> GetUserByIdAsync(int id);
         Task<UserModel> AddUserAsync(UserModel user);
        Task<UserModel> UpdateUserAsync(UserModel user);
        Task<UserModel> DeleteUserAsync(int id);
        Task<UserModel> GetUserByEmailAndPasswordAsync(string email, string password);
       //         UserModel CreateUser(UserModel user);
     //   UserModel GetUserById(int userId);

        
    }
}
