using System;
using audioteca.Models.Api;

namespace audioteca.ViewModels
{
    public class NotificationDetailPageViewModel : ViewModelBase
    {
        private NotificationModel _notification = new NotificationModel();
        public NotificationModel Notification
        {
            get { return _notification; }
            set
            {
                _notification = value;
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

        private bool _showGoDetails;
        public bool ShowGoDetails
        {
            get { return _showGoDetails; }
            set
            {
                _showGoDetails = value;
                RaisePropertyChanged();
            }
        }
        
    }
}
