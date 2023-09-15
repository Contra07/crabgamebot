using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SteamLibrary.JSONs
{
    public class GameItemDef
    {
        [JsonPropertyName("appid")]
        public string appid { get; set; }

        [JsonPropertyName("itemdefid")]
        public string itemdefid { get; set; }

        [JsonPropertyName("Timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("modified")]
        public string modified { get; set; }

        [JsonPropertyName("date_created")]
        public string date_created { get; set; }

        [JsonPropertyName("type")]
        public string type { get; set; }

        [JsonPropertyName("display_type")]
        public string display_type { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("quantity")]
        public int quantity { get; set; }

        [JsonPropertyName("description")]
        public string description { get; set; }

        [JsonPropertyName("background_color")]
        public string background_color { get; set; }

        [JsonPropertyName("icon_url")]
        public string icon_url { get; set; }

        [JsonPropertyName("icon_url_large")]
        public string icon_url_large { get; set; }

        [JsonPropertyName("name_color")]
        public string name_color { get; set; }

        [JsonPropertyName("tags")]
        public string tags { get; set; }

        [JsonPropertyName("tradable")]
        public bool tradable { get; set; }

        [JsonPropertyName("marketable")]
        public bool marketable { get; set; }

        [JsonPropertyName("commodity")]
        public bool commodity { get; set; }

        [JsonPropertyName("price_category")]
        public string price_category { get; set; }

        [JsonPropertyName("store_tags")]
        public string store_tags { get; set; }

        [JsonPropertyName("featured_index")]
        public string featured_index { get; set; }

        [JsonPropertyName("store_images")]
        public string store_images { get; set; }

        [JsonPropertyName("exchange")]
        public string exchange { get; set; }

        [JsonPropertyName("bundle")]
        public string bundle { get; set; }

        [JsonPropertyName("auto_stack")]
        public bool? auto_stack { get; set; }

        [JsonPropertyName("promo")]
        public string promo { get; set; }

        [JsonPropertyName("drop_interval")]
        public int? drop_interval { get; set; }

        [JsonPropertyName("tag_generators")]
        public string tag_generators { get; set; }
    }
}
