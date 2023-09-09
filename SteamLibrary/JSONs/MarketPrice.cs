using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SteamLibrary.JSONs
{
    public class MarketPrice
    {
        [JsonPropertyName("success")]
        public bool success { get; set; }

        [JsonPropertyName("lowest_price")]
        public string lowest_price { get; set; }

        [JsonPropertyName("volume")]
        public string volume { get; set; }

        [JsonPropertyName("median_price")]
        public string median_price { get; set; }
    }
}
