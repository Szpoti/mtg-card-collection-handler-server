using System.Collections.Generic;
using Services.Models;
using Newtonsoft.Json;

namespace MagicApi.Services.Models
{
    public class SearchContainer
    {
        [JsonProperty("total_cards")]
        public int TotalCards { get; set; }

        public List<CardModel> Data { get; set; }
    }
}
