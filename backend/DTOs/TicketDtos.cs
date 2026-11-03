using System.ComponentModel.DataAnnotations;
using SupportDashboard.Api.Models;

namespace SupportDashboard.Api.DTOs;

public record TicketListItemDto(
    int Id,
    string Title,
    string Description,
    TicketStatus Status,
    TicketPriority Priority,
    string CustomerName,
    string CustomerEmail,
    string? Assignee,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int CommentCount,
    int AgeInDays
);

public record TicketDetailDto(
    int Id,
    string Title,
    string Description,
    TicketStatus Status,
    TicketPriority Priority,
    string CustomerName,
    string CustomerEmail,
    string? Assignee,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int AgeInDays,
    IReadOnlyList<CommentDto> Comments,
    IReadOnlyList<TicketHistoryDto> History
);

public record CommentDto(
    int Id,
    int TicketId,
    string Text,
    string Author,
    DateTime CreatedAt
);

public record TicketHistoryDto(
    int Id,
    string Action,
    string? Field,
    string? OldValue,
    string? NewValue,
    string Actor,
    DateTime CreatedAt
);

public class CreateTicketRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(4000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;

    [Required, MaxLength(120)]
    public string CustomerName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string CustomerEmail { get; set; } = string.Empty;

    [MaxLength(120)]
    public string? Assignee { get; set; }
}

public class UpdateTicketRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(4000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public TicketStatus Status { get; set; }

    [Required]
    public TicketPriority Priority { get; set; }

    [Required, MaxLength(120)]
    public string CustomerName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string CustomerEmail { get; set; } = string.Empty;

    [MaxLength(120)]
    public string? Assignee { get; set; }

    [MaxLength(120)]
    public string? Actor { get; set; }
}

public class CreateCommentRequest
{
    [Required, MaxLength(2000)]
    public string Text { get; set; } = string.Empty;

    [MaxLength(120)]
    public string Author { get; set; } = "Support Agent";
}

public class TicketQuery
{
    public TicketStatus? Status { get; set; }
    public TicketPriority? Priority { get; set; }
    public string? Search { get; set; }
    public string? Assignee { get; set; }
    public string? SortBy { get; set; } = "createdAt";
    public string? SortDir { get; set; } = "desc";
    public bool NeedsAttention { get; set; }
}

public record DashboardStatsDto(
    int Total,
    int Open,
    int InProgress,
    int Resolved,
    int Closed,
    int HighPriorityOpen,
    int AgingUnassigned,
    int UpdatedToday
);
