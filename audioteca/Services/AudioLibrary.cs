
using System;
using System.Threading.Tasks;
using audioteca.Exceptions;
using audioteca.Models.Api;
using RestSharp;
using Xamarin.Forms;

namespace audioteca.Services
{
    public class AudioLibrary
    {
        private static AudioLibrary _instance;
        public static AudioLibrary Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AudioLibrary();
                }

                return _instance;
            }
        }

        public AudioLibrary()
        {

        }

        public async Task<TitleResult> GetBooksByTitle(int index, int count)
        {
            return await Get<TitleResult>("gettitles", null, index, count, null);
        }

        public async Task<TitleResult> GetBooksByAuthor(string author, int index, int count)
        {
            return await this.GetTitlesByAuthor(author, index, count);
        }

        public async Task<TitleResult> SearchBooksByTitle(string text, int index, int count)
        {
            return await this.Get<TitleResult>("searchtitles", text, index, count, null);
        }

        public async Task<AuthorsResult> GetAuthors(int index, int count)
        {
            return await this.Get<AuthorsResult>("getauthors", null, index, count, null);
        }

        public async Task<AuthorsResult> SearchAuthors(string text, int index, int count)
        {
            return await this.Get<AuthorsResult>("searchauthors", text, index, count, null);
        }

        public async Task<AuthorsResult> GetAuthorsFiltered(int index, int count, string filter)
        {
            return await this.Get<AuthorsResult>("GetAuthorsFiltered", null, index, count, filter);
        }

        public async Task<TitleResult> GetTitlesFiltered(int index, int count, string filter)
        {
            return await this.Get<TitleResult>("GetTitlesFiltered", null, index, count, filter);
        }

        public async Task<AudioBookLinkResult> GetAudioBookLink(string id)
        {
            var request = new RestRequest("getaudiobooklink", DataFormat.Json)
            {
                Method = Method.GET
            };
            request.AddParameter("session", Session.Instance.GetSession());
            request.AddParameter("id", id);

            return await Call<AudioBookLinkResult>(request);
        }

        public async Task<AudioBookDetailResult> GetBookDetail(string id)
        {
            var request = new RestRequest("getaudiobookdetail", DataFormat.Json)
            {
                Method = Method.GET
            };
            request.AddParameter("session", Session.Instance.GetSession());
            request.AddParameter("id", id);

            return await Call<AudioBookDetailResult>(request);
        }

        private async Task<TitleResult> GetTitlesByAuthor(string id, int index, int count)
        {
            var request = new RestRequest("gettitlesbyauthor", DataFormat.Json)
            {
                Method = Method.GET
            };
            request.AddParameter("session", Session.Instance.GetSession());
            request.AddParameter("id", id);
            request.AddParameter("index", index);
            request.AddParameter("count", count);

            return await Call<TitleResult>(request);
        }

        private async Task<T> Get<T>(string method, string text, int index, int count, string filter) where T : new()
        {
            var request = new RestRequest(method, DataFormat.Json)
            {
                Method = Method.GET
            };
            request.AddParameter("session", Session.Instance.GetSession());
            request.AddParameter("index", index);
            request.AddParameter("count", count);

            if (!string.IsNullOrEmpty(text))
            {
                request.AddParameter("text", text);
            }

            if (!string.IsNullOrEmpty(filter))
            {
                request.AddParameter("filter", filter);
            }

            return await Call<T>(request);
        }

        private async Task<T> Call<T>(RestRequest request) where T : new()
        {
            return await Task.Run<T>(() =>
            {
                var result = ApiClient.Instance.Client.Get<T>(request);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return result.Data;
                }
                else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Session.Instance.SetSession(null);
                    Session.Instance.SaveSession();
                    Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Application.Current.MainPage.Navigation.PushAsync(new AudioLibraryPage());
                    });
                }
                else
                {
                    Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Application.Current.MainPage.Navigation.PopToRootAsync();
                    });
                }

                return default;
            });
        }
    }
}
