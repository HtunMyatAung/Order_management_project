using System.Threading.Tasks;
namespace IdentityDemo.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);

    }
}
