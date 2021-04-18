using audioteca.Models.Api;
using System.Collections.ObjectModel;

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

        private bool _showMarkAllRead;
        public bool ShowMarkAllRead
        {
            get { return _showMarkAllRead; }
            set
            {
                _showMarkAllRead = value;
                RaisePropertyChanged();
            }
        }
    }
}
