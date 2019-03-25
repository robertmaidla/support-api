using System;

namespace SupportApi.Models
{
    public class TicketItem
    {
        public long Id {get; set;}
        public DateTime createdAt {get; set;}        
        public string query {get; set;}
        public bool handled {get; set;}
        public DateTime deadline {get; set;}

    }
}