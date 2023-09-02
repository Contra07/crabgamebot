using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SteamLibrary;

public static class JSONUtils
{
    public static List<Account> ReadUsers(string path){
        return JsonSerializer.Deserialize<List<Account>>(File.ReadAllText(path));
    }
}
