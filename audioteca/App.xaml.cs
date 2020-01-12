using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using audioteca.Helpers;
using audioteca.Models.Api;
using audioteca.Services;
using Xamarin.Forms;

namespace audioteca
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // var res = AsyncHelper.RunSync<bool>(() => Session.Instance.Login(1, "0001"));
            if (!string.IsNullOrEmpty(Session.Instance.GetSession()))
            {
                try
                {
                    var books = AsyncHelper.RunSync<TitleResult>(() => AudioLibrary.Instance.GetBooksByTitle(1, 10));
                    //var authors = AsyncHelper.RunSync<AuthorsResult>(() => AudioLibrary.Instance.GetAuthors(1, 10));
                    //var authorBooks = AsyncHelper.RunSync<TitleResult>(() => AudioLibrary.Instance.GetBooksByAuthor(authors.Authors[0].Id, 1, 10));
                    //var bookDetails = AsyncHelper.RunSync<AudioBookDetailResult>(() => AudioLibrary.Instance.GetBookDetail(books.Titles[0].Id));
                    //var searchBook = AsyncHelper.RunSync<TitleResult>(() => AudioLibrary.Instance.SearchBooksByTitle("dios", 1, 10));
                    //var searchAuthor = AsyncHelper.RunSync<AuthorsResult>(() => AudioLibrary.Instance.SearchAuthors("piper", 1, 10));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.Message}");
                }
            }
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
