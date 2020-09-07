using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MagicApi.Controllers;
using MagicApi.Models;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace MagicApi.Tests.Integration.Controllers
{
    [TestFixture]
    public class SearchControllerTests
    {
        private static SearchController _controller;

        [OneTimeSetUp]
        public static void OneTimeSetUp()
        {
            HttpClient httpClient = GetConfiguredHttpClient();
            _controller = new SearchController(httpClient);
        }

        [Test]
        public async Task AsAdvanced_WithExactName_ShouldReturnOneResult()
        {
            var filter = new SearchParam() { CardName = "Academy Elite" };

            var response = await _controller.AsAdvanced(filter);

            Assert.That(response.Value.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task AsAdvanced_WithPartialName_ShouldReturnTwelveResults()
        {
            var filter = new SearchParam() { CardName = "Academy" };

            var response = await _controller.AsAdvanced(filter);

            Assert.That(response.Value.Count(), Is.EqualTo(12));
        }

        [Test]
        public async Task AsAdvanced_WithExactLowercaseName_ShouldReturnOneResult()
        {
            var filter = new SearchParam() { CardName = "academy elite" };

            var response = await _controller.AsAdvanced(filter);

            Assert.That(response.Value.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task AsAdvanced_WithhoutColorFilter_ShouldReturnAllCards()
        {
            var filter = new SearchParam() { CardName = "Academy" };

            var response = await _controller.AsAdvanced(filter);

            Assert.That(response.Value.Count(), Is.EqualTo(12));
        }

        [Test]
        public async Task AsAdvanced_WithOnlyUColorFilter_ShouldReturnCardsThatContainsAtLeastUColor()
        {
            var filter = new SearchParam()
            {
                CardName = "Academy",
                Colors = new string[] { "U" }
            };

            var response = await _controller.AsAdvanced(filter);

            Assert.That(response.Value.Count(), Is.EqualTo(8));
        }

        [Test]
        public async Task AsAdvanced_WithUAndRColorFilter_ShouldReturnCardsThatContainsAtLeastUOrRColor()
        {
            var filter = new SearchParam()
            {
                CardName = "Academy",
                Colors = new string[] { "U", "R" }
            };

            var response = await _controller.AsAdvanced(filter);

            Assert.That(response.Value.Count(), Is.EqualTo(9));
        }

        [Test]
        public async Task AsAdvanced_WithTypeAndSubtype_ShouldReturnOnlyCardsThatIncludesThatSubtype()
        {
            var filter = new SearchParam()
            {
                CardName = "Academy",
                RequiredTypes = new Dictionary<string, string[]>()
                {
                    { "creature", new string[] { "Human" }},
                }
            };

            var response = await _controller.AsAdvanced(filter);

            Assert.That(response.Value.Count(), Is.EqualTo(6));
        }

        [TestCase(new string[] { "Human", "drake" }, 0)]
        [TestCase(new string[] { "HuMaN", "wIzArD" }, 4)]
        public async Task AsAdvanced_WithTypeAndTwoSubtypes_ShouldReturnOnlyCardsWithinThoseSubtypes(string[] subtypes, int expectedCount)
        {
            var filter = new SearchParam()
            {
                CardName = "Academy",
                RequiredTypes = new Dictionary<string, string[]>()
                {
                    { "creature", subtypes },
                }
            };

            var response = await _controller.AsAdvanced(filter);

            Assert.That(response.Value.Count(), Is.EqualTo(expectedCount));
        }

        [Test]
        public async Task AsAdvanced_WithArtifactType_ShouldReturnOneCard()
        {
            var filter = new SearchParam()
            {
                CardName = "Abandoned Sarcophagus",
                RequiredTypes = new Dictionary<string, string[]>()
                {
                    { "artifact", new string[0] },
                }
            };

            var response = await _controller.AsAdvanced(filter);

            Assert.That(response.Value.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task AsAdvanced_WithEnchantmentType_ShouldReturnOneCard()
        {
            var filter = new SearchParam()
            {
                CardName = "Warp Artifact",
                RequiredTypes = new Dictionary<string, string[]>()
                {
                    { "enchantment", new string[0] },
                }
            };

            var response = await _controller.AsAdvanced(filter);

            Assert.That(response.Value.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task AsAdvanced_WithLandType_ShouldReturnOneCard()
        {
            var filter = new SearchParam()
            {
                CardName = "Treetop village",
                RequiredTypes = new Dictionary<string, string[]>()
                {
                    { "land", new string[0] },
                }
            };

            var response = await _controller.AsAdvanced(filter);

            Assert.That(response.Value.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task AsAdvanced_WithPlaneswalkerType_ShouldReturnOneCard()
        {
            var filter = new SearchParam()
            {
                CardName = "Personal Decoy",
                RequiredTypes = new Dictionary<string, string[]>()
                {
                    { "planeswalker", new string[0] },
                }
            };

            var response = await _controller.AsAdvanced(filter);

            Assert.That(response.Value.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task AsAdvanced_WithMinAndMaxPrice_ShouldReturnCardsWithinThatIntervalInAnyCurrency()
        {
            var filter = new SearchParam()
            {
                CardName = "Academy",
                MaxPrice = 0.1,
            };

            var response = await _controller.AsAdvanced(filter);

            Assert.That(response.Value.Count(), Is.EqualTo(10));
        }

        [Test]
        public async Task AsAdvanced_WhenPriceIsNull_ShouldReturnThatCard()
        {
            var filter = new SearchParam()
            {
                CardName = "Tolarian Academy",
                MinPrice = double.MaxValue,
                MaxPrice = double.MaxValue,
            };

            var response = await _controller.AsAdvanced(filter);

            Assert.That(response.Value.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task AsAdvanced_WhenFilteringOnPrice_MinPriceShouldBeInclusive()
        {
            var filter = new SearchParam()
            {
                CardName = "Academy Rector",
                MinPrice = 699.95,
                MaxPrice = 700,
            };

            var response = await _controller.AsAdvanced(filter);

            Assert.That(response.Value.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task AsAdvanced_WhenFilteringOnPrice_MaxPriceShouldBeInclusive()
        {
            var filter = new SearchParam()
            {
                CardName = "Academy Rector",
                MinPrice = 600,
                MaxPrice = 699.96,
            };

            var response = await _controller.AsAdvanced(filter);

            Assert.That(response.Value.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task AsAdvanced_WithOnlyArtistFirstName_ShouldReturnCards()
        {
            var filter = new SearchParam()
            {
                CardName = "Academy",
                ArtistName = "Stephen",
            };

            var response = await _controller.AsAdvanced(filter);

            Assert.That(response.Value.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task AsAdvanced_ArtistNameFiltering_ShouldBeCaseInsensitive()
        {
            var filter = new SearchParam()
            {
                CardName = "Academy",
                ArtistName = "sTePhEn DaNiElE",
            };

            var response = await _controller.AsAdvanced(filter);

            Assert.That(response.Value.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task AsAdvanced_WhenFilteringOnArtists_ShouldFilterOnSecondArtistToo()
        {
            var filter = new SearchParam()
            {
                CardName = "Academy",
                ArtistName = "Szikszai",
            };

            var response = await _controller.AsAdvanced(filter);

            Assert.That(response.Value.Count(), Is.EqualTo(1));
        }

        private static readonly string[] _cardNames = {
            "Abandoned Sarcophagus",
            "Academy at Tolaria West",
            "Academy Journeymage",
            "Academy Researchers",
            "Personal Decoy",
            "Rayne, Academy Chancellor",
            "Academy Drake",
            "Academy Raider",
            "Academy Ruins",
            "Tolarian Academy",
            "Academy Elite",
            "Academy Rector",
            "Nephalia Academy",
            "Treetop Village",
            "Urza, Academy Headmaster",
            "Warp Artifact",
        };

        private static HttpClient GetConfiguredHttpClient()
        {
            var mockHttp = new MockHttpMessageHandler();

            string filePath = Path.Combine("StaticResources", "catalog", "card-names.json");
            mockHttp.When("https://api.scryfall.com/catalog/card-names")
                    .RespondWithJson(File.ReadAllText(filePath));

            var types = new List<string>()
            {
                "artifact-types",
                "enchantment-types",
                "land-types",
                "spell-types",
                "planeswalker-types",
                "creature-types",
                "artist-names",
            };
            types.ForEach(typeName =>
            {
                filePath = Path.Combine("StaticResources", "catalog", $"{typeName}.json");
                mockHttp.When($"https://api.scryfall.com/catalog/{typeName}")
                        .RespondWithJson(File.ReadAllText(filePath));
            });

            string cardsRootPath = Path.Combine("StaticResources", "cards");
            foreach (string cardName in _cardNames)
            {
                string cardFileName = Path.Combine(cardsRootPath, cardName.Replace(" ", "-").Replace(",", "") + ".json");
                mockHttp.When($"https://api.scryfall.com/cards/named?exact={Uri.EscapeDataString(cardName)}")
                        .RespondWithJson(File.ReadAllText(cardFileName));
            }

            return mockHttp.ToHttpClient();
        }
    }

    internal static class MockedRequestExtensions
    {
        internal static MockedRequest RespondWithJson(this MockedRequest request, string content)
        {
            return request.Respond("application/json", content);
        }
    }
}
