using System;
namespace audioteca.Models.Api
{
    public class LoginResult
    {
        public string Message { get; set; }
        public string Session { get; set; }
        public bool Success { get; set; }
    }
}
