using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SteamLibrary.JSONs
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class InventoryAsset
    {
        [JsonPropertyName("appid")]
        public int? appid { get; set; }

        [JsonPropertyName("contextid")]
        public int? contextid { get; set; }

        [JsonPropertyName("assetid")]
        public ulong assetid { get; set; }

        [JsonPropertyName("classid")]
        public ulong classid { get; set; }

        [JsonPropertyName("instanceid")]
        public ulong instanceid { get; set; }

        [JsonPropertyName("currencyid")]
        public int? currencyid { get; set; }

        [JsonPropertyName("amount")]
        public int? amount { get; set; }

        [JsonPropertyName("missing")]
        public bool? missing { get; set; }

        [JsonPropertyName("est_usd")]
        public int? est_usd { get; set; }
    }

    public class InventoryDescription
    {
        [JsonPropertyName("appid")]
        public int? appid { get; set; }

        [JsonPropertyName("classid")]
        public ulong classid { get; set; }

        [JsonPropertyName("instanceid")]
        public ulong instanceid { get; set; }

        [JsonPropertyName("currency")]
        public bool? currency { get; set; }

        [JsonPropertyName("background_color")]
        public string background_color { get; set; }

        [JsonPropertyName("icon_url")]
        public string icon_url { get; set; }

        [JsonPropertyName("icon_url_large")]
        public string icon_url_large { get; set; }

        [JsonPropertyName("descriptions")]
        public List<InventoryDescription> descriptions { get; set; }

        [JsonPropertyName("tradable")]
        public bool? tradable { get; set; }

        [JsonPropertyName("actions")]
        public List<object> actions { get; set; }

        [JsonPropertyName("owner_descriptions")]
        public List<OwnerDescription> owner_descriptions { get; set; }

        [JsonPropertyName("owner_actions")]
        public List<object> owner_actions { get; set; }

        [JsonPropertyName("fraudwarnings")]
        public List<object> fraudwarnings { get; set; }

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

        [JsonPropertyName("market_fee")]
        public string market_fee { get; set; }

        [JsonPropertyName("market_fee_app")]
        public int? market_fee_app { get; set; }

        [JsonPropertyName("contained_item")]
        public object contained_item { get; set; }

        [JsonPropertyName("market_actions")]
        public List<object> market_actions { get; set; }

        [JsonPropertyName("commodity")]
        public bool? commodity { get; set; }

        [JsonPropertyName("market_tradable_restriction")]
        public int? market_tradable_restriction { get; set; }

        [JsonPropertyName("market_marketable_restriction")]
        public int? market_marketable_restriction { get; set; }

        [JsonPropertyName("marketable")]
        public bool? marketable { get; set; }

        [JsonPropertyName("tags")]
        public List<Tag> tags { get; set; }

        [JsonPropertyName("item_expiration")]
        public string item_expiration { get; set; }

        [JsonPropertyName("market_buy_country_restriction")]
        public string market_buy_country_restriction { get; set; }

        [JsonPropertyName("market_sell_country_restriction")]
        public string market_sell_country_restriction { get; set; }

        [JsonPropertyName("value")]
        public string value { get; set; }

        [JsonPropertyName("color")]
        public string color { get; set; }

        [JsonPropertyName("label")]
        public string label { get; set; }
    }

    public class OwnerDescription
    {
        [JsonPropertyName("type")]
        public string type { get; set; }

        [JsonPropertyName("value")]
        public string value { get; set; }

        [JsonPropertyName("color")]
        public string color { get; set; }

        [JsonPropertyName("label")]
        public string label { get; set; }
    }

    public class InventoryAssets
    {
        [JsonPropertyName("assets")]
        public List<InventoryAsset> assets { get; set; }

        [JsonPropertyName("descriptions")]
        public List<InventoryDescription> descriptions { get; set; }

        [JsonPropertyName("missing_assets")]
        public List<object> missing_assets { get; set; }

        [JsonPropertyName("more_items")]
        public bool? more_items { get; set; }

        [JsonPropertyName("last_assetid")]
        public int? last_assetid { get; set; }

        [JsonPropertyName("total_inventory_count")]
        public int? total_inventory_count { get; set; }
    }

    public class Tag
    {
        [JsonPropertyName("appid")]
        public int? appid { get; set; }

        [JsonPropertyName("category")]
        public string category { get; set; }

        [JsonPropertyName("internal_name")]
        public string internal_name { get; set; }

        [JsonPropertyName("localized_category_name")]
        public string localized_category_name { get; set; }

        [JsonPropertyName("localized_tag_name")]
        public string localized_tag_name { get; set; }

        [JsonPropertyName("color")]
        public string color { get; set; }
    }
}
