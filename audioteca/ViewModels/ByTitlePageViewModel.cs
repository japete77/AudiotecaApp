using System.Collections.ObjectModel;
using System.Windows.Input;
using audioteca.Models.Api;

namespace audioteca.ViewModels
{
    public class ByTitlePageViewModel : ViewModelBase
    {
        private ObservableCollection<TitleModel> _items = new ObservableCollection<TitleModel>();
        public ObservableCollection<TitleModel> Items
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
    }
}
