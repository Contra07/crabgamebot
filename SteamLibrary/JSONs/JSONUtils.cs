using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SteamLibrary.JSONs
{
    public static class JSONUtils
    {
        public static List<Credentials> ParseUsers(string path)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                IncludeFields = true
            };
            return JsonSerializer.Deserialize<List<Credentials>>(File.ReadAllText(path), options);
        }

        public static List<InventoryItemDef> ParseInventoryItemDefs(string json)
        {
            return JsonSerializer.Deserialize<List<InventoryItemDef>>(json);
        }

        public static AssetMarketPrice ParseAssetMarketPrice(string json)
        {
            return JsonSerializer.Deserialize<AssetMarketPrice>(json);
        }

        public static List<GameItemDef> ParseGameItemDefs(string json)
        {
            return JsonSerializer.Deserialize<List<GameItemDef>>(json);
        }

        public static GameAssetsPage ParseGameAssetPage(string json)
        {
            return JsonSerializer.Deserialize<GameAssetsPage>(json);
        }

        public static InventoryAssets ParseInventoryAssets(string json)
        {
            return JsonSerializer.Deserialize<InventoryAssets>(json);
        }

        public static string SerializeInventoryItem(InventoryItemDef item)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                IncludeFields = true
            };
            return JsonSerializer.Serialize(item, options);
        }
    }
}