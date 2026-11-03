using Microsoft.EntityFrameworkCore;
using SupportDashboard.Api.Models;

namespace SupportDashboard.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<TicketHistoryEntry> TicketHistory => Set<TicketHistoryEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).HasMaxLength(200).IsRequired();
            entity.Property(t => t.Description).HasMaxLength(4000).IsRequired();
            entity.Property(t => t.CustomerName).HasMaxLength(120).IsRequired();
            entity.Property(t => t.CustomerEmail).HasMaxLength(200).IsRequired();
            entity.Property(t => t.Assignee).HasMaxLength(120);
            entity.HasIndex(t => t.Status);
            entity.HasIndex(t => t.Priority);
            entity.HasIndex(t => t.CreatedAt);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Text).HasMaxLength(2000).IsRequired();
            entity.Property(c => c.Author).HasMaxLength(120).IsRequired();
            entity.HasOne(c => c.Ticket)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TicketHistoryEntry>(entity =>
        {
            entity.HasKey(h => h.Id);
            entity.Property(h => h.Action).HasMaxLength(80).IsRequired();
            entity.Property(h => h.Field).HasMaxLength(80);
            entity.Property(h => h.OldValue).HasMaxLength(500);
            entity.Property(h => h.NewValue).HasMaxLength(500);
            entity.Property(h => h.Actor).HasMaxLength(120).IsRequired();
            entity.HasOne(h => h.Ticket)
                .WithMany(t => t.History)
                .HasForeignKey(h => h.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
