using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimerParcial.Data;
using PrimerParcial.Models;

namespace PrimerParcial.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public EventsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/Events?from=2025-10-01&to=2025-10-31&online=true&q=webinar
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetAll(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] bool? online,
            [FromQuery] string? q)
        {
            IQueryable<Event> query = _db.Events.AsNoTracking();

            if (from.HasValue)   query = query.Where(e => e.StartAt >= from.Value);
            if (to.HasValue)     query = query.Where(e => e.StartAt <= to.Value);
            if (online.HasValue) query = query.Where(e => e.IsOnline == online.Value);

            if (!string.IsNullOrWhiteSpace(q))
            {
                var s = q.Trim().ToLower();
                query = query.Where(e =>
                    e.Title.ToLower().Contains(s) ||
                    e.Location.ToLower().Contains(s) ||
                    (e.Notes != null && e.Notes.ToLower().Contains(s)));
            }

            var list = await query.OrderBy(e => e.StartAt).ToListAsync();
            return Ok(list);
        }

        // GET: api/Events/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Event>> GetById(int id)
        {
            var entity = await _db.Events.FindAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        // POST: api/Events
        [HttpPost]
        public async Task<ActionResult<Event>> Create([FromBody] Event input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (input.EndAt.HasValue && input.EndAt.Value < input.StartAt)
                return BadRequest("EndAt no puede ser menor que StartAt.");

            // Normalización
            input.Id = 0;
            input.Title = input.Title?.Trim() ?? string.Empty;
            input.Location = input.Location?.Trim() ?? string.Empty;
            input.Notes = string.IsNullOrWhiteSpace(input.Notes) ? null : input.Notes.Trim();

            _db.Events.Add(input);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
        }

        // PUT: api/Events/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Event updated)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != updated.Id) return BadRequest("El Id del cuerpo no coincide con la ruta.");
            if (updated.EndAt.HasValue && updated.EndAt.Value < updated.StartAt)
                return BadRequest("EndAt no puede ser menor que StartAt.");

            var entity = await _db.Events.FindAsync(id);
            if (entity == null) return NotFound();

            entity.Title    = updated.Title?.Trim() ?? string.Empty;
            entity.Location = updated.Location?.Trim() ?? string.Empty;
            entity.StartAt  = updated.StartAt;
            entity.EndAt    = updated.EndAt;
            entity.IsOnline = updated.IsOnline;
            entity.Notes    = string.IsNullOrWhiteSpace(updated.Notes) ? null : updated.Notes.Trim();

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Events/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _db.Events.FindAsync(id);
            if (entity == null) return NotFound();

            _db.Events.Remove(entity);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
