namespace audioteca.ViewModels
{
    public class AudioLibraryPageViewModel : ViewModelBase
    {
        private bool _authenticated = false;
        public bool Authenticated
        {
            get { return _authenticated; }
            set
            {
                _authenticated = value;
                RaisePropertyChanged();
            }
        }

        private bool _isAccesible = false;
        public bool IsAccesible
        {
            get { return _isAccesible; }
            set
            {
                _isAccesible = value;
                RaisePropertyChanged();
            }
        }

        private bool _mustChangePassword = false;
        public bool MustChangePassword
        {
            get { return _mustChangePassword; }
            set
            {
                _mustChangePassword = value;
                RaisePropertyChanged();
            }
        }

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

        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                RaisePropertyChanged();
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                RaisePropertyChanged();
            }
        }

        private string _newPassword;
        public string NewPassword
        {
            get { return _newPassword; }
            set
            {
                _newPassword = value;
                RaisePropertyChanged();
            }
        }

        private string _reNewPassword;
        public string ReNewPassword
        {
            get { return _reNewPassword; }
            set
            {
                _reNewPassword = value;
                RaisePropertyChanged();
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                RaisePropertyChanged();
            }
        }
    }
}
