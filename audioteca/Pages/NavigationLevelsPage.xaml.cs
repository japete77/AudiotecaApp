﻿using Acr.UserDialogs;
using audioteca.Services;
using audioteca.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static audioteca.ViewModels.NavigationLevelsPageViewModel;

namespace audioteca
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NavigationLevelsPage : ContentPage
    {
        private readonly NavigationLevelsPageViewModel _model;

        public NavigationLevelsPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            _model = new NavigationLevelsPageViewModel();
            this.BindingContext = _model;
            _model.Loading = true;

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            _model.Items = new ObservableCollection<NavigationLevel>();
            var levels = DaisyPlayer.Instance.GetNavigationLevels();
            levels.ForEach(item => _model.Items.Add(item));

            listView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
            listView.BindingContext = _model.Items;

            _model.Loading = false;
        }

        public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // has been set to null, do not 'process' tapped event
            if (e.SelectedItem == null) return;

            // de-select the row
            ((ListView)sender).SelectedItem = null;

            var level = (e.SelectedItem as NavigationLevel).Level;

            DaisyPlayer.Instance.SetLevel(level);

            await Navigation.PopAsync(true);
        }

        private async void ButtonClick_Back(object sender, EventArgs e)
        {
            await this.Navigation.PopAsync();
        }
    }
}