using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Services.Models;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;

namespace MagicApi.Controllers
{
    [Route("api/Card")]
    [ApiController]
    public class CardController : ControllerBase
    {

        [EnableCors("MainPolicy")]
        [HttpGet("byid/{id}")]
        public async Task<CardItem> getCardById(string id)
        {
            HttpClient client = new HttpClient();
            string json = await client.GetStringAsync($"https://api.scryfall.com/cards/{id}");
            CardModel cardModel = Newtonsoft.Json.JsonConvert.DeserializeObject<CardModel>(json);
            CardItem card = new CardItem(cardModel);
            return card;
        }

        [EnableCors("MainPolicy")]
        [HttpGet("byname/{name}")]
        public async Task<CardItem> getCardByName(string name)
        {
            HttpClient client = new HttpClient();
            string escapedCardName = System.Uri.EscapeDataString(name);
            string json = await client.GetStringAsync($"https://api.scryfall.com/cards/named?exact={escapedCardName}");
            CardModel cardModel = Newtonsoft.Json.JsonConvert.DeserializeObject<CardModel>(json);
            CardItem card = new CardItem(cardModel);
            return card;
        }

        [EnableCors("MainPolicy")]
        [HttpGet("{id}/prints")]
        public async Task<ActionResult<IEnumerable<CardItem>>> GetPrints(Guid id)
        {
            List<CardItem> cards = new List<CardItem>();
            HttpClient client = new HttpClient();
            string json = await client.GetStringAsync($"https://api.scryfall.com/cards/search?order=released\u0026q=oracleid%3A{id}\u0026unique=prints");

            Pageinator apiObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Pageinator>(json);
            foreach (var card in apiObject.data)
            {
                cards.Add(new CardItem(card));
            }
            return cards;
        }

    }
}
