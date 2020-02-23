using System.Collections.ObjectModel;
using System.Windows.Input;
using audioteca.Models.Api;

namespace audioteca.ViewModels
{
    public class ByAuthorPageViewModel : ViewModelBase
    {
        private ObservableCollection<Grouping<string, AuthorModel>> _items = new ObservableCollection<Grouping<string, AuthorModel>>();
        public ObservableCollection<Grouping<string, AuthorModel>> Items
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
