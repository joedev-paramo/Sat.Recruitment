using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Sat.Recruitment.Core.Domain;
using Sat.Recruitment.Data;
using Sat.Recruitment.Services;
using Xunit;

namespace Sat.Recruitment.Test
{
    public class UsersServiceTests
    {
        private readonly Mock<IUsersRepo> _usersRepoMock;
        private readonly UsersService _usersService;

        public UsersServiceTests()
        {
            _usersRepoMock = new Mock<IUsersRepo>();
            _usersService = new UsersService(_usersRepoMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Name = "John", Email = "john@example.com", Address = "123 Main St", Phone = "555-555-1234", UserType = UserTypes.Normal, Money = 50 },
                new User { Name = "Jane", Email = "jane@example.com", Address = "456 Oak St", Phone = "555-555-5678", UserType = UserTypes.SuperUser, Money = 150 }
            };

            _usersRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(users);

            // Act
            var result = await _usersService.GetAll();

            // Assert
            Assert.Equal(users, result);
        }

        [Fact]
        public async Task Create_NormalUser_ReturnsOk()
        {
            // Arrange
            var user = new User { Name = "John", Email = "john@example.com", Address = "123 Main St", Phone = "555-555-1234", UserType = UserTypes.Normal, Money = 50 };

            _usersRepoMock.Setup(repo => repo.IsUserExisting(It.IsAny<User>())).ReturnsAsync(false);
            _usersRepoMock.Setup(repo => repo.AddUser(It.IsAny<User>()));

            // Act
            var result = await _usersService.Create(user);

            // Assert
            Assert.Equal(CreateUserStates.Ok, result.State);
        }

        [Fact]
        public async Task Create_UserAlreadyExists_ReturnsUserAlreadyRegistered()
        {
            // Arrange
            var user = new User { Name = "John", Email = "john@example.com", Address = "123 Main St", Phone = "555-555-1234", UserType = UserTypes.Normal, Money = 50 };

            _usersRepoMock.Setup(repo => repo.IsUserExisting(It.IsAny<User>())).ReturnsAsync(true);

            // Act
            var result = await _usersService.Create(user);

            // Assert
            Assert.Equal(CreateUserStates.UserAlreadyRegistered, result.State);
        }

        [Fact]
        public async Task Create_InvalidEmail_ReturnsWrongFormatEmail()
        {
            // Arrange
            var user = new User { Name = "John", Email = "invalid-email", Address = "123 Main St", Phone = "555-555-1234", UserType = UserTypes.Normal, Money = 50 };

            // Act
            var result = await _usersService.Create(user);

            // Assert
            Assert.Equal(CreateUserStates.WrongFormatEmail, result.State);
        }
    }
}
