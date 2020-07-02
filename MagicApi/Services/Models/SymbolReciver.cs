using System.Collections.Generic;

namespace Services.Models
{
    public class SymbolReciver
    {
        public bool has_more { get; set; }
        public List<SymbolModel> data { get; set; }

    }
}