using System.ComponentModel.DataAnnotations;

namespace Fiorello.ViewModels.Account
{
    public class AccountLoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required,DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
