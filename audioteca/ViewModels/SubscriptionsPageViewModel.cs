using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using audioteca.Models.Api;

namespace audioteca.ViewModels
{
    public class SubscriptionsPageViewModel : ViewModelBase
    {
        private ObservableCollection<Subscription> _items = new ObservableCollection<Subscription>();
        public ObservableCollection<Subscription> Items
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
