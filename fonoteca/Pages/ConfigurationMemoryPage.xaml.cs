using fonoteca.ViewModels;
using fonoteca.Services;
using fonoteca.Helpers;

namespace fonoteca.Pages;

public partial class ConfigurationMemoryPage : ContentPage
{
	private ConfigurationMemoryPageViewModel _vm;

    public ConfigurationMemoryPage(ConfigurationMemoryPageViewModel vm)
	{
		InitializeComponent();        
		_vm = vm;
        _vm.Items = AudioBookDataDir.StorageDirs;
		BindingContext = _vm;
	}

    public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // has been set to null, do not 'process' tapped event
        if (e.SelectedItem == null) return;

        // de-select the row
        ((ListView)sender).SelectedItem = null;

        var selectedStorage = _vm.Items[e.SelectedItemIndex];

        // Ask the user to confirm data dir switch and move all the audiobooks folders...
        if (Session.Instance.GetDataDir() != selectedStorage.AbsolutePath)
        {
            var response = await DisplayAlert(
                "Aviso", 
                $"Esto moverá los audio libros a la {selectedStorage.Name}, dependiendo del tamaño puede tomar unos minutos ¿desea continuar?", 
                "Si", 
                "No"
            );

            if (response)
            {
                // Stop to prevent locked files
                if (DaisyPlayer.HasBeenInitialized) DaisyPlayer.Instance.Stop();

                // Run move
                await MoveData(Session.Instance.GetDataDir(), selectedStorage);
            }
        }

        await Shell.Current.Navigation.PopAsync();
    }

    private async Task MoveData(string source, StorageDir target)
    {
        //UserDialogs.Instance.ShowLoading("Calculando espacio");

        // Check disk space
        var sourceDir = Session.Instance.GetDataDir();
        var files = Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories);

        long totalSize = 0;
        for (int i = 0; i < files.Length; i++)
        {
            totalSize += new FileInfo(files[i]).Length;
        }
        //UserDialogs.Instance.HideLoading();

        if (target.FreeSpace < totalSize)
        {
            await DisplayAlert("Error", "No hay espacio suficiente en el almacenamiento destino para mover los audio libros", "OK");
            return;
        }

        // Move data
        DirectoryInfo dirInfo = new DirectoryInfo(target.AbsolutePath);
        var audioBookDirs = Directory.GetDirectories(source);

        //var progress = UserDialogs.Instance.Loading($"Moviendo audio libros");

        for (int i = 0; i < audioBookDirs.Length; i++)
        {
            var tmp = $"{target.AbsolutePath}/{Path.GetFileName(audioBookDirs[i])}/";

            if (!Directory.Exists(tmp)) Directory.CreateDirectory(tmp);

            var abookFiles = Directory.GetFiles(audioBookDirs[i]);
            for (int j = 0; j < abookFiles.Length; j++)
            {
                //progress.Title = $"Moviendo audio libros {(int)(i * 100 / audioBookDirs.Length) + (int)(j * 100 / (abookFiles.Length * audioBookDirs.Length))}%";

                File.Copy(abookFiles[j], $"{tmp}/{Path.GetFileName(abookFiles[j])}", true);
            }

            Directory.Delete(audioBookDirs[i], true);
        }

        //progress.Hide();

        // Update storage
        Session.Instance.SetDataDir(target.AbsolutePath);
        Session.Instance.SaveSession();

        //UserDialogs.Instance.HideLoading();
    }
}