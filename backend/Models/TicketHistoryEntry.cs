namespace SupportDashboard.Api.Models;

public class TicketHistoryEntry
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Field { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string Actor { get; set; } = "System";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Ticket Ticket { get; set; } = null!;
}
