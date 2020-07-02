using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Services.Models;
using Microsoft.AspNetCore.Cors;

namespace MagicApi.Controllers
{
    [Route("api/symbology")]
    [ApiController]
    public class SymbologyController : ControllerBase
    {
        [EnableCors("MainPolicy")]
        [HttpGet("symbols")]
        public async Task<ActionResult<IEnumerable<Symbol>>> GetSymbols()
        {
            List<Symbol> symbols = new List<Symbol>();
            HttpClient client = new HttpClient();
            string json = await client.GetStringAsync("https://api.scryfall.com/symbology");

            SymbolReciver apiObject = Newtonsoft.Json.JsonConvert.DeserializeObject<SymbolReciver>(json);
            foreach (var symbol in apiObject.data)
            {
                symbols.Add(new Symbol(symbol));
            }
            return symbols;
        }

    }
}
