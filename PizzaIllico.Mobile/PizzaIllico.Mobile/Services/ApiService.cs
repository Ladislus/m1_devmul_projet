using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Authentications;
using Xamarin.Essentials;

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
#if DEBUG
            Console.WriteLine("[DEBUG] CALL -> IsConnected ?");
#endif
            return !string.IsNullOrEmpty(await SecureStorage.GetAsync("access_token"));
        }

        private void Disconnect()
        {
#if DEBUG
            Console.WriteLine("[DEBUG] CALL ->  Disconnect !");
#endif
            SecureStorage.Remove("access_token");
            SecureStorage.Remove("refresh_token");
            SecureStorage.Remove("token_type");
            //TODO GoToHome page
#if DEBUG
            Console.WriteLine("[DEBUG] Disconnected !");
#endif
        }

        private async Task<bool> NeedRefresh()
        {
#if DEBUG
            Console.WriteLine("[DEBUG] CALL -> NeedRefresh ?");
#endif
            var ticks = long.Parse(await SecureStorage.GetAsync("expire_in"));
            var dateTime = new DateTime(ticks);
            return DateTime.Now.CompareTo(dateTime) > 0;
        }

        private async Task TestConnexion()
        {
#if DEBUG
            Console.WriteLine("[DEBUG] CALL ->  TestConnexion");
#endif
            if (await IsConnected())
            {
#if DEBUG
                Console.WriteLine("\t[DEBUG] connected !");
#endif
                if (await NeedRefresh())
                {
#if DEBUG
                    Console.WriteLine("[DEBUG] Need refresh !");
#endif
                    await Refresh();
                }
            }
            else
            {
#if DEBUG
                Console.WriteLine("\t[DEBUG] Not connected !");
#endif
                Disconnect();
            }
        }

        private async Task Refresh()
        {
#if DEBUG
            Console.WriteLine("[DEBUG] CALL -> Refresh !");
#endif
            String refreshToken = await SecureStorage.GetAsync("refresh_token");
            Response<LoginResponse> loginResponse = await RefreshRequestCall(
                new RefreshRequest
                {
                    ClientId = "MOBILE",
                    ClientSecret = "UNIV",
                    RefreshToken = refreshToken
                });
#if DEBUG
            Console.WriteLine("[DEBUG] R: " + loginResponse);
            Console.WriteLine("[DEBUG] Success: " + loginResponse.IsSuccess);
            Console.WriteLine("[DEBUG] ErrorCode: " + loginResponse.ErrorCode);
            Console.WriteLine("[DEBUG] ErrorMessage: " + loginResponse.ErrorMessage);
#endif
            if (loginResponse.IsSuccess)
            {
                await SecureStorage.SetAsync("token_type", loginResponse.Data.TokenType);
                await SecureStorage.SetAsync("access_token", loginResponse.Data.AccessToken);
                await SecureStorage.SetAsync("refresh_token", loginResponse.Data.RefreshToken);
                await SecureStorage.SetAsync("expire_in", DateTime.Now.AddSeconds(loginResponse.Data.ExpiresIn).Ticks.ToString());
#if DEBUG
                Console.WriteLine("[DEBUG] SUCCESS !");
                Console.WriteLine("[DEBUG] AccessToken: " + loginResponse.Data.AccessToken);
                Console.WriteLine("[DEBUG] RefreshToken: " + loginResponse.Data.RefreshToken);
                Console.WriteLine("[DEBUG] TokenType: " + loginResponse.Data.TokenType);
                Console.WriteLine("[DEBUG] Expire: " + loginResponse.Data.ExpiresIn);
#endif
            }
            else
            {
#if DEBUG
                Console.WriteLine("[DEBUG] FAILED !");

#endif
                Disconnect();
            }
        }

        private async Task<Response<LoginResponse>> RefreshRequestCall(RefreshRequest request)
        {
#if DEBUG
            Console.WriteLine("[DEBUG] CALL -> RefreshRequestCall !");
#endif
            StringContent content = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");

            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(HOST + Urls.REFRESH_TOKEN),
                Content = content
            };

            String token = await SecureStorage.GetAsync("access_token");
            String tokenType = await SecureStorage.GetAsync("token_type");
            if (token != null && tokenType != null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(tokenType, token);
            }
            else
            {
                Disconnect();
            }

            HttpResponseMessage response = await _client.SendAsync(requestMessage);
            string responseContent = await response.Content.ReadAsStringAsync();
#if DEBUG
            Console.WriteLine("[DEBUG] Result: " + responseContent);
#endif
            return JsonConvert.DeserializeObject<Response<LoginResponse>>(responseContent);
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
                await TestConnexion();

                String token = await SecureStorage.GetAsync("access_token");
                String tokenType = await SecureStorage.GetAsync("token_type");
                if (token != null && tokenType != null)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue(tokenType, token);
#if DEBUG
                    Console.WriteLine("[DEBUG] Get header: " + request.Headers.Authorization);
#endif
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
                await TestConnexion();

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