using fonoteca.Helpers;
using fonoteca.Services;
using fonoteca.ViewModels;

namespace fonoteca.Pages
{
    public partial class MainPage : ContentPage
    {
        private readonly MainPageViewModel _vm;

        public MainPage(MainPageViewModel vm)
        {
            InitializeComponent();

            //if (DaisyPlayer._player == null)
            //{
            //    DaisyPlayer._player = MediaElement;
            //}
            //else
            //{
            //    MediaElement = DaisyPlayer._player;
            //}

            _vm = vm;
            _vm.VersionInfo = AppSettings.Instance.VersionInfo;
            BindingContext = _vm;

            // No bloquear aquí. Esperad a que la vista cargue.
            this.Loaded += async (_, __) =>
            {
                // Paso 1: flag rápido
                _vm.OnlineMode = OfflineChecker.HasInternetAccessFlag;

                // Paso 2: confirmación asíncrona (no bloquea UI)
                try
                {
                    _vm.OnlineMode = await OfflineChecker.IsConnectedAsync;
                }
                catch
                {
                    _vm.OnlineMode = false;
                }
            };

            // Reaccionar a cambios de conectividad
            Connectivity.ConnectivityChanged += (_, e) =>
            {
                _vm.OnlineMode = e.NetworkAccess == NetworkAccess.Internet;
            };

            DaisyPlayer.Instance.Attach(MediaElement);
        }

        // Evitad cerrar la app en iOS
        protected override bool OnBackButtonPressed()
        {
#if ANDROID
            System.Environment.Exit(0);
#else
            // En iOS no se debe cerrar programáticamente
#endif
            return true;
        }
    }
}
