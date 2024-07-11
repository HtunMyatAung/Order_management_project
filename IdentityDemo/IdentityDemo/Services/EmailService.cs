using IdentityDemo.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Net.Sockets;
using System.Threading.Tasks;


public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
            var smtpSettings = _configuration.GetSection("Smtp");
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("uab zone", smtpSettings["From"]));
        message.To.Add(new MailboxAddress("Recipient Name", toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html")
        {
            Text = htmlMessage
        };

        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(smtpSettings["Host"], int.Parse(smtpSettings["Port"]), SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(smtpSettings["UserName"], smtpSettings["Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.HostNotFound || ex.SocketErrorCode == SocketError.NetworkUnreachable || ex.SocketErrorCode == SocketError.TimedOut)
        {
            Console.WriteLine($"Network Error: {ex.Message}");
            throw new Exception("Network error: Unable to reach the SMTP server. Please check your internet connection.");
        }
        catch (SmtpCommandException ex)
        {
            // Handle SMTP command errors
            Console.WriteLine($"SMTP Command Error: {ex.Message}");
            throw;
        }
        catch (SmtpProtocolException ex)
        {
            // Handle SMTP protocol errors
            Console.WriteLine($"SMTP Protocol Error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            // Handle other errors
            Console.WriteLine($"Unexpected Error: {ex.Message}");
            throw;
        }
    }
}
