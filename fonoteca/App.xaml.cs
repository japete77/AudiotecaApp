namespace fonoteca
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = new Window(new AppShell());

            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                // Establecer tamaño de la ventana
                window.Width = 400; // Ancho en píxeles
                window.Height = 700; // Alto en píxeles

                var display = DeviceDisplay.MainDisplayInfo;
                var screenWidthDip = display.Width / display.Density;
                var screenHeightDip = display.Height / display.Density;

                window.X = (screenWidthDip - window.Width) / 2;
                window.Y = (screenHeightDip - window.Height) / 2;
            }

            return window;
        }
    }
}
