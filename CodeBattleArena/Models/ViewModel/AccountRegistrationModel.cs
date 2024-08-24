using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Models.ViewModel
{
    public class AccountRegistrationModel
    {
        [Required(ErrorMessage = "Fill in all the fields")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Fill in all the fields")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Fill in all the fields")]
        public string? Password { get; set; }
    }
}
