using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using UsersAuthorization.Application.DTO;
using UsersAuthorization.Application.Interfaces;
using UsersAuthorization.Application.Service;
using UsersAuthorization.Domain.Entities;
using UsersAuthorization.Infrastructure.Repository;
using Xunit;

namespace UsersAuthorization.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IRepository<ApplicationUser>> _userRepositoryMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole<int>>> _roleManagerMock;
        private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly IAuthService _authService;

        public AuthServiceTests() 
        { 
            _userRepositoryMock = new Mock<IRepository<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object, null,null,null,null,null,null,null,null);
            _roleManagerMock = new Mock<RoleManager<IdentityRole<int>>>(
                new Mock<IRoleStore<IdentityRole<int>>>().Object, null, null, null, null);
            _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
            _mapperMock = new Mock<IMapper>();

            _authService = new AuthService(_userRepositoryMock.Object, _userManagerMock.Object, _roleManagerMock.Object, _jwtTokenGeneratorMock.Object,_mapperMock.Object);
        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenCredentialAreValid()
        {
            //arrange
            var loginRequest = new LoginRequestDTO { Username = "testUser@test.com", Password = "Password123!" };
            var user = new ApplicationUser { Name = "testUser", UserName = "testUser@test.com", Email = "testUser@test.com" };
            var roles = new List<string> { "User" };

            _userRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<ApplicationUser> { user });
            _userManagerMock.Setup(um => um.CheckPasswordAsync(user, loginRequest.Password)).ReturnsAsync(true);
            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(roles);
            _jwtTokenGeneratorMock.Setup(jwt => jwt.GenerateToken(user, roles)).Returns("fake-jwt-token");
            _mapperMock.Setup(m => m.Map<UserDTO>(user)).Returns(new UserDTO { Id = user.Id, Email = user.Email, Name = user.Name });


            //act
            var result = await _authService.Login(loginRequest);

            //assert
            Assert.NotNull(result);
            Assert.Equal("fake-jwt-token", result.Token);
            Assert.Equal(user.Email, result.User.Email);
        }

        [Fact]
        public async Task Register_ShouldReturnSuccessMessage_WhenUserIsCreated()
        {
            //arrange
            var registerRequest = new RegisterRequestDTO { Email = "newuser@new.com", Password = "Password123!", Name = "User New" };
            var user = new ApplicationUser { UserName = registerRequest.Email, Email = registerRequest.Email, Name = registerRequest.Name };

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), registerRequest.Password)).ReturnsAsync(IdentityResult.Success);
            _userRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<ApplicationUser> { user });
            //act
            var result = await _authService.Register(registerRequest);
            //asset
            Assert.Equal("", result);
        }

        [Fact]
        public async Task Register_ShouldReturnErrorMessage_WhenUserCreationFails()
        {
            //arrange
            var registerRequest = new RegisterRequestDTO { Email = "newuser@new.com", Password = "password", Name = "User New" };
            var identityError = new IdentityError { Description = "User creation failed" };
            var identityResult = IdentityResult.Failed(identityError);

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), registerRequest.Password)).ReturnsAsync(identityResult);

            //act
            var result = await _authService.Register(registerRequest);

            //assert
            Assert.Equal("User creation failed", result);

        }
    }
}
