using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Services.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;

namespace MagicApi.Controllers
{
    [Route("api/Search")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        [EnableCors("MainPolicy")]
        [HttpGet("card")]
        public async Task<ActionResult<IEnumerable<CardItem>>> ForCards([FromQuery(Name = "q")] string query, string colors)
        {
            if (query == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = "Query parameter (?q=<query>) is missing!" });
            }

            if (colors == null)
            {
                colors = "W,B,R,U,G";
            }

            string[] ArrayOfColors = colors.Split(',');
            HttpClient client = new HttpClient();
            string json = await client.GetStringAsync("https://api.scryfall.com/catalog/card-names");
            CardNameCatalog nameCatalog = Newtonsoft.Json.JsonConvert.DeserializeObject<CardNameCatalog>(json);

            List<CardItem> cards = new List<CardItem>();
            List<Task> tasks = new List<Task>();
            nameCatalog.data.ForEach(cardName =>
            {
                tasks.Add(Task.Run(async () =>
                {
                    if (!cardName.ToLower().Contains(query.ToLower()))
                    {
                        return;
                    }
                    string card = await client.GetStringAsync($"https://api.scryfall.com/cards/named?exact={cardName}");
                    CardModel cardModel = Newtonsoft.Json.JsonConvert.DeserializeObject<CardModel>(card);
                    CardItem cardItem = new CardItem(cardModel);
                    foreach (var color in ArrayOfColors)
                    {
                        if (cardItem.ColorIdentity.Contains(color))
                        {
                            cards.Add(cardItem);
                            break;
                        }
                    }
                    if (ArrayOfColors.Length == 5 && cardItem.ColorIdentity.Count == 0)
                    {
                        cards.Add(cardItem);
                    }
                }));
            });
            await Task.WhenAll(tasks.ToArray());
            return cards;
        }

    }
}
