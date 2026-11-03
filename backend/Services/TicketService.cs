using Microsoft.EntityFrameworkCore;
using SupportDashboard.Api.Data;
using SupportDashboard.Api.DTOs;
using SupportDashboard.Api.Models;

namespace SupportDashboard.Api.Services;

public class TicketService(AppDbContext db) : ITicketService
{
    public async Task<IReadOnlyList<TicketListItemDto>> GetTicketsAsync(TicketQuery query, CancellationToken ct = default)
    {
        var tickets = db.Tickets.AsNoTracking().AsQueryable();

        if (query.Status.HasValue)
        {
            tickets = tickets.Where(t => t.Status == query.Status.Value);
        }

        if (query.Priority.HasValue)
        {
            tickets = tickets.Where(t => t.Priority == query.Priority.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Assignee))
        {
            tickets = tickets.Where(t => t.Assignee == query.Assignee);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            tickets = tickets.Where(t =>
                t.Title.ToLower().Contains(term) ||
                t.Description.ToLower().Contains(term) ||
                t.CustomerName.ToLower().Contains(term) ||
                t.CustomerEmail.ToLower().Contains(term));
        }

        if (query.NeedsAttention)
        {
            var agingCutoff = DateTime.UtcNow.AddDays(-3);
            tickets = tickets.Where(t =>
                (t.Status == TicketStatus.Open || t.Status == TicketStatus.InProgress) &&
                (t.Priority == TicketPriority.High || t.Assignee == null || t.CreatedAt <= agingCutoff));
        }

        tickets = (query.SortBy?.ToLowerInvariant(), query.SortDir?.ToLowerInvariant()) switch
        {
            ("priority", "asc") => tickets.OrderBy(t => t.Priority).ThenByDescending(t => t.CreatedAt),
            ("priority", _) => tickets.OrderByDescending(t => t.Priority).ThenByDescending(t => t.CreatedAt),
            ("status", "asc") => tickets.OrderBy(t => t.Status).ThenByDescending(t => t.CreatedAt),
            ("status", _) => tickets.OrderByDescending(t => t.Status).ThenByDescending(t => t.CreatedAt),
            ("updatedat", "asc") => tickets.OrderBy(t => t.UpdatedAt),
            ("updatedat", _) => tickets.OrderByDescending(t => t.UpdatedAt),
            ("createdat", "asc") => tickets.OrderBy(t => t.CreatedAt),
            _ => tickets.OrderByDescending(t => t.CreatedAt)
        };

        var now = DateTime.UtcNow;
        return await tickets
            .Select(t => new TicketListItemDto(
                t.Id,
                t.Title,
                t.Description,
                t.Status,
                t.Priority,
                t.CustomerName,
                t.CustomerEmail,
                t.Assignee,
                t.CreatedAt,
                t.UpdatedAt,
                t.Comments.Count,
                (int)(now - t.CreatedAt).TotalDays
            ))
            .ToListAsync(ct);
    }

    public async Task<TicketDetailDto?> GetTicketAsync(int id, CancellationToken ct = default)
    {
        var ticket = await db.Tickets
            .AsNoTracking()
            .Include(t => t.Comments.OrderBy(c => c.CreatedAt))
            .Include(t => t.History.OrderByDescending(h => h.CreatedAt))
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        return ticket is null ? null : MapDetail(ticket);
    }

    public async Task<TicketDetailDto> CreateTicketAsync(CreateTicketRequest request, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var ticket = new Ticket
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Priority = request.Priority,
            Status = TicketStatus.Open,
            CustomerName = request.CustomerName.Trim(),
            CustomerEmail = request.CustomerEmail.Trim(),
            Assignee = string.IsNullOrWhiteSpace(request.Assignee) ? null : request.Assignee.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        ticket.History.Add(new TicketHistoryEntry
        {
            Action = "Created",
            Actor = "Support Manager",
            CreatedAt = now
        });

        db.Tickets.Add(ticket);
        await db.SaveChangesAsync(ct);

        return (await GetTicketAsync(ticket.Id, ct))!;
    }

