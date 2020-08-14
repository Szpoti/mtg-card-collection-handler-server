using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Services.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using System;
using Newtonsoft.Json;
using System.Linq;
using MagicApi.Models;
using MagicApi.Services.Models;

namespace MagicApi.Controllers
{
    [Route("api/Search")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        internal static readonly string[] SupportedColors = new string[] { "W", "B", "R", "U", "G" };
        private readonly HttpClient _httpClient;

        public SearchController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

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

        [EnableCors("MainPolicy")]
        [HttpPost("advanced")]
        public async Task<ActionResult<IEnumerable<CardItem>>> AsAdvanced(SearchParam query)
        {
            if (string.IsNullOrWhiteSpace(query.CardName))
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = "Partial card name is required to start searching!" });
            }

            query.CardName = query.CardName.ToLower();
            string[] requiredColors = query.Colors;
            if (query.Colors == null)
            {
                requiredColors = SupportedColors;
            }

            CardNameCatalog nameCatalog = null;
            var availableTypes = new Dictionary<string, List<string>>()
            {
                { "artifact-types", null },
                { "enchantment-types", null },
                { "land-types", null },
                { "spell-types", null },
                { "planeswalker-types", null },
                { "creature-types", null },
                { "artist-names", null },
            };
            List<Task> tasks = new List<Task>();
            tasks.Add(Task.Run(async () =>
            {
                string json = await _httpClient.GetStringAsync("https://api.scryfall.com/catalog/card-names");
                nameCatalog = JsonConvert.DeserializeObject<CardNameCatalog>(json);
            }));

            foreach (string typeName in availableTypes.Keys.ToList())
            {
                tasks.Add(Task.Run(async () =>
                {
                    string json = await _httpClient.GetStringAsync($"https://api.scryfall.com/catalog/{typeName}");
                    availableTypes[typeName] = JsonConvert.DeserializeObject<CardNameCatalog>(json).data;
                }));
            }
            await Task.WhenAll(tasks.ToArray());

            List<CardItem> results = new List<CardItem>();
            string escapedCardName = Uri.EscapeDataString(query.CardName);
            var searchResponse = await _httpClient.GetStringAsync($"https://api.scryfall.com/cards/search?q={escapedCardName}");
            List<CardModel> foundCards = JsonConvert.DeserializeObject<SearchContainer>(searchResponse).Data;
            foreach (CardModel card in foundCards)
            {
                if (!card.name.ToLower().Contains(query.CardName))
                {
                    continue;
                }

                if (!card.HasAny(requiredColors))
                {
                    continue;
                }

                if (!card.HasAny(query.RequiredTypes))
                {
                    continue;
                }

                if (!card.CostBetween(query.MinPrice, query.MaxPrice))
                {
                    continue;
                }

                if (!card.DrawnBy(query.ArtistName))
                {
                    continue;
                }

                results.Add(new CardItem(card));
            }

            return results;
        }
    }

    internal static class SearchExtensions
    {
        internal static bool HasAny(this CardModel card, string[] colors)
        {
            if (colors == null || colors.Length == 0)
            {
                return true;
            }

            if (card.color_identity.Count == SearchController.SupportedColors.Length
                || SearchController.SupportedColors.Length == colors.Length)
            {
                return true;
            }

            foreach (string color in colors)
            {
                if (card.color_identity.Contains(color))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool HasAny(this CardModel card, Dictionary<string, string[]> requestedSubtypes)
        {
            if (requestedSubtypes.Count == 0)
            {
                return true;
            }

            int remainingReqSubtypes = requestedSubtypes.Values.Sum(array => array.Length);
            if (remainingReqSubtypes == 0)
            {
                return true;
            }
            foreach (string typeName in requestedSubtypes.Keys)
            {
                string[] cardTypes = card.type_line.ToLower().Split(" — ");
                string mainType = cardTypes[0];
                string[] subtypes = 1 < cardTypes.Length ? cardTypes[1].Split(" ") : new string[0];
                if (cardTypes.Length == 1 && cardTypes[0] == typeName.ToLower())
                {
                    return true;
                }
                else if (cardTypes.Length == 2)
                {
                    if (requestedSubtypes[typeName] == null || requestedSubtypes[typeName].Length == 0)
                    {
                        continue;
                    }

                    foreach (string requestedSubtype in requestedSubtypes[typeName])
                    {
                        foreach (string cardSubType in subtypes)
                        {
                            if (cardSubType == requestedSubtype.ToLower())
                            {
                                remainingReqSubtypes--;
                            }
                        }
                    }

                    if (remainingReqSubtypes == 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal static bool CostBetween(this CardModel card, double min, double max)
        {
            if (IsBetween(card.prices.usd, min, max)
                || IsBetween(card.prices.usd_foil, min, max)
                || IsBetween(card.prices.eur, min, max)
                || IsBetween(card.prices.tix, min, max))
            {
                return true;
            }

            return false;
        }

        internal static bool DrawnBy(this CardModel card, string artistName)
        {
            if (string.IsNullOrWhiteSpace(artistName))
            {
                return true;
            }

            if (card.artist.ToLower().Contains(artistName.ToLower()))
            {
                return true;
            }

            return false;
        }

        private static bool IsBetween(double? value, double min, double max)
        {
            if (value == null)
            {
                return true;
            }

            return min <= value && value <= max;
        }
    }
}
