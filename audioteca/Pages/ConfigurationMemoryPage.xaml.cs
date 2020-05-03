using Acr.UserDialogs;
using audioteca.Helpers;
using audioteca.Services;
using audioteca.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace audioteca
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfigurationMemoryPage : ContentPage
    {
        private readonly ConfigurationMemoryPageViewModel _model;

        public ConfigurationMemoryPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            _model = new ConfigurationMemoryPageViewModel();
            this.BindingContext = _model;

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            _model.StorageDirs = AudioBookDataDir.StorageDirs;
            listView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
            listView.BindingContext = _model.StorageDirs;
        }

        public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // has been set to null, do not 'process' tapped event
            if (e.SelectedItem == null) return;

            // de-select the row
            ((ListView)sender).SelectedItem = null;

            var selectedStorage = _model.StorageDirs[e.SelectedItemIndex];

            // Ask the user to confirm data dir switch and move all the audiobooks folders...
            if (Session.Instance.GetDataDir() != selectedStorage.AbsolutePath)
            {
                UserDialogs.Instance.Confirm(
                    new ConfirmConfig
                    {
                        Title = "Aviso",
                        Message = $"Esto moverá los audio libros a la {selectedStorage.Name}, dependiendo del tamaño puede tomar unos minutos ¿desea continuar?",
                        OkText = "Si",
                        CancelText = "No",
                        OnAction = (action) =>
                        {
                            if (action)
                            {
                                // Stop to prevent locked files
                                DaisyPlayer.Instance.Stop();

                                // Run move
                                Task.Run(() => MoveData(Session.Instance.GetDataDir(), selectedStorage));
                            }
                        }
                    }
                );
            }

            await Navigation.PopAsync();
        }

        public async void GoToHome_Click(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        private void MoveData(string source, StorageDir target)
        {
            UserDialogs.Instance.ShowLoading("Calculando espacio");

            // Check disk space
            var sourceDir = Session.Instance.GetDataDir();
            var files = Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories);

            long totalSize = 0;
            for (int i = 0; i < files.Length; i++)
            {
                totalSize += new FileInfo(files[i]).Length;
            }
            UserDialogs.Instance.HideLoading();

            if (target.FreeSpace < totalSize)
            {
                UserDialogs.Instance.Alert(new AlertConfig
                {
                    Title = "Error",
                    Message = "No hay espacio suficiente en el almacenamiento destino para mover los audio libros"
                });

                return;
            }

            // Move data
            DirectoryInfo dirInfo = new DirectoryInfo(target.AbsolutePath);
            var audioBookDirs = Directory.GetDirectories(source);

            var progress = UserDialogs.Instance.Loading($"Moviendo audio libros");

            for (int i = 0; i < audioBookDirs.Length; i++)
            {
                var tmp = $"{target.AbsolutePath}/{Path.GetFileName(audioBookDirs[i])}/";

                if (!Directory.Exists(tmp)) Directory.CreateDirectory(tmp);

                var abookFiles = Directory.GetFiles(audioBookDirs[i]);
                for (int j = 0; j < abookFiles.Length; j++)
                {
                    progress.Title = $"Moviendo audio libros {(int)(i * 100 / audioBookDirs.Length) + (int)(j * 100 / (abookFiles.Length * audioBookDirs.Length))}%";

                    File.Copy(abookFiles[j], $"{tmp}/{Path.GetFileName(abookFiles[j])}", true);
                }

                Directory.Delete(audioBookDirs[i], true);
            }

            progress.Hide();

            // Update storage
            Session.Instance.SetDataDir(target.AbsolutePath);
            Session.Instance.SaveSession();

            UserDialogs.Instance.HideLoading();
        }

        private async void ButtonClick_Back(object sender, EventArgs e)
        {
            await this.Navigation.PopAsync();
        }
    }
}