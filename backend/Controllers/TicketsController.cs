using Microsoft.AspNetCore.Mvc;
using SupportDashboard.Api.DTOs;
using SupportDashboard.Api.Services;

namespace SupportDashboard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController(ITicketService ticketService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TicketListItemDto>>> GetTickets(
        [FromQuery] TicketQuery query,
        CancellationToken ct)
    {
        var tickets = await ticketService.GetTicketsAsync(query, ct);
        return Ok(tickets);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStatsDto>> GetStats(CancellationToken ct)
    {
        return Ok(await ticketService.GetStatsAsync(ct));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TicketDetailDto>> GetTicket(int id, CancellationToken ct)
    {
        var ticket = await ticketService.GetTicketAsync(id, ct);
        return ticket is null ? NotFound(new { message = $"Ticket {id} was not found." }) : Ok(ticket);
    }

    [HttpPost]
    public async Task<ActionResult<TicketDetailDto>> CreateTicket(
        [FromBody] CreateTicketRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var ticket = await ticketService.CreateTicketAsync(request, ct);
        return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TicketDetailDto>> UpdateTicket(
        int id,
        [FromBody] UpdateTicketRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var ticket = await ticketService.UpdateTicketAsync(id, request, ct);
        return ticket is null ? NotFound(new { message = $"Ticket {id} was not found." }) : Ok(ticket);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTicket(int id, CancellationToken ct)
    {
        var deleted = await ticketService.DeleteTicketAsync(id, ct);
        return deleted ? NoContent() : NotFound(new { message = $"Ticket {id} was not found." });
    }

    [HttpPost("{id:int}/comments")]
    public async Task<ActionResult<CommentDto>> AddComment(
        int id,
        [FromBody] CreateCommentRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var comment = await ticketService.AddCommentAsync(id, request, ct);
        return comment is null
            ? NotFound(new { message = $"Ticket {id} was not found." })
            : CreatedAtAction(nameof(GetTicket), new { id }, comment);
    }
}
