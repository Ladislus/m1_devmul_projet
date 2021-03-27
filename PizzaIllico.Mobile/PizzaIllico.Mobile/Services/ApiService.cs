using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Authentications;
using PizzaIllico.Mobile.Pages;
using Storm.Mvvm.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.Services
{
    public interface IApiService
    {
        Task<TResponse> Get<TResponse>(string url, string shopid = null, bool requireHeader = false);
        Task<TResponse> Post<TResponse, TData>(string url, TData data, bool requireHeader = false);
    }

    public class ApiService : IApiService
    {
        private const string HOST = "https://pizza.julienmialon.ovh/";
        private readonly HttpClient _client = new();

        private async Task<bool> IsConnected()
        {
            return !string.IsNullOrEmpty(await SecureStorage.GetAsync("access_token"));
        }

        private async void Disconnect()
        {
            SecureStorage.Remove("access_token");
            SecureStorage.Remove("refresh_token");
            SecureStorage.Remove("token_type");
            SecureStorage.Remove("login");
            SecureStorage.Remove("password");
        }

        private async Task<bool> NeedRefresh()
        {
            var ticks = long.Parse(await SecureStorage.GetAsync("expire_in"));
            var dateTime = new DateTime(ticks);
            return DateTime.Now.CompareTo(dateTime) > 0;
        }

        private async void TestConnexion()
        {
            if (await IsConnected())
            {
                if (await NeedRefresh())
                {
                    Refresh();
                }
            }
            else
            {
                Disconnect();
            }
        }

        private async void Refresh()
        {
            String refresh_token = await SecureStorage.GetAsync("refresh_token");
            String login = await SecureStorage.GetAsync("password");
            String password = await SecureStorage.GetAsync("password");
            StringContent content = new StringContent(JsonConvert.SerializeObject(new RefreshRequest
            {
                ClientId = login,
                ClientSecret = password,
                RefreshToken = refresh_token
            }), System.Text.Encoding.UTF8, "application/json");
            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                Content = content,
                Method = HttpMethod.Post,
                RequestUri = new Uri(HOST + Urls.REFRESH_TOKEN)
            };

            string responseContent = await (await _client.SendAsync(requestMessage)).Content.ReadAsStringAsync();
            Response<LoginResponse> loginResponse = JsonConvert.DeserializeObject<Response<LoginResponse>>(responseContent);
#if DEBUG
            Console.WriteLine(loginResponse.IsSuccess);
            Console.WriteLine(loginResponse.ErrorCode);
            Console.WriteLine(loginResponse.ErrorMessage);
            Console.WriteLine(loginResponse.Data.AccessToken);
            Console.WriteLine(loginResponse.Data.RefreshToken);
            Console.WriteLine(loginResponse.Data.RefreshToken);
#endif
            if (loginResponse.IsSuccess)
            {
                await SecureStorage.SetAsync("token_type", loginResponse.Data.TokenType);
                await SecureStorage.SetAsync("access_token", loginResponse.Data.AccessToken);
                await SecureStorage.SetAsync("refresh_token", loginResponse.Data.RefreshToken);
                await SecureStorage.SetAsync("expire_in", DateTime.Now.AddSeconds(loginResponse.Data.ExpiresIn).Ticks.ToString());
            }
            else
            {
                Disconnect();
            }
        }

        public async Task<TResponse> Get<TResponse>(string url, string param = null, bool requireHeader = false)
        {

            HttpRequestMessage request;
            if (param == null)
            {
                request = new HttpRequestMessage(HttpMethod.Get, HOST + url);
            }
            else
            {
                url = url.Replace("{shopId}",param);
                request = new HttpRequestMessage(HttpMethod.Get, HOST + url);
            }

            if (requireHeader)
            {
                TestConnexion();

                String token = await SecureStorage.GetAsync("access_token");
                String tokenType = await SecureStorage.GetAsync("token_type");
                if (token != null && tokenType != null)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue(tokenType, token);
                }
                else
                {
                    Disconnect();
                }
            }

            HttpResponseMessage response = await _client.SendAsync(request);

            string content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResponse>(content);
        }

        public async Task<TResponse> Post<TResponse, TData>(string url, TData data, bool requireHeader = false)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(HOST + url),
                Content = content
            };

            if (requireHeader)
            {
                TestConnexion();

                String token = await SecureStorage.GetAsync("access_token");
                String tokenType = await SecureStorage.GetAsync("token_type");
                if (token != null && tokenType != null)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue(tokenType, token);
                }
                else
                {
                    Disconnect();
                }
            }

            HttpResponseMessage response = await _client.SendAsync(request);

            string responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResponse>(responseContent);
        }
    }
}