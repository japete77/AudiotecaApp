using System;
using System.Collections.Generic;
using System.ComponentModel;
using Acr.UserDialogs;
using audioteca.Helpers;
using audioteca.Models.Api;
using audioteca.Services;
using audioteca.ViewModels;
using Xamarin.Forms;

namespace audioteca
{
    public partial class BookDetails : ContentPage, INotifyPropertyChanged
    {
        private readonly string bookId;
        
        private BookDetailsViewModel _model;

        public BookDetails(string id)
        {
            UserDialogs.Instance.ShowLoading("Cargando");

            this.bookId = id;

            _model = new BookDetailsViewModel();
            _model.Loading = true;

            this.BindingContext = _model;

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            _model.AudioBook = AsyncHelper.RunSync(() => AudioLibrary.Instance.GetBookDetail(this.bookId));

            UserDialogs.Instance.HideLoading();

            _model.Loading = false;
        }

        public void ButtonClick_Download(object sender, EventArgs e)
        {
        }
    }
}
