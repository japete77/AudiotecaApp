using System;
using System.Collections.ObjectModel;
using audioteca.Models.Api;

namespace audioteca.ViewModels
{
    public class SubscriptionTitlesPageViewModel : ViewModelBase
    {
        private ObservableCollection<SubscriptionTitle> _items = new ObservableCollection<SubscriptionTitle>();
        public ObservableCollection<SubscriptionTitle> Items
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
