using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MagicApi.Models;
using System.Net.Http;
using Services.Models;
using Microsoft.AspNetCore.Cors;

namespace MagicApi.Controllers
{
    [Route("api/Cards")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private readonly CardContext _context;

        public CardsController(CardContext context)
        {
            _context = context;
        }

        // GET: api/CardItems
        [EnableCors("MainPolicy")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CardItem>>> GetCardItems()
        {
            int CARDSNUMBER = 30;
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

        // GET: api/CardItems/4386cb3c-45ac-481c-aa68-04e6efc0442e
        [HttpGet("{id}")]
        public async Task<ActionResult<CardItemDTO>> GetCardItem(System.Guid id)
        {
            var cardItem = await _context.CardItems.FindAsync(id);

            if (cardItem == null)
            {
                return NotFound();
            }

            return ItemToDTO(cardItem);
        }

        // PUT: api/CardItems/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCardItem(System.Guid id, CardItemDTO cardItemDTO)
        {
            if (id != cardItemDTO.Id)
            {
                return BadRequest();
            }

            var cardItem = await _context.CardItems.FindAsync(id);
            if (cardItem == null)
            {
                return NotFound();
            }

            cardItem.Name = cardItemDTO.Name;
            cardItem.IsAvailable = cardItemDTO.IsAvailable;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!CardItemExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/CardItems
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<CardItemDTO>> PostCardItem(CardItemDTO cardItemDTO)
        {
            var cardItem = new CardItem
            {
                IsAvailable = cardItemDTO.IsAvailable,
                Name = cardItemDTO.Name
            };

            _context.CardItems.Add(cardItem);
            await _context.SaveChangesAsync();

            //return CreatedAtAction("GetCardItem", new { id = cardItem.Id }, cardItem);
            return CreatedAtAction(nameof(GetCardItem), new { id = cardItem.Id }, ItemToDTO(cardItem));
        }

        // DELETE: api/CardItems/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCardItem(long id)
        {
            var cardItem = await _context.CardItems.FindAsync(id);
            if (cardItem == null)
            {
                return NotFound();
            }

            _context.CardItems.Remove(cardItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CardItemExists(System.Guid id)
        {
            return _context.CardItems.Any(e => e.Id == id);
        }

        private static CardItemDTO ItemToDTO(CardItem todoItem) =>
        new CardItemDTO
        {
            Id = todoItem.Id,
            Name = todoItem.Name,
            IsAvailable = todoItem.IsAvailable
        };
    }
}
