using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimerParcial.Data;
using PrimerParcial.Models;

namespace PrimerParcial.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupportTicketsController : ControllerBase
    {
        private readonly AppDbContext _db;

        // Estados que implican cierre del ticket
        private static readonly HashSet<string> ClosedStates = new(StringComparer.OrdinalIgnoreCase)
        {
            "Resolved", "Closed"
        };

        public SupportTicketsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/SupportTickets?status=Open&severity=High
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupportTicket>>> GetAll(
            [FromQuery] string? status,
            [FromQuery] string? severity)
        {
            IQueryable<SupportTicket> query = _db.SupportTickets.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(status))
            {
                var s = status.Trim();
                query = query.Where(t => t.Status != null && t.Status.ToLower() == s.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(severity))
            {
                var sev = severity.Trim();
                query = query.Where(t => t.Severity != null && t.Severity.ToLower() == sev.ToLower());
            }

            var list = await query
                .OrderByDescending(t => t.OpenedAt)
                .ToListAsync();

            return Ok(list);
        }

        // GET: api/SupportTickets/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SupportTicket>> GetById(int id)
        {
            var ticket = await _db.SupportTickets.FindAsync(id);
            if (ticket == null) return NotFound();
            return Ok(ticket);
        }

        // POST: api/SupportTickets
        [HttpPost]
        public async Task<ActionResult<SupportTicket>> Create([FromBody] SupportTicket ticket)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            ticket.Id = 0;
            ticket.OpenedAt = DateTime.UtcNow;

            // Normalizar strings (opcional pero recomendado)
            ticket.Status = ticket.Status?.Trim() ?? string.Empty;
            ticket.Severity = ticket.Severity?.Trim() ?? string.Empty;
            ticket.AssignedTo = string.IsNullOrWhiteSpace(ticket.AssignedTo) ? null : ticket.AssignedTo.Trim();

            // Cerrar automáticamente si el estado inicial es "Resolved" o "Closed"
            if (ClosedStates.Contains(ticket.Status))
                ticket.ClosedAt ??= DateTime.UtcNow;
            else
                ticket.ClosedAt = null;

            _db.SupportTickets.Add(ticket);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
        }

        // PUT: api/SupportTickets/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SupportTicket updated)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != updated.Id) return BadRequest("El Id del cuerpo no coincide con la ruta.");

            var ticket = await _db.SupportTickets.FindAsync(id);
            if (ticket == null) return NotFound();

            ticket.Subject = updated.Subject;
            ticket.RequesterEmail = updated.RequesterEmail;
            ticket.Description = updated.Description;
            ticket.Severity = updated.Severity?.Trim() ?? string.Empty;
            ticket.Status = updated.Status?.Trim() ?? string.Empty;
            ticket.AssignedTo = string.IsNullOrWhiteSpace(updated.AssignedTo) ? null : updated.AssignedTo.Trim();

            if (ClosedStates.Contains(ticket.Status))
                ticket.ClosedAt ??= DateTime.UtcNow;
            else
                ticket.ClosedAt = null;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/SupportTickets/5/status
        // Body esperado (JSON): "Closed"  (un string entre comillas)
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var ticket = await _db.SupportTickets.FindAsync(id);
            if (ticket == null) return NotFound();

            var normalized = status?.Trim() ?? string.Empty;
            ticket.Status = normalized;

            if (ClosedStates.Contains(normalized))
                ticket.ClosedAt ??= DateTime.UtcNow;
            else
                ticket.ClosedAt = null;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/SupportTickets/5/assign
        // Body esperado (JSON): "juan.perez"  (string) o ""/null para desasignar
        [HttpPatch("{id:int}/assign")]
        public async Task<IActionResult> AssignTo(int id, [FromBody] string? assignee)
        {
            var ticket = await _db.SupportTickets.FindAsync(id);
            if (ticket == null) return NotFound();

            ticket.AssignedTo = string.IsNullOrWhiteSpace(assignee) ? null : assignee.Trim();

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/SupportTickets/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _db.SupportTickets.FindAsync(id);
            if (ticket == null) return NotFound();

            _db.SupportTickets.Remove(ticket);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
