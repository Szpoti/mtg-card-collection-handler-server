using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MagicApi.Models
{
    public class SearchParam
    {
        public SearchParam()
        {
            RequiredTypes = new Dictionary<string, string[]>();
            MaxPrice = double.MaxValue;
        }

        public string CardName { get; set; }

        public string[] Colors { get; set; }

        [JsonPropertyName("types")]
        public Dictionary<string, string[]> RequiredTypes { get; set; }

        public double MinPrice { get; set; }

        public double MaxPrice { get; set; }

        public string ArtistName { get; set; }

        public override string ToString()
        {
            var result = new System.Text.StringBuilder();
            result.Append("{ Query: ");
            result.Append(CardName);
            result.Append(", ");
            result.Append("Colors: ");
            result.Append(Colors);
            result.Append(", ");
            result.Append("Types: { ");
            foreach (var typeName in RequiredTypes.Keys)
            {
                result.Append(typeName);
                if (RequiredTypes[typeName] != null)
                {
                    result.Append(": ");
                    result.Append(string.Join(",", RequiredTypes[typeName]));
                }
            }
            result.Append(" }, ");
            result.Append("MinPrice: ");
            result.Append(MinPrice);
            result.Append(", ");
            result.Append("MaxPrice: ");
            result.Append(MaxPrice);
            result.Append(", ");
            result.Append("ArtistName: ");
            result.Append(ArtistName);
            result.Append(" }");
            return result.ToString();
        }
    }
}
