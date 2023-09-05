using System;
using System.Text.Json.Serialization;

namespace SteamLibrary.JSONs;

public class Credentials
{
    [JsonPropertyName("login")]
    public string Login { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("secret")]
    public string Secret { get; set; }

    [JsonPropertyName("number")]
    public string Number { get; set; }

    public Credentials(string login, string password)
    {
        Login = login;
        Password = password;
    }
}
