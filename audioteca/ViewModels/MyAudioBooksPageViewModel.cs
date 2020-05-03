using audioteca.Models.Audiobook;
using System.Collections.ObjectModel;

namespace audioteca.ViewModels
{
    public class MyAudioBooksPageViewModel : ViewModelBase
    {
        private ObservableCollection<Grouping<string, MyAudioBook>> _items = new ObservableCollection<Grouping<string, MyAudioBook>>();
        public ObservableCollection<Grouping<string, MyAudioBook>> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                RaisePropertyChanged();
            }
        }

        private bool _loading;
        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged();
            }
        }
    }
}
