using Newtonsoft.Json;

namespace fonoteca.Models.Api
{
    public class LoginRequest
    {
        [JsonProperty("user")]
        public int User { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
