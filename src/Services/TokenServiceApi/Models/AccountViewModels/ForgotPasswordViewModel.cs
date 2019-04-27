using System.ComponentModel.DataAnnotations;

namespace MicroServicesOnDocker.Services.TokenServiceApi.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
