using System;
namespace audioteca.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private string _versionInfo;
        public string VersionInfo
        {
            get { return _versionInfo; }
            set
            {
                _versionInfo = value;
                RaisePropertyChanged();
            }
        }

    }
}
