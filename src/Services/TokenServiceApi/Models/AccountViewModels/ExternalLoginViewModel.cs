using System.ComponentModel.DataAnnotations;

namespace MicroServicesOnDocker.Services.TokenServiceApi.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
