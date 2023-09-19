using System;
using System.Text.Json.Serialization;

namespace SteamLibrary.Models
{
    public class AccountCredentialsModel
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

        public AccountCredentialsModel()
        {
            
        }
        public AccountCredentialsModel(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public AccountCredentialsModel(string login, string password, string email, string secret, string number)
        {
            Login = login;
            Password = password;
            Email = email;
            Secret = secret;
            Number = number;
        }
    }
}


