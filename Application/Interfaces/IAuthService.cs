using UsersAuthorization.Application.DTO;

namespace UsersAuthorization.Application.Interfaces
{
    public interface IAuthService
    {
        Task<string> Register(RegisterRequestDTO requestDTO);
        Task<LoginResponseDTO> Login(LoginRequestDTO requestDTO);
    }
}
