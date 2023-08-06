// using tryy.DATA;
// using tryy.Models;

// namespace tryy.Services
// {
//     public interface IUserService
//     {
//         UserModel CreateUserWithRole(UserModel user, int roleId);
//     }

//     public class UserService : IUserService
//     {
//         private readonly IUserRepository _userRepository;
//         private readonly IRoleRepository _roleRepository;

//         public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
//         {
//             _userRepository = userRepository;
//             _roleRepository = roleRepository;
//         }

//         public UserModel CreateUserWithRole(UserModel user, int roleId)
//         {
//             var role = _roleRepository.GetRoleById(roleId);
//             if (role == null)
//             {
//                 throw new RoleNotFoundException(roleId);
//             }

//             user.Roles = new List<RoleModel> { role };
//             return _userRepository.CreateUser(user);
//         }
//     }
// }
