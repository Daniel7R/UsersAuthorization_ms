using AutoMapper;
using UsersAuthorization.Application.Messages.Response;
using UsersAuthorization.Domain.Entities;
using UsersAuthorization.Infrastructure.Repository;

namespace UsersAuthorization.Application.EventHandler
{
    public class UserEventHandler
    {
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly IMapper _mapper;

        public UserEventHandler(IRepository<ApplicationUser> userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<GetUserByIdResponse> GetUserInfo(int idUser)
        {

            var user = await _userRepository.GetByIdAsync(idUser);
            return new GetUserByIdResponse
            {
                Id = user?.Id ?? 0,
                Name = user?.Name,
                Email = user?.Email
            };
        }

        /// <summary>
        /// Multiple users
        /// </summary>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public async Task<List<GetUserByIdResponse>> GetUsersInfo(List<int> idUsers)
        {

            var users = await _userRepository.GetAllAsync();

            var filteredUsers = users.Where(u => idUsers.Contains(u.Id)).ToList();

            List<GetUserByIdResponse> response = filteredUsers.Select(user => new GetUserByIdResponse
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name
            }
            ).ToList();
            return response;

        }

        /// <summary>
        /// Returns id users register
        /// </summary>
        /// <returns></returns>
        public async Task<List<int>> GetAllUserEmails()
        {
            var users = await _userRepository.GetAllAsync();
            List<int> emails = new List<int>();
            if (users != null && users.Any() && users.Count() > 0)
            {
                emails = users.Select(x => x.Id).ToList();
            }

            return emails;
        }
    }
}
