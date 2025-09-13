using System.ComponentModel;

namespace fonoteca.Services
{
    public class PlayerMetadata : INotifyPropertyChanged
    {
        string title, chapter;

        public string Title
        {
            get => title;
            set { if (title != value) { title = value; PropertyChanged?.Invoke(this, new(nameof(Title))); } }
        }

        public string Chapter
        {
            get => chapter;
            set { if (chapter != value) { chapter = value; PropertyChanged?.Invoke(this, new(nameof(Chapter))); } }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
