namespace audioteca.ViewModels
{
    public class ChangePasswordPageViewModel : ViewModelBase
    {
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

        private string _confirmNewPassword;
        public string ConfirmNewPassword
        {
            get { return _confirmNewPassword; }
            set
            {
                _confirmNewPassword = value;
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
