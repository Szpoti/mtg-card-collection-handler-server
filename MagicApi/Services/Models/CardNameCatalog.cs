using System.Collections.Generic;

namespace Services.Models
{
    public class CardNameCatalog
    {
        public string uri { get; set; } 
        public int total_values { get; set; } 
        public List<string> data { get; set; } 
    }
}