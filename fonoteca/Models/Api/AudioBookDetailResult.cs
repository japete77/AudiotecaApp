using fonoteca.Helpers;

namespace fonoteca.Models.Api
{
    public class AudioBookDetailResult
    {
        public string Id { get; set; }
        public AuthorModel Author { get; set; }
        public string Comments { get; set; }
        public string Editorial { get; set; }
        public int LengthHours { get; set; }
        public int LengthMins { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
        public string TitleSort
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Title) || Title.Length == 0)
                    return "?";

                return TextHelper.RemoveDiacritics(Title[0].ToString()).ToUpper();
            }
        }
    }
}
