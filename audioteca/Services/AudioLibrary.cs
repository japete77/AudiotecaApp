using audioteca.Models.Api;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        TitleResult _titles;
        DateTime _expireTitles;

        AuthorsResult _authors;
        DateTime _expireAuthors;

        private const int ExpirySecs = Int32.MaxValue;

        public AudioLibrary()
        {

        }

        public async Task WarmUp()
        {
            await GetBooksByTitle(1, Int32.MaxValue);
            await GetAuthors(1, Int32.MaxValue);
        }

        public async Task RefreshBooks()
        {
            _titles = await Get<TitleResult>("titles", null, 0, Int32.MaxValue, null);
            _expireTitles = DateTime.Now.AddSeconds(ExpirySecs);
        }

        public async Task<TitleResult> GetBooksByTitle(int index, int count)
        {
            if (_titles == null || _expireTitles < DateTime.Now)
            {
                _titles = await Get<TitleResult>("titles", null, index, count, null);
                _expireTitles = DateTime.Now.AddSeconds(ExpirySecs);
            }

            return _titles;
        }

        public async Task<TitleResult> GetBooksByAuthor(string author, int index, int count)
        {
            var titles = await GetBooksByTitle(index, count);

            var titlesByAuthor = titles.Titles.Where(w => w.AuthorId == author).ToList();

            return new TitleResult
            {
                Titles = titlesByAuthor,
                Total = titlesByAuthor.Count
            };
        }

        public async Task<AuthorsResult> GetAuthors(int index, int count)
        {
            if (_authors == null || _expireAuthors < DateTime.Now)
            {
                _authors = await this.Get<AuthorsResult>("authors", null, index, count, null);
                _expireAuthors = DateTime.Now.AddSeconds(ExpirySecs);
            }

            return _authors;
        }

        public async Task<string> GetAudioBookLink(string id)
        {
            if (Int32.TryParse(id, out int res))
            {
                var request = new RestRequest($"title/{id}/link", DataFormat.Json)
                {
                    Method = Method.GET
                };
                request.AddParameter("session", Session.Instance.GetSession());

                var result = await Call<AudioBookLinkResult>(request);

                return result.AudioBookLink;
            }
            else
            {
                var request = new RestRequest($"subscription/title/{id}/link", DataFormat.Json)
                {
                    Method = Method.GET
                };
                request.AddParameter("session", Session.Instance.GetSession());

                var result = await Call<SubscriptionTitleLinkResult>(request);

                return result.SubscriptionTitleLink;
            }
        }

        public async Task<AudioBookDetailResult> GetBookDetail(string id)
        {
            var request = new RestRequest($"title/{id}", DataFormat.Json)
            {
                Method = Method.GET
            };
            request.AddParameter("session", Session.Instance.GetSession());

            return await Call<AudioBookDetailResult>(request);
        }

        public async Task<UserSubscriptions> GetUserSubscriptions()
        {
            var request = new RestRequest($"subscriptions", DataFormat.Json)
            {
                Method = Method.GET
            };
            request.AddParameter("session", Session.Instance.GetSession());

            return await Call<UserSubscriptions>(request);
        }

        public async Task<List<SubscriptionTitle>> GetSubscriptionTitles(string code)
        {
            var request = new RestRequest($"subscriptions/titles", DataFormat.Json)
            {
                Method = Method.GET
            };
            request.AddParameter("session", Session.Instance.GetSession());
            request.AddParameter("code", code);

            var result = await Call<SubscriptionTitleResult>(request);

            return result.Titles;
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
            return await Task.Run<T>(async () =>
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

                    if (!await Session.Instance.IsAuthenticated())
                    {
                        Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await Application.Current.MainPage.Navigation.PushAsync(new LoginPage());
                        });
                    }

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
