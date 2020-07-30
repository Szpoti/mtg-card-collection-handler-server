using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Services.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using System;

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
            System.Console.WriteLine("Getting card names...");
            string json = await client.GetStringAsync("https://api.scryfall.com/catalog/card-names");
            System.Console.WriteLine("Card names arrived!");
            System.Console.WriteLine("Deserializing card names...");
            CardNameCatalog nameCatalog = Newtonsoft.Json.JsonConvert.DeserializeObject<CardNameCatalog>(json);
            System.Console.WriteLine("Card names serializied!");

            List<CardItem> cards = new List<CardItem>();
            List<Task> tasks = new List<Task>();
            nameCatalog.data.ForEach(cardName =>
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        if (!cardName.ToLower().Contains(query.ToLower()))
                        {
                            return;
                        }
                        System.Console.WriteLine("Trying to get " + cardName + "...");
                        string escapedCardName = System.Uri.EscapeDataString(cardName);
                        string card = await client.GetStringAsync($"https://api.scryfall.com/cards/named?exact={escapedCardName}");
                        System.Console.WriteLine(cardName + " arrived!");

                        CardModel cardModel = Newtonsoft.Json.JsonConvert.DeserializeObject<CardModel>(card);
                        CardItem cardItem = new CardItem(cardModel);
                        foreach (var color in ArrayOfColors)
                        {
                            if (cardItem.ColorIdentity.Contains(color))
                            {
                                System.Console.WriteLine(cardName + " added!");
                                cards.Add(cardItem);
                                break;
                            }
                        }
                        if (ArrayOfColors.Length == 5 && cardItem.ColorIdentity.Count == 0)
                        {
                            System.Console.WriteLine(cardName + " added!");
                            cards.Add(cardItem);
                        }
                    }
                    catch (HttpRequestException ex)
                    {

                        System.Console.ForegroundColor = ConsoleColor.Red;
                        System.Console.WriteLine("\n\n\nException caught at " + cardName + "\n\n\n");
                        System.Console.ResetColor();

                    }
                }));
            });
            await Task.WhenAll(tasks.ToArray());
            System.Console.WriteLine("Returning cards!");
            return cards;
        }

    }
}
