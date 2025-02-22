using AutoMapper;
using Microsoft.AspNetCore.Identity;
using UsersAuthorization.Application.DTO;
using UsersAuthorization.Application.Interfaces;
using UsersAuthorization.Domain.Entities;
using UsersAuthorization.Infrastructure.Data;
using UsersAuthorization.Infrastructure.Repository;

namespace UsersAuthorization.Application.Service
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IMapper _mapper;

        public AuthService(IRepository<ApplicationUser> userRepository, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager, IJwtTokenGenerator jwtTokenGenerator, IMapper mapper)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _mapper = mapper;
        }

        async Task<LoginResponseDTO> IAuthService.Login(LoginRequestDTO requestDTO)
        {
            var users = await _userRepository.GetAllAsync();
            
            var user = users.FirstOrDefault(u => u.UserName.ToLower() == requestDTO.Username.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, requestDTO.Password);

            if (user == null || !isValid)
            {
                return new LoginResponseDTO { User = null, Token = "" };
            }

            UserDTO userDto =  _mapper.Map<UserDTO>(user);
                
                /*new()
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,

            };*/

            var roles = await _userManager.GetRolesAsync(user);

            string token = _jwtTokenGenerator.GenerateToken(user, roles);

            LoginResponseDTO response = new()
            {
                User = userDto,
                Token = token
            };

            return response;
        }

        async Task<string> IAuthService.Register(RegisterRequestDTO requestDTO)
        {
            ApplicationUser user = new ApplicationUser
            {
                Email = requestDTO.Email,
                UserName = requestDTO.Email,
                Name = requestDTO.Name,
                NormalizedUserName = requestDTO.Email.ToUpper()
            };

            try
            {
                var result = await _userManager.CreateAsync(user, requestDTO.Password);
                if (result.Succeeded)
                {
                    var users = await _userRepository.GetAllAsync();
                        
                     var userCreated = users.First(u => u.UserName == requestDTO.Email);

                    UserDTO userDto = new()
                    {
                        Id = userCreated.Id,
                        Email = userCreated.Email,
                        Name = userCreated.Name
                    };

                    return "";
                }

                return result.Errors.FirstOrDefault().Description;

            }
            catch (Exception ex)
            {
                return "Error encountered";
            }

        }
    }
}
