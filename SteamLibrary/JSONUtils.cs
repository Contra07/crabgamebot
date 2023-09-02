using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SteamLibrary;

public static class JSONUtils
{
    public static List<Credentials> ReadUsers(string path){
        JsonSerializerOptions options = new (){
            WriteIndented = true,
            IncludeFields = true
        };
        return JsonSerializer.Deserialize<List<Credentials>>(File.ReadAllText(path), options);
    }
}
