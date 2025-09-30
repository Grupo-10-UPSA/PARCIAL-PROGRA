using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimerParcial.Data;
using PrimerParcial.Models;

namespace PrimerParcial.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProductController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> Get()
            => await _context.Products.ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> Get(int id)
        {
            var p = await _context.Products.FindAsync(id);
            if (p == null) return NotFound();
            return p;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Post(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Product product)
        {
            if (id != product.Id) return BadRequest();
            _context.Entry(product).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Products.AnyAsync(e => e.Id == id)) return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _context.Products.FindAsync(id);
            if (p == null) return NotFound();
            _context.Products.Remove(p);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}