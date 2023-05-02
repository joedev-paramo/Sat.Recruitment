using System.ComponentModel.DataAnnotations;
using Sat.Recruitment.Core.Domain;
using Sat.Recruitment.Data;
using Sat.Recruitment.Services.Responses;

namespace Sat.Recruitment.Services
{
    /// <summary>
    /// The UsersService class provides an implementation of IUserService for managing user data,
    /// such as creating a new user and retrieving all users. It works with the IUsersRepo interface
    /// for data persistence.
    /// </summary>
    public class UsersService : IUsersService
    {
        private readonly IUsersRepo userRepo;

        public UsersService(IUsersRepo userRepo)
        {
            this.userRepo = userRepo;
        }

        /// <summary>
        /// Creates a new user by verifying the email format, normalizing the email address, checking for duplicates,
        /// and adding the user to the repository. Returns a CreateUserResponse indicating the result of the operation.
        /// </summary>
        /// <param name="user">The user object containing the information to be added.</param>
        /// <returns>A CreateUserResponse object indicating the result of the user creation process.</returns>
        public async Task<CreateUserResponse> Create(User user)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user));

            // verify if the email format is ok
            if (!new EmailAddressAttribute().IsValid(user.Email))
            {
                return new CreateUserResponse() { State = CreateUserStates.WrongFormatEmail };
            }

            // normalize the email address
            user.Email = NormalizeEmailAddress(user.Email);

            // verify if the user is duplicated
            if (await userRepo.IsUserExisting(user))
            {
                return new CreateUserResponse() { State = CreateUserStates.UserAlreadyRegistered };
            }

            // bonus
            AddBonusOnUserCreation(user);

            // add the user to the repository
            await userRepo.AddUser(user);

            return new CreateUserResponse() { State = CreateUserStates.Ok };
        }

        /// <summary>
        /// Get all the users.
        /// </summary>
        /// <returns>The list of users.</returns>
        public async Task<List<User>> GetAll()
        {
            return await userRepo.GetAll();
        }

        /// <summary>
        /// Normalizes the email address of the given user by removing all '.' characters in the local part
        /// and removing all characters after (and including) the '+' character if it is present.
        /// </summary>
        /// <param name="email">The email address to be normalized.</param>
        /// <returns>The normalized email address as a string.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the provided email parameter is null.</exception>
        internal static string NormalizeEmailAddress(string email)
        {
            _ = email ?? throw new ArgumentNullException(nameof(email));

            var emailParts = email.Split('@', StringSplitOptions.RemoveEmptyEntries);

            // Get the local part (before the '@') and domain part (after the '@')
            string localPart  = emailParts[0];
            string domainPart = emailParts[1];

            // In the original code, the index of '+' was calculated before removing the '.' characters,
            // which could lead to an incorrect index when calling Remove(atIndex).
            // This updated code fixes the potential error by removing the '.' characters first.

            // Remove all '.' characters in the local part
            localPart = localPart.Replace(".", string.Empty);

            // Find the position of the '+' character in the modified local part
            int plusIndex = localPart.IndexOf("+", StringComparison.Ordinal);

            // If '+' is present, remove all characters from '+' to the end of the local part
            if (plusIndex >= 0)
            {
                localPart = localPart.Remove(plusIndex);
            }

            // Combine the modified local part and domain part
            var normalizedEmail = $"{localPart}@{domainPart}";

            return normalizedEmail;
        }

        /// <summary>
        /// Adds a bonus to the user's money based on their UserType and the initial amount of money.
        /// The following conditions apply:
        /// - If the UserType is Normal and the money is greater than 100, a 12% bonus is added.
        /// - If the UserType is Normal and the money is between 10 and 100, an 8% bonus is added.
        /// - If the UserType is SuperUser and the money is greater than 100, a 20% bonus is added.
        /// - If the UserType is Premium and the money is greater than 100, the money is doubled.
        /// </summary>
        /// <param name="user">The user object whose money will be updated with the bonus.</param>
        internal static void AddBonusOnUserCreation(User user)
        {
            decimal bonus = 0;

            switch (user.UserType)
            {
                case UserTypes.Normal:
                    if (user.Money > 100)
                    {
                        bonus = user.Money * 0.12m;
                    }
                    else if (user.Money > 10)
                    {
                        bonus = user.Money * 0.08m;
                    }

                    break;

                case UserTypes.SuperUser:
                    if (user.Money > 100)
                    {
                        bonus = user.Money * 0.20m;
                    }

                    break;

                case UserTypes.Premium:
                    if (user.Money > 100)
                    {
                        bonus = user.Money;
                    }

                    break;
            }

            user.Money += bonus;
        }
    }
}
