using System.Collections.Generic;
using Newtonsoft.Json;
using System;
namespace Services.Models
{
    public class SymbolModel
    {
        public string symbol { get; set; }
        public string svg_uri { get; set; }
        public string loose_variant { get; set; }
        public string english { get; set; }
        public bool transposable { get; set; }
        public bool represents_mana { get; set; }
        public bool appears_in_mana_costs { get; set; }
        public string cmc { get; set; }
        public bool funny { get; set; }
        public List<object> colors { get; set; }
        public object gatherer_alternates { get; set; }

    }

}