namespace audioteca.ViewModels
{
    public class ConfigurationPageViewModel : ViewModelBase
    {
        private double _speed;
        public double Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
                RaisePropertyChanged();
            }
        }
    }
}
