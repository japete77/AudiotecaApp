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
    }
}
