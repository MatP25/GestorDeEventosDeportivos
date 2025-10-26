namespace GestorEventosDeportivos.Modules.EmailVerification.Domain;

public class VerificationTokenModel
{
    public Guid UserId { get; set; }
    public Guid Token { get; set; }
    public DateTime ExpiryDate { get; set; }
}
