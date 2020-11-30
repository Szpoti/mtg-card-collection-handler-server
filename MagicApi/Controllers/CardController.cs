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
        [HttpGet("homepage")]
        public async Task<ActionResult<IEnumerable<CardItem>>> GetCardItems()
        {
            int CARDSNUMBER = 32;
            List<CardItem> cards = new List<CardItem>();
            List<Task> requestSenders = new List<Task>();
            HttpClient client = new HttpClient();
            for (int i = 0; i < CARDSNUMBER; i++)
            {
                requestSenders.Add(Task.Run(async () =>
               {
                   string card = await client.GetStringAsync("https://api.scryfall.com/cards/random");
                   CardModel cardModel = Newtonsoft.Json.JsonConvert.DeserializeObject<CardModel>(card);
                   CardItem cardItem = new CardItem(cardModel);
                   cards.Add(cardItem);
               }));
            }
            await Task.WhenAll(requestSenders.ToArray());
            return cards;
        }

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