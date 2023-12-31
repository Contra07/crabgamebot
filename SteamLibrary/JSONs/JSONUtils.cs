﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using SteamLibrary.Models;

namespace SteamLibrary.JSONs
{
    public static class JSONUtils
    {
        public static List<AccountCredentialsModel> ParseUsers(string path)
        {
            return JsonSerializer.Deserialize<List<AccountCredentialsModel>>(File.ReadAllText(path));
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