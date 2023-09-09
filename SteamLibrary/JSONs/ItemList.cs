using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SteamLibrary.JSONs
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class AssetDescription
    {
        [JsonPropertyName("appid")]
        public int appid { get; set; }

        [JsonPropertyName("classid")]
        public string classid { get; set; }

        [JsonPropertyName("instanceid")]
        public string instanceid { get; set; }

        [JsonPropertyName("currency")]
        public int currency { get; set; }

        [JsonPropertyName("background_color")]
        public string background_color { get; set; }

        [JsonPropertyName("icon_url")]
        public string icon_url { get; set; }

        [JsonPropertyName("icon_url_large")]
        public string icon_url_large { get; set; }

        [JsonPropertyName("descriptions")]
        public List<Description> descriptions { get; set; }

        [JsonPropertyName("tradable")]
        public int tradable { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("name_color")]
        public string name_color { get; set; }

        [JsonPropertyName("type")]
        public string type { get; set; }

        [JsonPropertyName("market_name")]
        public string market_name { get; set; }

        [JsonPropertyName("market_hash_name")]
        public string market_hash_name { get; set; }

        [JsonPropertyName("commodity")]
        public int commodity { get; set; }

        [JsonPropertyName("market_tradable_restriction")]
        public int market_tradable_restriction { get; set; }

        [JsonPropertyName("market_marketable_restriction")]
        public int market_marketable_restriction { get; set; }

        [JsonPropertyName("marketable")]
        public int marketable { get; set; }
    }

    public class Description
    {
        [JsonPropertyName("type")]
        public string type { get; set; }

        [JsonPropertyName("value")]
        public string value { get; set; }
    }

    public class Result
    {
        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("hash_name")]
        public string hash_name { get; set; }

        [JsonPropertyName("sell_listings")]
        public int sell_listings { get; set; }

        [JsonPropertyName("sell_price")]
        public int sell_price { get; set; }

        [JsonPropertyName("sell_price_text")]
        public string sell_price_text { get; set; }

        [JsonPropertyName("app_icon")]
        public string app_icon { get; set; }

        [JsonPropertyName("app_name")]
        public string app_name { get; set; }

        [JsonPropertyName("asset_description")]
        public AssetDescription asset_description { get; set; }

        [JsonPropertyName("sale_price_text")]
        public string sale_price_text { get; set; }
    }

    public class ItemList
    {
        [JsonPropertyName("success")]
        public bool success { get; set; }

        [JsonPropertyName("start")]
        public int start { get; set; }

        [JsonPropertyName("pagesize")]
        public int pagesize { get; set; }

        [JsonPropertyName("total_count")]
        public int total_count { get; set; }

        [JsonPropertyName("searchdata")]
        public Searchdata searchdata { get; set; }

        [JsonPropertyName("results")]
        public List<Result> results { get; set; }
    }

    public class Searchdata
    {
        [JsonPropertyName("query")]
        public string query { get; set; }

        [JsonPropertyName("search_descriptions")]
        public bool search_descriptions { get; set; }

        [JsonPropertyName("total_count")]
        public int total_count { get; set; }

        [JsonPropertyName("pagesize")]
        public int pagesize { get; set; }

        [JsonPropertyName("prefix")]
        public string prefix { get; set; }

        [JsonPropertyName("class_prefix")]
        public string class_prefix { get; set; }
    }

}
