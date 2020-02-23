using audioteca.Helpers;
using audioteca.Models.Config;
using System.Collections.Generic;

namespace audioteca.ViewModels
{
    public class ConfigurationSpeedPageViewModel : ViewModelBase
    {
        private List<PlayerSpeed> _speeds;
        public List<PlayerSpeed> Speeds
        {
            get { return _speeds; }
            set
            {
                _speeds = value;
                RaisePropertyChanged();
            }
        }
    }
}
