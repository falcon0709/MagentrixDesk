namespace SupportDashboard.Api.Models;

public class Comment
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Ticket Ticket { get; set; } = null!;
}
