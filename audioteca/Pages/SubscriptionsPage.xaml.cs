using audioteca.Models.Api;
using audioteca.Services;
using audioteca.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace audioteca
{
    public partial class SubscriptionsPage : ContentPage
    {
        private readonly SubscriptionsPageViewModel _model;

        private UserSubscriptions _subscriptions;

        public SubscriptionsPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            _model = new SubscriptionsPageViewModel
            {
                Loading = true,
                Items = new ObservableCollection<Subscription>()
            };

            this.BindingContext = _model;

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            if (_model.Loading)
            {
                _subscriptions = await AudioLibrary.Instance.GetUserSubscriptions(true);

                if (_subscriptions == null)
                {
                    return;
                }

                // bind model
                listView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
                listView.BindingContext = _model.Items;
                listView.BeginRefresh();

                _subscriptions.Subscriptions
                    .OrderBy(o => o.Description)
                    .ToList()
                    .ForEach(item => _model.Items.Add(item));

                listView.EndRefresh();

                _model.Loading = false;
            }
        }

        public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // has been set to null, do not 'process' tapped event
            if (e.SelectedItem == null) return;

            // de-select the row
            ((ListView)sender).SelectedItem = null;

            var subscriptionCode = (e.SelectedItem as Subscription).Code;

            if (!string.IsNullOrEmpty(subscriptionCode))
            {
                await Navigation.PushAsync(new SubscriptionTitlesPage((e.SelectedItem as Subscription).Code), true);
            }
        }

        private async void ButtonClick_Back(object sender, EventArgs e)
        {
            await this.Navigation.PopAsync();
        }
    }
}
