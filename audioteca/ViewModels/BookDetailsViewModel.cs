using System;
using audioteca.Models.Api;

namespace audioteca.ViewModels
{
    public class BookDetailsViewModel : ViewModelBase
    {
        private AudioBookDetailResult _audioBook;
        public AudioBookDetailResult AudioBook
        {
            get { return _audioBook; }
            set
            {
                _audioBook = value;
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

        private string _statusDescription;
        public string StatusDescription
        {
            get { return _statusDescription; }
            set
            {
                _statusDescription = value;
                RaisePropertyChanged();
            }
        }

        private bool _showDownload;
        public bool ShowDownload
        {
            get { return _showDownload; }
            set
            {
                _showDownload = value;
                RaisePropertyChanged();
            }
        }

        private bool _showCancel;
        public bool ShowCancel
        {
            get { return _showCancel; }
            set
            {
                _showCancel = value;
                RaisePropertyChanged();
            }
        }

        private bool _showListen;
        public bool ShowListen
        {
            get { return _showListen; }
            set
            {
                _showListen = value;
                RaisePropertyChanged();
            }
        }

        private bool _showStatus;
        public bool ShowStatus
        {
            get { return _showStatus; }
            set
            {
                _showStatus = value;
                RaisePropertyChanged();
            }
        }
    }
}
