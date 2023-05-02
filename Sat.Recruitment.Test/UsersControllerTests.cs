using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Sat.Recruitment.Api.Controllers;
using Sat.Recruitment.Api.Infrastructure;
using Sat.Recruitment.Core.Domain;
using Sat.Recruitment.Services.Responses;
using Sat.Recruitment.Services;
using Xunit;

namespace Sat.Recruitment.Test
{
    public class UsersControllerTests
    {
        private readonly Mock<IUsersService> _usersServiceMock;
        private readonly Mock<ILoggerManager> _loggerMock;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _usersServiceMock = new Mock<IUsersService>();
            _loggerMock = new Mock<ILoggerManager>();
            _controller = new UsersController(_usersServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Get_ReturnsListOfUsers()
        {
            // Arrange
            var expectedUsers = new List<User>
            {
                new User { Name = "John Doe", Email = "john.doe@example.com" },
                new User { Name = "Jane Doe", Email = "jane.doe@example.com" },
            };
            _usersServiceMock.Setup(service => service.GetAll()).ReturnsAsync(expectedUsers);

            // Act
            var result = await _controller.Get();

            // Assert
            _usersServiceMock.Verify(service => service.GetAll(), Times.Once);
            Assert.Equal(expectedUsers, result);
        }

        [Fact]
        public async Task CreateUser_ReturnsResultWithSuccess_WhenUserIsCreated()
        {
            // Arrange
            var newUser = new User { Name = "John Doe", Email = "john.doe@example.com" };
            _usersServiceMock.Setup(service => service.Create(It.IsAny<User>())).ReturnsAsync(new CreateUserResponse { State = CreateUserStates.Ok });

            // Act
            var result = await _controller.CreateUser(newUser);

            // Assert
            _usersServiceMock.Verify(service => service.Create(It.IsAny<User>()), Times.Once);
            Assert.True(result.IsSuccess);
            Assert.Equal(string.Empty, result.Errors);
        }

        [Fact]
        public async Task CreateUser_ReturnsResultWithFailure_WhenUserAlreadyRegistered()
        {
            // Arrange
            var newUser = new User { Name = "John Doe", Email = "john.doe@example.com" };
            _usersServiceMock.Setup(service => service.Create(It.IsAny<User>())).ReturnsAsync(new CreateUserResponse { State = CreateUserStates.UserAlreadyRegistered });

            // Act
            var result = await _controller.CreateUser(newUser);

            // Assert
            _usersServiceMock.Verify(service => service.Create(It.IsAny<User>()), Times.Once);
            Assert.False(result.IsSuccess);
            Assert.Equal("The user is already registered. Cannot create again.", result.Errors);
        }

        [Fact]
        public async Task CreateUser_ReturnsResultWithFailure_WhenWrongFormatEmail()
        {
            // Arrange
            var newUser = new User { Name = "John Doe", Email = "john.doe@example.com" };
            _usersServiceMock.Setup(service => service.Create(It.IsAny<User>())).ReturnsAsync(new CreateUserResponse { State = CreateUserStates.WrongFormatEmail });

            // Act
            var result = await _controller.CreateUser(newUser);

            // Assert
            _usersServiceMock.Verify(service => service.Create(It.IsAny<User>()), Times.Once);
            Assert.False(result.IsSuccess);
            Assert.Equal("The email format is not valid. Please provide an valid email address.", result.Errors);
        }
    }
}
