using SupportDashboard.Api.Models;

namespace SupportDashboard.Api.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Tickets.Any())
        {
            return;
        }

        var now = DateTime.UtcNow;

        var tickets = new List<Ticket>
        {
            new()
            {
                Title = "Cannot reset password after SSO migration",
                Description = "Customer is stuck on the forgot-password flow after we enabled SSO. Reset emails never arrive.",
                Status = TicketStatus.Open,
                Priority = TicketPriority.High,
                CustomerName = "Ava Chen",
                CustomerEmail = "ava.chen@northwind.io",
                Assignee = null,
                CreatedAt = now.AddDays(-5).AddHours(-2),
                UpdatedAt = now.AddDays(-5).AddHours(-2)
            },
            new()
            {
                Title = "Dashboard charts load slowly on Safari",
                Description = "Reports take 8–12 seconds to render on Safari 17. Chrome is fine. Happens for accounts with >50k events.",
                Status = TicketStatus.InProgress,
                Priority = TicketPriority.Medium,
                CustomerName = "Marcus Lee",
                CustomerEmail = "mlee@brightpath.com",
                Assignee = "Jordan Hale",
                CreatedAt = now.AddDays(-3).AddHours(-6),
                UpdatedAt = now.AddHours(-4)
            },
            new()
            {
                Title = "Invoice PDF missing tax line for EU customers",
                Description = "EU invoices generated this month omit VAT. Finance needs a corrected export before month-end close.",
                Status = TicketStatus.Open,
                Priority = TicketPriority.High,
                CustomerName = "Sofia Berg",
                CustomerEmail = "sofia.berg@lumen.eu",
                Assignee = "Priya Nair",
                CreatedAt = now.AddDays(-1).AddHours(-1),
                UpdatedAt = now.AddHours(-1)
            },
            new()
            {
                Title = "Webhook retries flooding customer endpoint",
                Description = "After a brief outage, webhook retries are hammering their endpoint at ~200/min. Need throttle or pause.",
                Status = TicketStatus.InProgress,
                Priority = TicketPriority.High,
                CustomerName = "Devon Walsh",
                CustomerEmail = "devon@stackyard.dev",
                Assignee = "Jordan Hale",
                CreatedAt = now.AddHours(-10),
                UpdatedAt = now.AddHours(-2)
            },
            new()
            {
                Title = "Request: export tickets to CSV",
                Description = "Manager wants a weekly CSV export of closed tickets for QBR prep. Nice-to-have but recurring ask.",
                Status = TicketStatus.Open,
                Priority = TicketPriority.Low,
                CustomerName = "Nina Patel",
                CustomerEmail = "nina.patel@orbitly.com",
                Assignee = null,
                CreatedAt = now.AddDays(-8),
                UpdatedAt = now.AddDays(-7)
            },
            new()
            {
                Title = "Mobile app crash on iOS 18 login",
                Description = "Crash on Face ID prompt for ~15% of iOS 18 users. Workaround: password login works.",
                Status = TicketStatus.Resolved,
                Priority = TicketPriority.High,
                CustomerName = "Chris Ortega",
                CustomerEmail = "chris.o@fieldkit.app",
                Assignee = "Sam Ortiz",
                CreatedAt = now.AddDays(-6),
                UpdatedAt = now.AddDays(-1)
            },
            new()
            {
                Title = "Clarification on API rate limits",
                Description = "Customer asking whether rate limits are per key or per org. Docs are ambiguous.",
                Status = TicketStatus.Closed,
                Priority = TicketPriority.Low,
                CustomerName = "Harper Quinn",
                CustomerEmail = "harper@quillstack.io",
                Assignee = "Priya Nair",
                CreatedAt = now.AddDays(-12),
                UpdatedAt = now.AddDays(-10)
            },
            new()
            {
                Title = "Wrong timezone on scheduled reports",
                Description = "Reports scheduled for 9am EST are firing at 9am UTC after last release.",
                Status = TicketStatus.Open,
                Priority = TicketPriority.Medium,
                CustomerName = "Elena Rossi",
                CustomerEmail = "elena.rossi@cobalt.co",
                Assignee = "Sam Ortiz",
                CreatedAt = now.AddDays(-2).AddHours(-3),
                UpdatedAt = now.AddDays(-2)
            },
            new()
            {
                Title = "Unable to invite teammate — seat limit error",
                Description = "Enterprise plan shows 0 seats remaining even though billing page shows 12 seats available.",
                Status = TicketStatus.InProgress,
                Priority = TicketPriority.Medium,
                CustomerName = "Tomás Alvarez",
                CustomerEmail = "talvarez@rivermark.com",
                Assignee = "Priya Nair",
                CreatedAt = now.AddDays(-4),
                UpdatedAt = now.AddHours(-6)
            },
            new()
            {
                Title = "Dark mode contrast issues in ticket list",
                Description = "Priority badges fail WCAG AA on dark backgrounds. Accessibility review requested.",
                Status = TicketStatus.Resolved,
                Priority = TicketPriority.Low,
                CustomerName = "Jamie Fox",
                CustomerEmail = "jamie.fox@a11ylab.org",
                Assignee = "Jordan Hale",
                CreatedAt = now.AddDays(-9),
                UpdatedAt = now.AddDays(-3)
            }
        };

        db.Tickets.AddRange(tickets);
        db.SaveChanges();

        var comments = new List<Comment>
        {
            new()
            {
                TicketId = tickets[1].Id,
                Author = "Jordan Hale",
                Text = "Reproduced on Safari 17.4. Profiling points to a main-thread chart layout thrash.",
                CreatedAt = now.AddDays(-2)
            },
            new()
            {
                TicketId = tickets[1].Id,
                Author = "Marcus Lee",
                Text = "Happy to jump on a call with a sample account if needed.",
                CreatedAt = now.AddDays(-1).AddHours(-5)
            },
            new()
            {
                TicketId = tickets[3].Id,
                Author = "Jordan Hale",
                Text = "Paused outbound webhooks for this tenant. Investigating retry backlog size.",
                CreatedAt = now.AddHours(-3)
            },
            new()
            {
                TicketId = tickets[5].Id,
                Author = "Sam Ortiz",
                Text = "Hotfix shipped in 2.14.3. Asking customer to confirm on latest build.",
                CreatedAt = now.AddDays(-1).AddHours(-2)
            },
            new()
            {
                TicketId = tickets[2].Id,
                Author = "Priya Nair",
                Text = "Pulled sample invoices — VAT field is null for DE/FR orgs created after Oct 1.",
                CreatedAt = now.AddHours(-5)
            }
        };

        db.Comments.AddRange(comments);

        var history = new List<TicketHistoryEntry>
        {
            new()
            {
                TicketId = tickets[0].Id,
                Action = "Created",
                Actor = "System",
                CreatedAt = tickets[0].CreatedAt
            },
            new()
            {
                TicketId = tickets[1].Id,
                Action = "Created",
                Actor = "System",
                CreatedAt = tickets[1].CreatedAt
            },
            new()
            {
                TicketId = tickets[1].Id,
                Action = "Updated",
                Field = "Status",
                OldValue = "Open",
                NewValue = "InProgress",
                Actor = "Jordan Hale",
                CreatedAt = now.AddDays(-2).AddHours(-1)
            },
            new()
            {
                TicketId = tickets[1].Id,
                Action = "Updated",
                Field = "Assignee",
                OldValue = null,
                NewValue = "Jordan Hale",
                Actor = "Jordan Hale",
                CreatedAt = now.AddDays(-2).AddHours(-1)
            },
            new()
            {
                TicketId = tickets[3].Id,
                Action = "Created",
                Actor = "System",
                CreatedAt = tickets[3].CreatedAt
            },
            new()
            {
                TicketId = tickets[3].Id,
                Action = "Updated",
                Field = "Status",
                OldValue = "Open",
                NewValue = "InProgress",
                Actor = "Jordan Hale",
                CreatedAt = now.AddHours(-8)
            },
            new()
            {
                TicketId = tickets[5].Id,
                Action = "Updated",
                Field = "Status",
                OldValue = "InProgress",
                NewValue = "Resolved",
                Actor = "Sam Ortiz",
                CreatedAt = now.AddDays(-1)
            },
            new()
            {
                TicketId = tickets[6].Id,
                Action = "Updated",
                Field = "Status",
                OldValue = "Resolved",
                NewValue = "Closed",
                Actor = "Priya Nair",
                CreatedAt = now.AddDays(-10)
            }
        };

        foreach (var ticket in tickets)
        {
            if (!history.Any(h => h.TicketId == ticket.Id && h.Action == "Created"))
            {
                history.Add(new TicketHistoryEntry
                {
                    TicketId = ticket.Id,
                    Action = "Created",
                    Actor = "System",
                    CreatedAt = ticket.CreatedAt
                });
            }
        }

        db.TicketHistory.AddRange(history);
        db.SaveChanges();
    }
}
