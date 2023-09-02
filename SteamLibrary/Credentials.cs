using System;
using System.Text.Json.Serialization;

namespace SteamLibrary;

public class Credentials
{
    [JsonPropertyName("login")]
    public string Login {get; set; } 

    [JsonPropertyName("password")]
    public string Password {get; set; }

    public Credentials(string login, string password)
    {
        Login = login;
        Password = password;
    }
}
