using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tryy.Models;
namespace tryy.DATA
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDBContext _context;

        public RoleRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task<List<RoleModel>> GetRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<RoleModel> GetRoleByIdAsync(int id)
        {
            return await _context.Roles.FirstOrDefaultAsync(role => role.Id == id);
        }

        public async Task<RoleModel> CreateRoleAsync(RoleModel role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<RoleModel> UpdateRoleAsync(RoleModel role)
        {
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<RoleModel> DeleteRoleAsync(int id)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(role => role.Id == id);
            if (role != null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
            }
            return role;
        }
    }
}