    public async Task<TicketDetailDto?> UpdateTicketAsync(int id, UpdateTicketRequest request, CancellationToken ct = default)
    {
        var ticket = await db.Tickets.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (ticket is null)
        {
            return null;
        }

        var actor = string.IsNullOrWhiteSpace(request.Actor) ? "Support Manager" : request.Actor.Trim();
        var now = DateTime.UtcNow;

        TrackChange(ticket, "Title", ticket.Title, request.Title.Trim(), actor, now);
        TrackChange(ticket, "Description", ticket.Description, request.Description.Trim(), actor, now);
        TrackChange(ticket, "Status", ticket.Status.ToString(), request.Status.ToString(), actor, now);
        TrackChange(ticket, "Priority", ticket.Priority.ToString(), request.Priority.ToString(), actor, now);
        TrackChange(ticket, "CustomerName", ticket.CustomerName, request.CustomerName.Trim(), actor, now);
        TrackChange(ticket, "CustomerEmail", ticket.CustomerEmail, request.CustomerEmail.Trim(), actor, now);
        TrackChange(
            ticket,
            "Assignee",
            ticket.Assignee,
            string.IsNullOrWhiteSpace(request.Assignee) ? null : request.Assignee.Trim(),
            actor,
            now);

        ticket.Title = request.Title.Trim();
        ticket.Description = request.Description.Trim();
        ticket.Status = request.Status;
        ticket.Priority = request.Priority;
        ticket.CustomerName = request.CustomerName.Trim();
        ticket.CustomerEmail = request.CustomerEmail.Trim();
        ticket.Assignee = string.IsNullOrWhiteSpace(request.Assignee) ? null : request.Assignee.Trim();
        ticket.UpdatedAt = now;

        await db.SaveChangesAsync(ct);
        return await GetTicketAsync(id, ct);
    }

    public async Task<bool> DeleteTicketAsync(int id, CancellationToken ct = default)
    {
        var ticket = await db.Tickets.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (ticket is null)
        {
            return false;
        }

        db.Tickets.Remove(ticket);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<CommentDto?> AddCommentAsync(int ticketId, CreateCommentRequest request, CancellationToken ct = default)
    {
        var ticket = await db.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId, ct);
        if (ticket is null)
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var author = string.IsNullOrWhiteSpace(request.Author) ? "Support Agent" : request.Author.Trim();
        var comment = new Comment
        {
            TicketId = ticketId,
            Text = request.Text.Trim(),
            Author = author,
            CreatedAt = now
        };

        ticket.UpdatedAt = now;
        ticket.History.Add(new TicketHistoryEntry
        {
            Action = "CommentAdded",
            Field = "Comment",
            NewValue = Truncate(request.Text.Trim(), 120),
            Actor = author,
            CreatedAt = now
        });

        db.Comments.Add(comment);
        await db.SaveChangesAsync(ct);

        return new CommentDto(comment.Id, comment.TicketId, comment.Text, comment.Author, comment.CreatedAt);
    }

    public async Task<DashboardStatsDto> GetStatsAsync(CancellationToken ct = default)
    {
        var tickets = await db.Tickets.AsNoTracking().ToListAsync(ct);
        var startOfDay = DateTime.UtcNow.Date;
        var agingCutoff = DateTime.UtcNow.AddDays(-3);

        return new DashboardStatsDto(
            tickets.Count,
            tickets.Count(t => t.Status == TicketStatus.Open),
            tickets.Count(t => t.Status == TicketStatus.InProgress),
            tickets.Count(t => t.Status == TicketStatus.Resolved),
            tickets.Count(t => t.Status == TicketStatus.Closed),
            tickets.Count(t =>
                t.Priority == TicketPriority.High &&
                (t.Status == TicketStatus.Open || t.Status == TicketStatus.InProgress)),
            tickets.Count(t =>
                t.Assignee == null &&
                (t.Status == TicketStatus.Open || t.Status == TicketStatus.InProgress) &&
                t.CreatedAt <= agingCutoff),
            tickets.Count(t => t.UpdatedAt >= startOfDay)
        );
    }

    private static TicketDetailDto MapDetail(Ticket ticket)
    {
        var age = (int)(DateTime.UtcNow - ticket.CreatedAt).TotalDays;
        return new TicketDetailDto(
            ticket.Id,
            ticket.Title,
            ticket.Description,
            ticket.Status,
            ticket.Priority,
            ticket.CustomerName,
            ticket.CustomerEmail,
            ticket.Assignee,
            ticket.CreatedAt,
            ticket.UpdatedAt,
            age,
            ticket.Comments
                .Select(c => new CommentDto(c.Id, c.TicketId, c.Text, c.Author, c.CreatedAt))
                .ToList(),
            ticket.History
                .Select(h => new TicketHistoryDto(h.Id, h.Action, h.Field, h.OldValue, h.NewValue, h.Actor, h.CreatedAt))
                .ToList()
        );
    }

    private static void TrackChange(
        Ticket ticket,
        string field,
        string? oldValue,
        string? newValue,
        string actor,
        DateTime at)
    {
        if (string.Equals(oldValue, newValue, StringComparison.Ordinal))
        {
            return;
        }

        ticket.History.Add(new TicketHistoryEntry
        {
            Action = "Updated",
            Field = field,
            OldValue = Truncate(oldValue, 500),
            NewValue = Truncate(newValue, 500),
            Actor = actor,
            CreatedAt = at
        });
    }

    private static string? Truncate(string? value, int max)
    {
        if (value is null)
        {
            return null;
        }

        return value.Length <= max ? value : value[..max];
    }
}
