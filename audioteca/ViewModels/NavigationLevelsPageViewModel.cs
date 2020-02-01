using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace audioteca.ViewModels
{
    public class NavigationLevelsPageViewModel : ViewModelBase
    {
        public class NavigationLevel
        {
            public int Level { get; set; }
            public string Label { get; set; }
        }

        private ObservableCollection<NavigationLevel> _items = new ObservableCollection<NavigationLevel>();
        public ObservableCollection<NavigationLevel> Items
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
