using Sat.Recruitment.Core.Domain;
using Sat.Recruitment.Services;
using Xunit;

namespace Sat.Recruitment.Test
{
    public class AddBonusOnUserCreationTests
    {
        [Theory]
        [InlineData(UserTypes.Normal, 120, 134.4)]
        [InlineData(UserTypes.Normal, 50, 54)]
        [InlineData(UserTypes.Normal, 5, 5)]
        public void AddBonusOnUserCreation_NormalUser_MoneyIsUpdated(UserTypes userType, decimal initialMoney, decimal expectedMoney)
        {
            // Arrange
            User user = new User { UserType = userType, Money = initialMoney };

            // Act
            UsersService.AddBonusOnUserCreation(user);

            // Assert
            Assert.Equal(expectedMoney, user.Money);
        }

        [Theory]
        [InlineData(120, 144)]
        [InlineData(50, 50)]
        public void AddBonusOnUserCreation_SuperUser_MoneyIsUpdated(decimal initialMoney, decimal expectedMoney)
        {
            // Arrange
            User user = new User { UserType = UserTypes.SuperUser, Money = initialMoney };

            // Act
            UsersService.AddBonusOnUserCreation(user);

            // Assert
            Assert.Equal(expectedMoney, user.Money);
        }

        [Theory]
        [InlineData(120, 240)]
        [InlineData(50, 50)]
        public void AddBonusOnUserCreation_PremiumUser_MoneyIsUpdated(decimal initialMoney, decimal expectedMoney)
        {
            // Arrange
            User user = new User { UserType = UserTypes.Premium, Money = initialMoney };

            // Act
            UsersService.AddBonusOnUserCreation(user);

            // Assert
            Assert.Equal(expectedMoney, user.Money);
        }
    }
}
