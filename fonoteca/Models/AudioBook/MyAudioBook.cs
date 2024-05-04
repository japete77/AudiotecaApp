using fonoteca.Models.Api;

namespace fonoteca.Models.Audiobook
{
    public class MyAudioBook
    {
        public MyAudioBook()
        {
        }
        public AudioBookDetailResult Book { get; set; }
        public string Path { get; set; }
        public string TmpFolder { get; set; }
        public string Filename { get; set; }
        public int Progress { get; set; }
        public string StatusDescription { get; set; }
        public int ErrorCode { get; set; }
        public string StatusKey { get; set; }
    }
}
