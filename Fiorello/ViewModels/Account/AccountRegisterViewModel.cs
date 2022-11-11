using System.ComponentModel.DataAnnotations;

namespace Fiorello.ViewModels.Account
{
    public class AccountRegisterViewModel
    {
        [Required,MaxLength(50)]
        public string Fullname { get; set; }
        [Required,MaxLength(50)]
        public string Username { get; set; }

        [Required,DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required,DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, DataType(DataType.Password), Display(Name = "Confirm Password"), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
