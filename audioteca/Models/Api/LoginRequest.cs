using System;
using Newtonsoft.Json;

namespace audioteca.Models.Api
{
    public class LoginRequest
    {
        [JsonProperty("user")]
        public int User { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
