using audioteca.Helpers;
using System.Collections.ObjectModel;

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

        private StorageDir _storage;
        public StorageDir Storage
        {
            get { return _storage; }
            set
            {
                _storage = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<StorageDir> _items = new ObservableCollection<StorageDir>();
        public ObservableCollection<StorageDir> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                RaisePropertyChanged();
            }
        }
    }
}
