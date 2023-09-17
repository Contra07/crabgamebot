using System;
using System.Text.Json.Serialization;

namespace SteamLibrary.JSONs
{
    public class InventoryItemDef
    {
        [JsonPropertyName("accountid")]
        public string accountid { get; set; }

        [JsonPropertyName("itemid")]
        public string itemid { get; set; }

        [JsonPropertyName("quantity")]
        public int quantity { get; set; }

        [JsonPropertyName("originalitemid")]
        public string originalitemid { get; set; }

        [JsonPropertyName("itemdefid")]
        public string itemdefid { get; set; }

        [JsonPropertyName("appid")]
        public int appid { get; set; }

        [JsonPropertyName("acquired")]
        public string acquired { get; set; }

        [JsonPropertyName("state")]
        public string state { get; set; }

        [JsonPropertyName("origin")]
        public string origin { get; set; }

        [JsonPropertyName("state_changed_timestamp")]
        public string state_changed_timestamp { get; set; }

        [JsonPropertyName("tags")]
        public string tags { get; set; }
    }
}