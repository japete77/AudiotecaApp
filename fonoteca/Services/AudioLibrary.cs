using fonoteca.Models.Api;
using fonoteca.Pages;
using RestSharp;
using System.Runtime.CompilerServices;

namespace fonoteca.Services
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

        TitleResult _titlesLatest;
        DateTime _expireTitlesLatest;

        AuthorsResult _authors;
        DateTime _expireAuthors;

        private const int ExpirySecs = int.MaxValue;

        public AudioLibrary()
        {

        }

        public async Task WarmUp()
        {
            await GetBooksByTitle(1, int.MaxValue);
            await GetAuthors(1, int.MaxValue);
        }

        public async Task RefreshBooks()
        {
            _titles = await Get<TitleResult>("titles", null, 0, int.MaxValue, null);
            _expireTitles = DateTime.Now.AddSeconds(ExpirySecs);
        }

        public async Task<TitleResult> GetBooksByTitle(int index, int count, bool orderByLatest = false)
        {
            if (orderByLatest)
            {
                if (_titlesLatest == null || _expireTitlesLatest < DateTime.Now)
                {
                    _titlesLatest = await Get<TitleResult>("titles/latest", null, index, count, null);
                    _expireTitlesLatest = DateTime.Now.AddSeconds(ExpirySecs);
                }
                return _titlesLatest;
            }
            else
            {
                if (_titles == null || _expireTitles < DateTime.Now)
                {
                    _titles = await Get<TitleResult>("titles", null, index, count, null);
                    _expireTitles = DateTime.Now.AddSeconds(ExpirySecs);
                }
                return _titles;
            }            
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

        public async Task IncreaseTitleDownloadCounter(string id)
        {
            if (int.TryParse(id, out int res))
            {
                var request = new RestRequest($"title/{id}/link/count?session={Session.Instance.GetSession()}")
                {
                    Method = Method.Post
                };

                await CallPost(request);
            }
            else
            {
                var request = new RestRequest($"subscription/title/{id}/link/count?session={Session.Instance.GetSession()}")
                {
                    Method = Method.Post,                    
                };

                await CallPost(request);                
            }
        }


        public async Task<string> GetAudioBookLink(string id)
        {
            if (int.TryParse(id, out int res))
            {
                var request = new RestRequest($"title/{id}/link")
                {
                    Method = Method.Get,
                    RequestFormat = DataFormat.Json
                };
                request.AddParameter("session", Session.Instance.GetSession());

                var result = await Call<AudioBookLinkResult>(request);

                return result.AudioBookLink;
            }
            else
            {
                var request = new RestRequest($"subscription/title/{id}/link")
                {
                    Method = Method.Get,
                    RequestFormat = DataFormat.Json
                };
                request.AddParameter("session", Session.Instance.GetSession());

                var result = await Call<SubscriptionTitleLinkResult>(request);

                return result.SubscriptionTitleLink;
            }
        }

        public async Task<AudioBookDetailResult> GetBookDetail(string id)
        {
            var request = new RestRequest($"title/{id}")
            {
                Method = Method.Get,
                RequestFormat = DataFormat.Json
            };
            request.AddParameter("session", Session.Instance.GetSession());

            return await Call<AudioBookDetailResult>(request);
        }

        public async Task<UserSubscriptions> GetUserSubscriptions(bool onlyAppSubscription = true)
        {
            var request = new RestRequest($"subscriptions")
            {
                Method = Method.Get,
                RequestFormat = DataFormat.Json
            };
            request.AddParameter("session", Session.Instance.GetSession());
            request.AddParameter("onlyAppSubscriptions", onlyAppSubscription);

            return await Call<UserSubscriptions>(request);
        }

        public async Task<List<SubscriptionTitle>> GetSubscriptionTitles(string code)
        {
            var request = new RestRequest($"subscriptions/titles")
            {
                Method = Method.Get,
                RequestFormat = DataFormat.Json
            };
            request.AddParameter("session", Session.Instance.GetSession());
            request.AddParameter("code", code);

            var result = await Call<SubscriptionTitleResult>(request);

            return result.Titles;
        }

        public async Task<List<NotificationModel>> GetNotifications()
        {
            var request = new RestRequest($"notifications")
            {
                Method = Method.Get,
                RequestFormat = DataFormat.Json
            };
            request.AddParameter("session", Session.Instance.GetSession());

            var result = await Call<NotificationsResult>(request);

            return result.Notifications;
        }

        public async Task<List<int>> GetNotificationsIds()
        {
            var request = new RestRequest($"notifications/ids")
            {
                Method = Method.Get,
                RequestFormat = DataFormat.Json
            };
            request.AddParameter("session", Session.Instance.GetSession());

            var result = await Call<List<int>>(request);

            return result;
        }

        private async Task<T> Get<T>(string method, string text, int index, int count, string filter) where T : new()
        {
            var request = new RestRequest(method)
            {
                Method = Method.Get,
                RequestFormat = DataFormat.Json
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
                var result = ApiClient.Instance.Client.ExecuteGet<T>(request);
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
                        await Shell.Current.GoToAsync(nameof(LoginPage));
                    }
                    else
                    {
                        await Shell.Current.GoToAsync(nameof(MainPage));
                    }
                }
                else
                {
                    await Shell.Current.GoToAsync(nameof(LoginPage));
                }

                return default;
            });
        }

        private async Task CallPost(RestRequest request)
        {
            await Task.Run(async () =>
            {
                var result = ApiClient.Instance.Client.ExecutePost(request);
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Session.Instance.SetSession(null);
                    Session.Instance.SaveSession();

                    if (!await Session.Instance.IsAuthenticated())
                    {
                        await Shell.Current.GoToAsync(nameof(LoginPage));
                    }
                    else
                    {
                        await Shell.Current.GoToAsync(nameof(MainPage));
                    }
                }
                else
                {
                    await Shell.Current.GoToAsync(nameof(LoginPage));
                }
            });
        }
    }
}
