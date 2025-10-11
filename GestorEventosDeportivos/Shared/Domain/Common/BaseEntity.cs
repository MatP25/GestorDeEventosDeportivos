namespace GestorEventosDeportivos.Shared.Domain.Common;

public abstract class BaseEntity<TId>
{
    public TId Id { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public interface IAggregateRoot { }
