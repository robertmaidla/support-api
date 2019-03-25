using Microsoft.EntityFrameworkCore;

namespace SupportApi.Models
{
    public class TicketContext : DbContext
    {
        public TicketContext(DbContextOptions<TicketContext> options) : base(options) {}
        public DbSet<TicketItem> TicketItems {get; set;}
    }
}