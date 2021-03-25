using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace PizzaIllico.Mobile.Services
{
    public interface IApiService
    {
        Task<TResponse> Get<TResponse>(string url, string shopid = null);
        Task<TResponse> Post<TResponse, TData>(string url, TData data, bool isLogged = false);
    }

    public class ApiService : IApiService
    {
        private const string HOST = "https://pizza.julienmialon.ovh/";
        private readonly HttpClient _client = new HttpClient();

        public async Task<TResponse> Get<TResponse>(string url, string param = null)
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

            HttpResponseMessage response = await _client.SendAsync(request);

            string content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResponse>(content);
        }

        public async Task<TResponse> Post<TResponse, TData>(string url, TData data, bool isLogged = false)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(HOST + url),
                Content = content
            };

            if (isLogged)
            {
                String token = await SecureStorage.GetAsync("access_token");
                String tokenType = await SecureStorage.GetAsync("token_type");
                if (token != null && tokenType != null)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue(tokenType, token);
                }
                else
                {
                    Console.WriteLine("NOTIMPLEMENTED !");
                    // TODO try refresh, if unssuccessful, logout
                }
            }

            HttpResponseMessage response = await _client.SendAsync(request);

            string responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResponse>(responseContent);
        }
    }
}