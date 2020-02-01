using Acr.UserDialogs;
using audioteca.Models.Daisy;
using audioteca.Services;
using audioteca.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace audioteca
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AudioBookIndexPage : ContentPage
    {
        private readonly AudioBookIndexViewModel _model;

        private DaisyBook _book;

        public AudioBookIndexPage()
        {
            UserDialogs.Instance.ShowLoading("Cargando");

            _model = new AudioBookIndexViewModel();
            this.BindingContext = _model;

            _model.Loading = true;

            Title = "Índice";
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            _book = DaisyPlayer.Instance.GetDaisyBook();

            if (_book != null)
            {
                int selectedLevel = DaisyPlayer.Instance.GetPlayerInfo().Position.NavigationLevel;
                _model.Items = _book.Body.Where(w => w.Level <= selectedLevel).ToList();
                listView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
                listView.BindingContext = _model.Items;
            }

            _model.Loading = false;

            UserDialogs.Instance.HideLoading();
        }


        public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // has been set to null, do not 'process' tapped event
            if (e.SelectedItem == null) return;

            // de-select the row
            ((ListView)sender).SelectedItem = null;

            var selectedItem = (e.SelectedItem as SmilInfo);

            var selectedSequence =_book.Sequence.Where(w => w.Id == selectedItem.Id).First();

            await DaisyPlayer.Instance.Seek(_book.Sequence.IndexOf(selectedSequence));

            await Navigation.PopAsync(true);
        }
    }
}