using System.Collections.Generic;
using System.Threading.Tasks;
using tryy.Models;

namespace tryy.DATA
{
    public interface IRoleRepository
    {
        Task<List<RoleModel>> GetRolesAsync();
        Task<RoleModel> GetRoleByIdAsync(int id);
        Task<RoleModel> CreateRoleAsync(RoleModel role);
        Task<RoleModel> UpdateRoleAsync(RoleModel role);
        Task<RoleModel> DeleteRoleAsync(int id);
    }
}
