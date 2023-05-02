using Sat.Recruitment.Core.Domain;

namespace Sat.Recruitment.Data
{
    public interface IUsersRepo
    {
        public Task<List<User>> GetAll();

        public Task<bool> IsUserExisting(User user);

        public Task AddUser(User user);
    }
}
