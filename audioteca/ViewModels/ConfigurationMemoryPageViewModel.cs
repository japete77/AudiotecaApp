using audioteca.Helpers;
using audioteca.Models.Config;
using audioteca.Models.Player;
using System;
using System.Collections.Generic;
using System.Text;

namespace audioteca.ViewModels
{
    public class ConfigurationMemoryPageViewModel : ViewModelBase
    {
        private List<StorageDir> _storageDirs;
        public List<StorageDir> StorageDirs
        {
            get { return _storageDirs; }
            set
            {
                _storageDirs = value;
                RaisePropertyChanged();
            }
        }
    }
}
