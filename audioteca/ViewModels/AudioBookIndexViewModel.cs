using audioteca.Models.Daisy;
using System;
using System.Collections.Generic;
using System.Text;

namespace audioteca.ViewModels
{
    public class AudioBookIndexViewModel : ViewModelBase
    {
        private List<SmilInfo> _items;
        public List<SmilInfo> Items
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
