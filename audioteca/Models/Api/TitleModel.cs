using audioteca.Helpers;
using System;
namespace audioteca.Models.Api
{
    public class TitleModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string AuthorId { get; set; }
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
