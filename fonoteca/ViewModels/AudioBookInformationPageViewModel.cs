using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace fonoteca.ViewModels
{
    public partial class AudioBookInformationPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private string id;

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string creator;

        [ObservableProperty]
        private string date;

        [ObservableProperty]
        private string format;

        [ObservableProperty]
        private string identifier;

        [ObservableProperty]
        private string publisher;

        [ObservableProperty]
        private string subject;

        [ObservableProperty]
        private string source;

        [ObservableProperty]
        private string charset;

        [ObservableProperty]
        private string generator;

        [ObservableProperty]
        private string narrator;

        [ObservableProperty]
        private string producer;

        [ObservableProperty]
        private string totalTime;

        [RelayCommand]
        void DeleteBook()
        {
            // TODO: Add MAUI user dialog

            //UserDialogs.Instance.Confirm(
            //    new ConfirmConfig
            //    {
            //        Title = "Aviso",
            //        Message = "Esto eliminará completamente el audio libro del dispositivo y todos los marcadores asociados ¿desea continuar?",
            //        OkText = "Si",
            //        CancelText = "No",
            //        OnAction = async (action) =>
            //        {
            //            if (action)
            //            {
            //                await DaisyPlayer.Instance.Stop();

            //                var daisyBook = DaisyPlayer.Instance.GetDaisyBook();

            //                await AudioBookStore.Instance.Delete(daisyBook.Id);

            //                DaisyPlayer.Instance.CleanupPlayerInfo();

            //                DaisyPlayer.Instance.CleanupDaisyBook();

            //                await Navigation.PopToRootAsync(true);
            //            }
            //        }
            //    }
            //);
        }

        [RelayCommand]
        public async Task GoToBack()
        {
            await Shell.Current.Navigation.PopAsync(true);
        }
    }
}
