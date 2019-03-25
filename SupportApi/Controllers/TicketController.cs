using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SupportApi.Models;
using Newtonsoft.Json;

namespace SupportApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly TicketContext _context;

        public TicketController(TicketContext context) 
        {
            _context = context;
        }

        [HttpGet("generate")]
        public async Task<IActionResult> GenerateTestData()
        {
            if (_context.TicketItems.Count() == 0)
            {
                string mockData = System.IO.File.ReadAllText("MockData/MockTickets.json");
                _context.TicketItems.AddRange(JsonConvert.DeserializeObject<IEnumerable<TicketItem>>(mockData));
                _context.SaveChanges();
            }
            return Ok();
        }

        // GET: api/ticket
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketItem>>> GetTicketItems()
        {
            return await _context.TicketItems.ToListAsync();
        }




        // POST: api/ticket
        [HttpPost]
        public async Task<ActionResult<TicketItem>> PostTicketItem(TicketItem item)
        {
            _context.TicketItems.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTicketItem), new { id = item.Id }, item);
        }



        // GET: api/ticket/id
        [HttpGet("{id}")]
        public async Task<ActionResult<TicketItem>> GetTicketItem(long id)
        {
            var ticketItem = await _context.TicketItems.FindAsync(id);
            if (ticketItem == null)
            {
                return NotFound();
            }
            return ticketItem;
        }

        // PUT: api/ticket/id
        // Maybe try PATCH here since the only put will be to toggle 'handled' (true/false)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTicketItem(long id, TicketItem item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/ticket/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicketItem(long id)
        {
            var ticketToDelete = _context.Find<TicketItem>(id);
            if (ticketToDelete == null) {
                return NotFound();
            }

            _context.Remove(ticketToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}