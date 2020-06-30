using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MagicApi.Models;

namespace MagicApi.Controllers
{
    [Route("api/TodoItems")]
    [ApiController]
    public class CardItemsController : ControllerBase
    {
        private readonly CardContext _context;

        public CardItemsController(CardContext context)
        {
            _context = context;
        }

        // GET: api/CardItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CardItemDTO>>> GetCardItems()
        {
            return await _context.CardItems.Select(x => ItemToDTO(x)).ToListAsync();
        }

        // GET: api/CardItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CardItemDTO>> GetCardItem(long id)
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
        public async Task<IActionResult> PutCardItem(long id, CardItemDTO cardItemDTO)
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

        private bool CardItemExists(long id)
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
