using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using UsersAuthorization.Domain.Entities;

namespace UsersAuthorization.Application.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<IdentityRole<int>>> GetRoles();
        Task<bool> AssignRole(string email, string password);
    }
}
