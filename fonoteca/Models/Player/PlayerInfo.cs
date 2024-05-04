using CommunityToolkit.Maui.Core.Primitives;

namespace fonoteca.Models.Player
{
    public class PlayerInfo
    {
        public MediaElementState Status { get; set; }
        // public string Filename { get; set; }
        public string CurrentFilename { get; set; }
        public SeekInfo Position { get; set; }
        public List<Bookmark> Bookmarks { get; set; }
    }
}
