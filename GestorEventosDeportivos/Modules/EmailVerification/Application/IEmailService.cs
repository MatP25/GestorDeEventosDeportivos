namespace GestorEventosDeportivos.Modules.EmailVerification.Application;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string toEmail, Guid token);
}
