using Sat.Recruitment.Core.Domain;
using Sat.Recruitment.Services.Responses;

namespace Sat.Recruitment.Services
{
    public interface IUsersService
    {
        public Task<List<User>> GetAll();

        public Task<CreateUserResponse> Create(User user);
    }
}
