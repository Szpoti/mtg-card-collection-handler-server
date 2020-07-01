using System.Collections.Generic;
using Newtonsoft.Json;
using Services.Models;

public class CardItem
{

    public CardItem()
    {

    }
    public CardItem(CardModel cardModel)
    {
        Id = cardModel.id;
        Name = cardModel.name;
        Rarity = cardModel.rarity;
        Set = cardModel.set;
        SetName = cardModel.setName;
        Type = cardModel.type_line;
        Prices = priceFilter(cardModel.prices);
        Text = cardModel.oracle_text;
        ImageUri = cardModel.image_uris.border_crop;
    }

    public System.Guid Id { get; }
    public string Name { get; set; }
    public string Rarity { get; }
    public string Set { get; }
    public string SetName { get; }
    public string Type { get; }
    public string Prices { get; }
    public string Text { get; }
    public string ImageUri { get; }
    public bool IsAvailable { get; set; }
    public string Secret { get; }


    private string priceFilter(Prices prices)
    {
        if (prices.usd != null)
        {
            return $"{prices.usd}$";
        }
        else if (prices.eur != null)
        {
            return $"{prices.eur}€";
        }
        else
        {
            return "price not found";
        }
    }
}