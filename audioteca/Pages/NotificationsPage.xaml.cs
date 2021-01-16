using Acr.UserDialogs;
using audioteca.Models.Api;
using audioteca.Services;
using audioteca.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace audioteca
{
    public partial class NotificationsPage : ContentPage
    {
        private readonly NotificationsPageViewModel _model;

        private List<NotificationModel> _notifications;

        public NotificationsPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            _model = new NotificationsPageViewModel
            {
                Loading = true,
                Items = new ObservableCollection<NotificationModel>()
            };

            this.BindingContext = _model;

            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            if (_notifications != null) return;

            _model.Loading = true;

            UserDialogs.Instance.ShowLoading("Cargando notificaciones");

            try
            {
                _notifications = await NotificationsStore.Instance.GetNotifications();

                if (_notifications == null)
                {
                    return;
                }

                // bind model
                listView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
                listView.BindingContext = _model.Items;
                listView.BeginRefresh();

                _model.Items.Clear();
                _notifications
                    .ForEach(item => _model.Items.Add(item));

                listView.EndRefresh();
            }
            catch
            {

            }

            UserDialogs.Instance.HideLoading();

            _model.Loading = false;
        }

        public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            _model.Loading = true;

            // has been set to null, do not 'process' tapped event
            if (e.SelectedItem == null) return;

            // de-select the row
            ((ListView)sender).SelectedItem = null;

            await Navigation.PushAsync(
                new NotificationDetailPage(
                    e.SelectedItem as NotificationModel,
                    e.SelectedItemIndex
                ),
                true
            );

            _model.Loading = false;
        }

        private async void ButtonClick_Back(object sender, EventArgs e)
        {
            await this.Navigation.PopAsync();
        }
    }
}
