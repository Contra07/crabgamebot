using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SteamLibrary.JSONs;

public static class JSONUtils
{
    public static List<Credentials> ParseUsers(string path)
    {
        JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            IncludeFields = true
        };
        return JsonSerializer.Deserialize<List<Credentials>>(File.ReadAllText(path), options);
    }

    public static List<InventoryItem> ParseInventory(string json)
    {
        return JsonSerializer.Deserialize<List<InventoryItem>>(json);
    }

    public static string SerializeInventoryItem(InventoryItem item)
    {
        JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            IncludeFields = true
        };
        return JsonSerializer.Serialize(item, options);
    }
}
