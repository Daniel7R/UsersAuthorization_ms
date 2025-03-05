using UsersAuthorization.Application.Messages.Response;
using UsersAuthorization.Domain.Entities;
using UsersAuthorization.Infrastructure.Repository;

namespace UsersAuthorization.Infrastructure.EventHandler
{
    public class UserEventHandler
    {
        private readonly IRepository<ApplicationUser> _userRepository;

        public UserEventHandler(IRepository<ApplicationUser> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<GetUserByIdResponse> GetUserInfo(int idUser)
        {
            var user = await _userRepository.GetByIdAsync(idUser);
#pragma warning disable CS8601 // Posible asignación de referencia nula
            return new GetUserByIdResponse
            {
                Id = user?.Id ?? 0,
                Name = user?.Name,
                Email = user?.Email
            };
        }
    }
}
