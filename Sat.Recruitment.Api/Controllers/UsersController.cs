using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sat.Recruitment.Api.Infrastructure;
using Sat.Recruitment.Core.Domain;
using Sat.Recruitment.Services;

namespace Sat.Recruitment.Api.Controllers
{
    //public class Result
    //{
    //    public bool IsSuccess { get; set; }
    //    public string Errors { get; set; }
    //}

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

        //[HttpPost]
        //[Route("/create-user")]
        //public async Task<Result> CreateUser(string name, string email, string address, string phone, string userType, string money)
        //{
        //    _logger.LogInformation("Entering CreateUser method.");

        //    var errors = "";

        //    ValidateErrors(name, email, address, phone, ref errors);

        //    if (errors != null && errors != "")
        //        return new Result()
        //        {
        //            IsSuccess = false,
        //            Errors = errors
        //        };

        //    var newUser = new User
        //    {
        //        Name = name,
        //        Email = email,
        //        Address = address,
        //        Phone = phone,
        //        UserType = userType,
        //        Money = decimal.Parse(money)
        //    };

        //    if (newUser.UserType == "Normal")
        //    {
        //        if (decimal.Parse(money) > 100)
        //        {
        //            var percentage = Convert.ToDecimal(0.12);
        //            //If new user is normal and has more than USD100
        //            var gif = decimal.Parse(money) * percentage;
        //            newUser.Money = newUser.Money + gif;
        //        }
        //        if (decimal.Parse(money) < 100)
        //        {
        //            if (decimal.Parse(money) > 10)
        //            {
        //                var percentage = Convert.ToDecimal(0.8);
        //                var gif = decimal.Parse(money) * percentage;
        //                newUser.Money = newUser.Money + gif;
        //            }
        //        }
        //    }
        //    if (newUser.UserType == "SuperUser")
        //    {
        //        if (decimal.Parse(money) > 100)
        //        {
        //            var percentage = Convert.ToDecimal(0.20);
        //            var gif = decimal.Parse(money) * percentage;
        //            newUser.Money = newUser.Money + gif;
        //        }
        //    }
        //    if (newUser.UserType == "Premium")
        //    {
        //        if (decimal.Parse(money) > 100)
        //        {
        //            var gif = decimal.Parse(money) * 2;
        //            newUser.Money = newUser.Money + gif;
        //        }
        //    }


        //    var reader = ReadUsersFromFile();

        //    //Normalize email
        //    var aux = newUser.Email.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

        //    var atIndex = aux[0].IndexOf("+", StringComparison.Ordinal);

        //    aux[0] = atIndex < 0 ? aux[0].Replace(".", "") : aux[0].Replace(".", "").Remove(atIndex);

        //    newUser.Email = string.Join("@", new string[] { aux[0], aux[1] });

        //    while (reader.Peek() >= 0)
        //    {
        //        var line = reader.ReadLineAsync().Result;
        //        var user = new User
        //        {
        //            Name = line.Split(',')[0].ToString(),
        //            Email = line.Split(',')[1].ToString(),
        //            Phone = line.Split(',')[2].ToString(),
        //            Address = line.Split(',')[3].ToString(),
        //            UserType = line.Split(',')[4].ToString(),
        //            Money = decimal.Parse(line.Split(',')[5].ToString()),
        //        };
        //        _users.Add(user);
        //    }
        //    reader.Close();
        //    try
        //    {
        //        var isDuplicated = false;
        //        foreach (var user in _users)
        //        {
        //            if (user.Email == newUser.Email
        //                ||
        //                user.Phone == newUser.Phone)
        //            {
        //                isDuplicated = true;
        //            }
        //            else if (user.Name == newUser.Name)
        //            {
        //                if (user.Address == newUser.Address)
        //                {
        //                    isDuplicated = true;
        //                    throw new Exception("User is duplicated");
        //                }

        //            }
        //        }

        //        if (!isDuplicated)
        //        {
        //            Debug.WriteLine("User Created");

        //            return new Result()
        //            {
        //                IsSuccess = true,
        //                Errors = "User Created"
        //            };
        //        }
        //        else
        //        {
        //            Debug.WriteLine("The user is duplicated");

        //            return new Result()
        //            {
        //                IsSuccess = false,
        //                Errors = "The user is duplicated"
        //            };
        //        }
        //    }
        //    catch
        //    {
        //        Debug.WriteLine("The user is duplicated");
        //        return new Result()
        //        {
        //            IsSuccess = false,
        //            Errors = "The user is duplicated"
        //        };
        //    }

        //    return new Result()
        //    {
        //        IsSuccess = true,
        //        Errors = "User Created"
        //    };
        //}

        //Validate errors
        //private void ValidateErrors(string name, string email, string address, string phone, ref string errors)
        //{
        //    if (name == null)
        //        //Validate if Name is null
        //        errors = "The name is required";
        //    if (email == null)
        //        //Validate if Email is null
        //        errors = errors + " The email is required";
        //    if (address == null)
        //        //Validate if Address is null
        //        errors = errors + " The address is required";
        //    if (phone == null)
        //        //Validate if Phone is null
        //        errors = errors + " The phone is required";
        //}
    }
    //public class User
    //{
    //    public string Name { get; set; }
    //    public string Email { get; set; }
    //    public string Address { get; set; }
    //    public string Phone { get; set; }
    //    public string UserType { get; set; }
    //    public decimal Money { get; set; }
    //}
}
