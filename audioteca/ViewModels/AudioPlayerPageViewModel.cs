using System;
using System.Collections.Generic;
using System.Text;

namespace audioteca.ViewModels
{
    public class AudioPlayerPageViewModel : ViewModelBase
    {
        private bool _loading = false;
        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged();
            }
        }

        private string _currentTC;
        public string CurrentTC
        {
            get { return _currentTC; }
            set
            {
                _currentTC = value;
                RaisePropertyChanged();
            }
        }

        private string _playStopCaption;
        public string PlayStopCaption
        {
            get { return _playStopCaption; }
            set
            {
                _playStopCaption = value;
                RaisePropertyChanged();
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged();
            }
        }

        private string _chapter;
        public string Chapter
        {
            get { return _chapter; }
            set
            {
                _chapter = value;
                RaisePropertyChanged();
            }
        }

        private string _navigationLevel;
        public string NavigationLevel
        {
            get { return _navigationLevel; }
            set
            {
                _navigationLevel = value;
                RaisePropertyChanged();
            }
        }
    }
}
