using System.Collections.ObjectModel;
using System.Windows.Input;
using audioteca.Models.Api;

namespace audioteca.ViewModels
{
    public class ByTitlePageViewModel : ViewModelBase
    {
        private ObservableCollection<Grouping<string, TitleModel>> _items = new ObservableCollection<Grouping<string, TitleModel>>();
        public ObservableCollection<Grouping<string, TitleModel>> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                RaisePropertyChanged();
            }
        }

        private bool _searchText;
        public bool SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
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
