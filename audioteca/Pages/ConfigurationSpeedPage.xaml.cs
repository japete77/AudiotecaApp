using audioteca.Models.Config;
using audioteca.Services;
using audioteca.ViewModels;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace audioteca
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfigurationSpeedPage : ContentPage
    {
        private readonly ConfigurationSpeedPageViewModel _model;

        public ConfigurationSpeedPage()
        {
            _model = new ConfigurationSpeedPageViewModel();
            this.BindingContext = _model;
            Title = "Velocidad";

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            _model.Speeds = new List<PlayerSpeed> {
                new PlayerSpeed { Label = "1", Speed = 1.0f },
                new PlayerSpeed { Label = "1.5", Speed = 1.5f },
                new PlayerSpeed { Label = "2", Speed = 2.0f },
                new PlayerSpeed { Label = "2.5", Speed = 2.5f },
                new PlayerSpeed { Label = "3", Speed = 3.0f }
            };
            listView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
            listView.BindingContext = _model.Speeds;
        }

        public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // has been set to null, do not 'process' tapped event
            if (e.SelectedItem == null) return;

            // de-select the row
            ((ListView)sender).SelectedItem = null;

            DaisyPlayer.Instance.SetSpeed(_model.Speeds[e.SelectedItemIndex].Speed);
            Session.Instance.SetSpeed(_model.Speeds[e.SelectedItemIndex].Speed);
            Session.Instance.SaveSession();

            await Navigation.PopAsync();
        }

        public async void GoToHome_Click(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}