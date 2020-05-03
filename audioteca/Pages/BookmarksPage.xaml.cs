using Acr.UserDialogs;
using audioteca.Models.Player;
using audioteca.Services;
using audioteca.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace audioteca
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookmarksPage : ContentPage
    {
        private readonly BookmarksPageViewModel _model;

        public BookmarksPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            _model = new BookmarksPageViewModel();
            this.BindingContext = _model;
            _model.Loading = true;

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            var playerInfo = DaisyPlayer.Instance.GetPlayerInfo();

            if (playerInfo != null && playerInfo.Bookmarks != null)
            {
                _model.Items = new ObservableCollection<Bookmark>();
                playerInfo.Bookmarks.ForEach(item => _model.Items.Add(item));
                listView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
                listView.BindingContext = _model.Items;
            }

            _model.Loading = false;
        }

        public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // has been set to null, do not 'process' tapped event
            if (e.SelectedItem == null) return;

            // de-select the row
            ((ListView)sender).SelectedItem = null;

            var selectedItem = (e.SelectedItem as Bookmark);

            await DaisyPlayer.Instance.Seek(selectedItem.Index, selectedItem.TC);

            await Navigation.PopAsync(true);
        }

        public void ButtonClick_Delete(object sender, EventArgs e)
        {
            var button = (sender as Button);
            int id = (int)button.CommandParameter;

            var playerInfo = DaisyPlayer.Instance.GetPlayerInfo();
            var toDelete = playerInfo.Bookmarks.Where(w => w.Id == id).FirstOrDefault();
            if (toDelete != null)
            {
                UserDialogs.Instance.Confirm(
                    new ConfirmConfig
                    {
                        Title = "Aviso",
                        Message = $"Esto eliminará el marcador {toDelete.Title} ¿desea continuar?",
                        OkText = "Si",
                        CancelText = "No",
                        OnAction = (action) =>
                        {
                            if (action)
                            {
                                DaisyPlayer.Instance.RemoveBookmark(toDelete);
                                _model.Items.Remove(toDelete);
                            }
                        }
                    }
                );
            }
        }

        private async void ButtonClick_Back(object sender, EventArgs e)
        {
            await this.Navigation.PopAsync();
        }
    }
}