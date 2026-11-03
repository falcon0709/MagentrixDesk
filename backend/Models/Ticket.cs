namespace SupportDashboard.Api.Models;

public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? Assignee { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<TicketHistoryEntry> History { get; set; } = new List<TicketHistoryEntry>();
}
