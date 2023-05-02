using System.ComponentModel.DataAnnotations;

namespace Sat.Recruitment.Core.Domain
{
    public class User
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "The name is required")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The email is required")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The address is required")]
        public string Address { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The phone is required")]
        public string Phone { get; set; }

        public UserTypes UserType { get; set; }

        public decimal Money { get; set; }
    }
}
