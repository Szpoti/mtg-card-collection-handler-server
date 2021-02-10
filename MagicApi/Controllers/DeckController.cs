using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.Extensions.Options;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using MagicApi.Models;
using MagicApi.Services;
using MagicApi.Data;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace MagicApi.Controllers
{
    [Route("api/deck")]
    [ApiController]
    public class DeckController : ControllerBase
    {
        private readonly MTGContext _context;
        private readonly IConfiguration _configuration;


        public DeckController(MTGContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        [AllowAnonymous]
        [EnableCors("MainPolicy")]
        [HttpPost("create")]
        public IActionResult CreateDeck([FromBody] Deck deck)
        {
            try
            {
                var existingDeck = _context.Decks.Where(d => d.Name == deck.Name).FirstOrDefault();
                if (existingDeck == null)
                {
                    Deck newDeck = new Deck() { Name = deck.Name, UserId = deck.UserId, FormatId = deck.FormatId };
                    _context.Add(newDeck);
                    _context.SaveChanges();
                    return Ok("Deck succesfully created");
                }
                return StatusCode(409, "Deck name already in the database");
            }
            catch (System.Exception e)
            {
                return StatusCode(400, e);
            }
        }

        [AllowAnonymous]
        [EnableCors("MainPolicy")]
        [HttpPost("rename/{id}/{newName}")]
        public IActionResult RenameDeck(int id, string newName)
        {
            try
            {
                var existingDeck = _context.Decks.Where(d => d.Id == id).FirstOrDefault();
                if (existingDeck != null)
                {
                    existingDeck.Name = newName;
                    _context.SaveChanges();
                    return Ok("Deck was succesfully renamed!");
                }
                return NotFound("No deck with the given DeckId was found.");
            }
            catch (System.Exception e)
            {
                return StatusCode(409, e);
            }
        }

        [AllowAnonymous]
        [EnableCors("MainPolicy")]
        [HttpGet("decksOf/{id}")]
        public IActionResult GetDecksOf(int id)
        {
            try
            {
                var existingUser = _context.Users.Where(u => u.Id == id).FirstOrDefault();
                if (existingUser != null)
                {
                    List<Deck> decks = _context.Decks.Where(d => d.UserId == existingUser.Id).ToList();
                    return Ok(decks);
                }
                return NotFound("No user with given Id was found");
            }
            catch (System.Exception e)
            {
                return StatusCode(409, e);
            }
        }

        [AllowAnonymous]
        [EnableCors("MainPolicy")]
        [HttpPost("addTo/{deckId}/{quantity}/{cardId}")]
        public IActionResult AddCardToDeck(int deckId, int quantity, string cardId)
        {
            try
            {
                var existingDeck = _context.Decks.Where(d => d.Id == deckId).FirstOrDefault();
                if (existingDeck != null)
                {
                    var existingCard = _context.DeckCards.Where(c => c.CardId == cardId).FirstOrDefault();
                    if (existingCard != null)
                    {
                        existingCard.Quantity += quantity;
                    }
                    else
                    {
                        _context.DeckCards.Add(new DeckCard() { CardId = cardId, DeckId = deckId, Quantity = quantity });
                    }
                    _context.SaveChanges();
                    return Ok("Card(s) succesfully added to deck");
                }
                return NotFound("Deck with given Id not found.");
            }
            catch (System.Exception e)
            {
                return StatusCode(409, e);
            }
        }

        [AllowAnonymous]
        [EnableCors("MainPolicy")]
        [HttpPost("removeFrom/{deckId}/{quantity}/{cardId}")]
        public IActionResult RemoveCardFromDeck(int deckId, int quantity, string cardId)
        {
            try
            {
                var existingDeck = _context.Decks.Where(d => d.Id == deckId).FirstOrDefault();
                if (existingDeck != null)
                {
                    var existingCard = _context.DeckCards.Where(c => c.CardId == cardId).FirstOrDefault();
                    if (existingCard != null)
                    {
                        if (existingCard.Quantity - quantity <= 0)
                        {
                            _context.DeckCards.Remove(existingCard);
                        }
                        else
                        {
                            existingCard.Quantity -= quantity;
                        }
                    }
                    else
                    {
                        return NotFound("CardId in given deck not found.");
                    }
                    _context.SaveChanges();
                    return Ok("Card(s) succesfully removed from deck");
                }
                return NotFound("Deck with given Id not found.");
            }
            catch (System.Exception e)
            {
                return StatusCode(409, e);
            }
        }

    }
}
