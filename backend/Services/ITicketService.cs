using SupportDashboard.Api.DTOs;

namespace SupportDashboard.Api.Services;

public interface ITicketService
{
    Task<IReadOnlyList<TicketListItemDto>> GetTicketsAsync(TicketQuery query, CancellationToken ct = default);
    Task<TicketDetailDto?> GetTicketAsync(int id, CancellationToken ct = default);
    Task<TicketDetailDto> CreateTicketAsync(CreateTicketRequest request, CancellationToken ct = default);
    Task<TicketDetailDto?> UpdateTicketAsync(int id, UpdateTicketRequest request, CancellationToken ct = default);
    Task<bool> DeleteTicketAsync(int id, CancellationToken ct = default);
    Task<CommentDto?> AddCommentAsync(int ticketId, CreateCommentRequest request, CancellationToken ct = default);
    Task<DashboardStatsDto> GetStatsAsync(CancellationToken ct = default);
}
