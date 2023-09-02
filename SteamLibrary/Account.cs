using System;
using System.Text.Json.Serialization;

namespace SteamLibrary;

public class Account
{
    [JsonPropertyName("login")]
    public string Login {get; set; }
    [JsonPropertyName("password")]
    public string Password {get; set; }
    public Account(string login, string password)
    {
        Login = login;
        Password = password;
    }
}
