using System.Security.Claims;
using GestorEventosDeportivos.Modules.EmailVerification.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace GestorEventosDeportivos.Modules.EmailVerification.Application;

public class VerificationUserService
{
    private static readonly List<ApplicationUser> _users = new();
    private static readonly List<VerificationTokenModel> _tokens = new();

    private readonly IEmailService _emailService;

    public VerificationUserService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public Task<ApplicationUser> RegisterAsync(string email)
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = email.Trim() };
        _users.Add(user);

        var token = new VerificationTokenModel
        {
            UserId = user.Id,
            Token = Guid.NewGuid(),
            ExpiryDate = DateTime.UtcNow.AddMinutes(30)
        };
        _tokens.Add(token);

        // Simular el envio del email
        _ = _emailService.SendVerificationEmailAsync(user.Email, token.Token);

        return Task.FromResult(user);
    }

    public Task<(bool valid, Guid userId)> ValidateTokenAsync(Guid token)
    {
        var t = _tokens.FirstOrDefault(x => x.Token == token);
        if (t is null) return Task.FromResult((false, Guid.Empty));
        if (t.ExpiryDate < DateTime.UtcNow) return Task.FromResult((false, Guid.Empty));
        // Consumir el token
        _tokens.Remove(t);
        return Task.FromResult((true, t.UserId));
    }

    public Task<ApplicationUser?> GetUserAsync(Guid userId)
    {
        return Task.FromResult(_users.FirstOrDefault(u => u.Id == userId));
    }

    // Consumir token por usuario (simulación de "click" en enlace)
    public Task<bool> ConsumeTokenForUserAsync(Guid userId)
    {
        var t = _tokens.FirstOrDefault(x => x.UserId == userId);
        if (t is null) return Task.FromResult(false);
        if (t.ExpiryDate < DateTime.UtcNow) return Task.FromResult(false);
        _tokens.Remove(t);
        return Task.FromResult(true);
    }

    // Regenera la cookie con el claim EmailVerified = true
    public async Task SignInWithVerifiedClaimAsync(Guid userId, HttpContext http)
    {
        var user = await GetUserAsync(userId);
        if (user is null) return;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("EmailVerified", "true")
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }
}
