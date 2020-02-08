using Acr.UserDialogs;
using audioteca.Helpers;
using audioteca.Services;
using audioteca.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace audioteca
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfigurationPage : ContentPage
    {
        private ConfigurationPageViewModel _model { get; }

        public ConfigurationPage()
        {
            _model = new ConfigurationPageViewModel();
            BindingContext = _model;
            _model.Items = new ObservableCollection<StorageDir>(AudioBookDataDir.StorageDirs);
            Title = "Configuración";
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            _model.Speed = Session.Instance.GetSpeed();
            var storage = _model.Items
                .Where(w => w.AbsolutePath == Session.Instance.GetDataDir())
                .FirstOrDefault();
            _model.Storage = storage;
        }

        public async void GoToHome_Click(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        private void pickerSpeed_SelectedIndexChanged(object sender, EventArgs e)
        {
            DaisyPlayer.Instance.SetSpeed((float)_model.Speed);
            Session.Instance.SetSpeed(_model.Speed);
            Session.Instance.SaveSession();
        }

        private void pickerStorage_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Ask the user to confirm data dir switch and move all the audiobooks folders...
            if (Session.Instance.GetDataDir() != _model.Storage.AbsolutePath)
            {
                UserDialogs.Instance.Confirm(
                    new ConfirmConfig
                    {
                        Title = "Aviso",
                        Message = $"Esto moverá los audio libros al almacenamiento seleccionado, dependiendo del tamaño puede tomar unos minutos ¿desea continuar?",
                        OkText = "Si",
                        CancelText = "No",
                        OnAction = (action) =>
                        {
                            if (action)
                            {
                                // Stop to prevent locked files
                                DaisyPlayer.Instance.Stop();

                                // Run move
                                Task.Run(() => MoveData(Session.Instance.GetDataDir(), _model.Storage.AbsolutePath));
                            }
                            else
                            {
                                _model.Storage = AudioBookDataDir.StorageDirs.Where(w => w.AbsolutePath == Session.Instance.GetDataDir()).First();
                            }
                        }
                    }
                );
            }
        }

        private void MoveData(string source, string target)
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

            if (_model.Storage.FreeSpace < totalSize)
            {
                UserDialogs.Instance.Alert(new AlertConfig
                {
                    Title = "Error",
                    Message = "No hay espacio suficiente en el almacenamiento destino para mover los audio libros"
                });

                _model.Storage = AudioBookDataDir.StorageDirs.Where(w => w.AbsolutePath == Session.Instance.GetDataDir()).First();

                return;
            }

            // Move data
            DirectoryInfo dirInfo = new DirectoryInfo(target);
            var audioBookDirs = Directory.GetDirectories(source);

            var progress = UserDialogs.Instance.Loading($"Moviendo audio libros");

            for (int i = 0; i < audioBookDirs.Length; i++)
            {                
                var tmp = $"{target}/{Path.GetFileName(audioBookDirs[i])}/";

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
            Session.Instance.SetDataDir(_model.Storage.AbsolutePath);
            Session.Instance.SaveSession();
            
            UserDialogs.Instance.HideLoading();
        }

        public void ButtonClick_ClearCredentials(object sender, EventArgs e)
        {
            UserDialogs.Instance.Confirm(
                new ConfirmConfig
                {
                    Title = "Aviso",
                    Message = $"Esto limpiará las credenciales de usuario y tendrá que iniciar sesión de nuevo con un usuario y contraseña ¿desea continuar?",
                    OkText = "Si",
                    CancelText = "No",
                    OnAction = (action) =>
                    {
                        if (action)
                        {
                            Session.Instance.CleanCredentials();
                            Session.Instance.SaveSession();
                        }
                    }
                }
            );
        }

    }
}