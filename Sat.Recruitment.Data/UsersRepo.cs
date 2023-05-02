using System.Globalization;
using Sat.Recruitment.Core.Domain;

namespace Sat.Recruitment.Data
{
    /// <summary>
    /// The UsersRepo class provides methods for managing user data in the Users.txt file, such as reading all users,
    /// checking if a user exists, and adding a new user. This class implements the IUsersRepo interface.
    /// </summary>
    public class UsersRepo : IUsersRepo
    {
        private string filePath = @"./Files/Users.txt";

        /// <summary>
        /// Get all the users from the repository.
        /// </summary>
        /// <returns>The list of users.</returns>
        public async Task<List<User>> GetAll()
        {
            return await ReadUsersFromFile();
        }

        /// <summary>
        /// Determines if a user with the same email, phone, name, or address already exists in the list of users
        /// read from the file. The user is considered to exist if either the email or phone matches another user,
        /// or if both the name and address match another user.
        /// </summary>
        /// <param name="user">The user object containing the information to be checked for existence.</param>
        /// <returns>A boolean value indicating whether a user with the same email, phone, name, or address already exists.</returns>
        public async Task<bool> IsUserExisting(User user)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user));

            user.Name     = user.Name.Trim();
            user.Email    = user.Email.Trim();
            user.Address  = user.Address.Trim();
            user.Phone    = user.Phone.Trim();

            bool userExists;

            var users = await ReadUsersFromFile();

            // this code mantains the same logic from the original code:
            // if email or phone is the same, the user already exists,
            // else, if name and address is the same, the user already exists.
            userExists = users.Any(x => x.Email == user.Email || x.Phone == user.Phone);

            if (!userExists)
            {
                userExists = users.Any(x => x.Name == user.Name && x.Address == user.Address);
            }

            return userExists;
        }

        /// <summary>
        /// Asynchronously adds a new user to the repository.
        /// </summary>
        /// <param name="user">The user object to be added to the repository.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task AddUser(User user)
        {
            user.Name    = user.Name.Replace(",", string.Empty);
            user.Phone   = user.Phone.Replace(",", string.Empty);
            user.Address = user.Address.Replace(",", string.Empty);

            // CultureInfo.InvariantCulture is used to save the decimal value with . instead of ,
            string userLine = $"{user.Name},{user.Email},{user.Phone},{user.Address},{user.UserType},{user.Money.ToString(CultureInfo.InvariantCulture)}";
            await File.AppendAllTextAsync(filePath, Environment.NewLine + userLine);
        }

        private async Task<List<User>> ReadUsersFromFile()
        {
            List<User> users = new List<User>();

            if (File.Exists(filePath))
            {
                string[] lines = await File.ReadAllLinesAsync(filePath);

                foreach (string line in lines)
                {
                    string[] fields = line.Split(',');

                    if (fields.Length == 6)
                    {
                        var user = new User()
                        {
                            Name     = fields[0].Trim(),
                            Email    = fields[1].Trim(),
                            Phone    = fields[2].Trim(),
                            Address  = fields[3].Trim(),
                            UserType = Enum.TryParse<UserTypes>(fields[4].Trim(), out var userType) ? userType : UserTypes.Normal,
                            Money    = decimal.TryParse(fields[5].Trim(), out var money) ? money : 0
                        };

                        users.Add(user);
                    }
                }
            }

            return users;
        }
    }
}
