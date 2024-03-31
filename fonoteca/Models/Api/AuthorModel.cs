using fonoteca.Helpers;

namespace fonoteca.Models.Api
{
    public class AuthorModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NameSort
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name) || Name.Length == 0)
                    return "?";

                return TextHelper.RemoveDiacritics(Name[0].ToString()).ToUpper();
            }
        }
    }
}
