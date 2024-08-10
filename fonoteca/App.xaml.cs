namespace fonoteca
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                // Establecer tamaño de la ventana
                window.Width = 400; // Ancho en píxeles
                window.Height = 700; // Alto en píxeles

                // También puedes centrar la ventana en la pantalla
                window.X = (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density - window.Width) / 2;
                window.Y = (DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density - window.Height) / 2;
            }

            return window;
        }
    }
}
