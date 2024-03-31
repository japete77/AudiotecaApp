namespace fonoteca.Models.Api
{
    public class ChangePasswordRequest
    {
        public string Session { get; set; }

        public string NewPassword { get; set; }
    }
}
