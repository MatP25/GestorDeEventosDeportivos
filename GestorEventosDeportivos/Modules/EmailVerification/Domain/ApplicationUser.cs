namespace GestorEventosDeportivos.Modules.EmailVerification.Domain;

public class ApplicationUser
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
}
