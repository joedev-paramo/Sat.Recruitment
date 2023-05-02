using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sat.Recruitment.Api.Infrastructure;
using Sat.Recruitment.Core.Domain;
using Sat.Recruitment.Services;

namespace Sat.Recruitment.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public partial class UsersController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IUsersService _usersService;

        private readonly List<User> _users = new List<User>();
        public UsersController(IUsersService userService, ILoggerManager logger)
        {
            _logger       = logger;
            _usersService = userService;
        }

        [HttpGet]
        public async Task<IEnumerable<User>> Get()
        {
            _logger.LogInformation("Entering Get method.");
            List<User> users = await _usersService.GetAll();
            return users;
        }

        [HttpPost]
        [Route("create-user")]
        public async Task<Result> CreateUser(User user)
        {
            var result = new Result();
            string errorId = DateTime.UtcNow.Ticks.ToString();

            try
            {
                var response = await _usersService.Create(user);

                switch (response.State)
                {
                    case CreateUserStates.Ok:
                        result.IsSuccess = true;
                        result.Errors    = string.Empty;
                        break;

                    case CreateUserStates.UserAlreadyRegistered:
                        result.IsSuccess = false;
                        result.Errors    = "The user is already registered. Cannot create again.";
                        break;

                    case CreateUserStates.WrongFormatEmail:
                        result.IsSuccess = false;
                        result.Errors    = "The email format is not valid. Please provide an valid email address.";
                        break;

                    default:
                        _logger.LogError($"{errorId} Tried to create user with an unexpected response. response.State: {response.State}");
                        result.IsSuccess = false;
                        result.Errors    = $"An internal error has occurred. ErrorId: {errorId}";
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{errorId} {ex}");
                result.IsSuccess = false;
                result.Errors    = $"An internal error has occurred. ErrorId: {errorId}";
            }

            return result;
        }
    }
}
