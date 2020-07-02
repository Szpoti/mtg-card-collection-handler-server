using System.Collections.Generic;

namespace Services.Models
{
    public class Pageinator
    {
        public int total_cards { get; set; }
        public bool has_more { get; set; }
        public List<CardModel> data { get; set; }

    }
}