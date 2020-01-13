using System;
using System.Collections.Generic;
using System.ComponentModel;
using Acr.UserDialogs;
using audioteca.Helpers;
using audioteca.Models.Api;
using audioteca.Services;
using Xamarin.Forms;

namespace audioteca
{
    public partial class BookDetails : ContentPage, INotifyPropertyChanged
    {
        public new event PropertyChangedEventHandler PropertyChanged;

        public AudioBookDetailResult AudioBook { get; set; }
        private readonly string bookId;
        public bool Loading { get; set; }

        public BookDetails(string id)
        {
            UserDialogs.Instance.ShowLoading("Cargando");

            this.bookId = id;
            this.Loading = true;

            this.BindingContext = this;

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            AudioBook = AsyncHelper.RunSync(() => AudioLibrary.Instance.GetBookDetail(this.bookId));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AudioBook)));
            UserDialogs.Instance.HideLoading();
            this.Loading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Loading)));
        }

        public void ButtonClick_Download(object sender, EventArgs e)
        {
        }
    }
}
