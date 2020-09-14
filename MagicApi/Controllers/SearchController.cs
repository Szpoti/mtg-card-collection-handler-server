using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Services.Models;
using Microsoft.AspNetCore.Cors;
using System;
using Newtonsoft.Json;
using System.Linq;
using MagicApi.Models;
using MagicApi.Services.Models;
using System.Text;

namespace MagicApi.Controllers
{
    [Route("api/Search")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public SearchController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [EnableCors("MainPolicy")]
        [HttpGet("card")]
        public Task<List<CardItem>> ForCards([FromQuery(Name = "q")] string query, string colors)
        {
            return AsAdvanced(new SearchParam() { CardName = query, Colors = colors?.Split(','), });
        }

        [EnableCors("MainPolicy")]
        [HttpPost("advanced")]
        public async Task<List<CardItem>> AsAdvanced(SearchParam query)
        {
            StringBuilder urlParam = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(query.CardName))
            {
                urlParam.AppendFormat("name:{0} ", query.CardName);
            }

            foreach (string[] subtypes in query.RequiredTypes.Values)
            {
                foreach (string subtype in subtypes)
                {
                    urlParam.AppendFormat("t:{0} ", subtype);
                }
            }

            if (query.Colors != null && query.Colors.Length != 0)
            {
                urlParam.AppendFormat("c:{0} ", string.Concat(query.Colors));
            }

            if (query.ArtistName != string.Empty)
            {
                urlParam.AppendFormat("a:\"{0}\" ", query.ArtistName);
            }

            if (query.MinPrice != 0)
            {
                urlParam.AppendFormat("usd>={0} eur>={0} ", query.MinPrice);
            }

            if (query.MaxPrice != double.MaxValue)
            {
                urlParam.AppendFormat("usd<={0} eur<={0} ", query.MaxPrice);
            }

            string url = $"https://api.scryfall.com/cards/search?q={Uri.EscapeDataString(urlParam.ToString())}&page=";
            int apiPage = 1;
            List<CardItem> results = new List<CardItem>();
            SearchContainer searchContainer = await SendApiRequest(url, apiPage, results);
            List<Task> tasks = new List<Task>();
            for (int i = searchContainer.TotalCards; i > 175; i -= 175)
            {
                apiPage++;
                tasks.Add(SendApiRequest(url, apiPage, results));
            }

            await Task.WhenAll(tasks);

            return results.OrderBy(card => card.Name).ToList();
        }

        private async Task<SearchContainer> SendApiRequest(string url, int page, List<CardItem> results)
        {
            url = url + (page);
            try
            {
                string searchResponse = await _httpClient.GetStringAsync(url);
                SearchContainer searchContainer = JsonConvert.DeserializeObject<SearchContainer>(searchResponse);
                foreach (CardModel item in searchContainer.Data)
                {
                    results.Add(new CardItem(item));
                }
                return searchContainer;
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                System.Console.WriteLine(e);
                return new SearchContainer() { TotalCards = 0, Data = new List<CardModel>() };
            }
        }
    }
}
