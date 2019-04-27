using System.Threading.Tasks;

namespace MicroServicesOnDocker.Services.TokenServiceApi.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
