using System;
using System.Collections.ObjectModel;
using audioteca.Models.Api;

namespace audioteca.ViewModels
{
    public class NotificationsPageViewModel : ViewModelBase
    {
        private ObservableCollection<NotificationModel> _items = new ObservableCollection<NotificationModel>();
        public ObservableCollection<NotificationModel> Items
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
