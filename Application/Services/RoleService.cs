using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UsersAuthorization.Application.Interfaces;
using UsersAuthorization.Domain.Entities;
using UsersAuthorization.Infrastructure.Data;

namespace UsersAuthorization.Application.Service
{
    public class RoleService : IRoleService
    {
        private readonly UserDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public RoleService(UserDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public async Task<IEnumerable<IdentityRole<int>>> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            return roles;
        }
        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.Users.FirstOrDefault(u => u.UserName.ToLower() == email.ToLower());
            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole<int>(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);

                return true;
            }

            return false;
        }
    }
}
