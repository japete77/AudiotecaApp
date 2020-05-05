using Acr.UserDialogs;
using audioteca.Services;
using audioteca.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace audioteca
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AudioBookInformationPage : ContentPage
    {
        private readonly AudioBookInformationViewModel _model;

        public AudioBookInformationPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            _model = new AudioBookInformationViewModel();
            this.BindingContext = _model;

            var daisyBook = DaisyPlayer.Instance.GetDaisyBook();
            if (daisyBook != null)
            {
                _model.Charset = daisyBook.Charset;
                _model.Creator = daisyBook.Creator;
                _model.Date = daisyBook.Date;
                _model.Format = daisyBook.Format;
                _model.Generator = daisyBook.Generator;
                _model.Id = daisyBook.Id;
                _model.Identifier = daisyBook.Identifier;
                _model.Narrator = daisyBook.Narrator;
                _model.Producer = daisyBook.Producer;
                _model.Publisher = daisyBook.Publisher;
                _model.Source = daisyBook.Source;
                _model.Subject = daisyBook.Subject;
                _model.Title = daisyBook.Title;
                _model.TotalTime = daisyBook.TotalTime;
            }

            InitializeComponent();
        }

        public void ButtonClick_Delete(object sender, EventArgs e)
        {
            UserDialogs.Instance.Confirm(
                new ConfirmConfig
                {
                    Title = "Aviso",
                    Message = "Esto eliminará completamente el audio libro del dispositivo y todos los marcadores asociados ¿desea continuar?",
                    OkText = "Si",
                    CancelText = "No",
                    OnAction = async (action) =>
                    {
                        if (action)
                        {
                            DaisyPlayer.Instance.Stop();

                            var daisyBook = DaisyPlayer.Instance.GetDaisyBook();

                            await AudioBookStore.Instance.Delete(daisyBook.Id);

                            DaisyPlayer.Instance.CleanupPlayerInfo();

                            await Navigation.PopToRootAsync(true);
                        }
                    }
                }
            );
        }

        private async void ButtonClick_Back(object sender, EventArgs e)
        {
            await this.Navigation.PopAsync();
        }
    }
}