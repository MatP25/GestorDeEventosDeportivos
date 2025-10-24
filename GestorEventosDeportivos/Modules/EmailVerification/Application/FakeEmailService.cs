using Microsoft.Extensions.Logging;

namespace GestorEventosDeportivos.Modules.EmailVerification.Application;

public class FakeEmailService : IEmailService
{
    private readonly ILogger<FakeEmailService> _logger;

    public FakeEmailService(ILogger<FakeEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendVerificationEmailAsync(string toEmail, Guid token)
    {
        var url = $"/verify?token={token}";
        _logger.LogInformation("[FakeEmailService] Enviando verificación a {Email}: {Url}", toEmail, url);
        Console.WriteLine($"[FakeEmailService] Verificación para {toEmail}: {url}");
        return Task.CompletedTask;
    }
}
