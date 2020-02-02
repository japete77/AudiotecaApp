using audioteca.Models.Player;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace audioteca.ViewModels
{
    public class BookmarksPageViewModel : ViewModelBase
    {
        private ObservableCollection<Bookmark> _items = new ObservableCollection<Bookmark>();
        public ObservableCollection<Bookmark> Items
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
