using System.Collections.Generic;
using Services.Models;

public class CardItem
{
    public CardItem()
    {
    }

    public CardItem(CardModel cardModel)
    {
        Id = cardModel.id;
        OracleId = cardModel.oracle_id;
        Name = cardModel.name;
        Rarity = cardModel.rarity;
        Set = cardModel.set;
        SetName = cardModel.setName;
        Type = cardModel.type_line;
        Price = priceFilter(cardModel.prices);
        Text = cardModel.oracle_text;
        ColorIdentity = cardModel.color_identity;
        ImageUri = cardModel.image_uris?.border_crop;
        Legalities = cardModel.legalities;
        Layout = cardModel.layout;
        CardImages = ImageUri == null ? new string[] { cardModel.card_faces[0].image_uris.border_crop, cardModel.card_faces[1].image_uris.border_crop } : null;
        CardFaces = cardModel.card_faces != null ? new string[] { cardModel.card_faces[0].oracle_text, cardModel.card_faces[1].oracle_text } : null;
    }

    public System.Guid Id { get; }
    public System.Guid OracleId { get; }
    public string Name { get; set; }
    public string Rarity { get; }
    public string Set { get; }
    public string SetName { get; }
    public string Type { get; }
    public string Price { get; }
    public string Text { get; }
    public List<string> ColorIdentity { get; }
    public string ImageUri { get; }
    public bool IsAvailable { get; set; }
    public string Secret { get; }
    public string Layout { get; set; }
    public Legalities Legalities { get; set; }
    public string[] CardFaces { get; set; }
    public string[] CardImages { get; set; }
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