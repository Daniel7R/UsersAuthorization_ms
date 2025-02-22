using UsersAuthorization.Domain.Entities;

namespace UsersAuthorization.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
    }
}
