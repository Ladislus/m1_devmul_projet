using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PizzaIllico.Mobile.Services
{
    public interface IApiService
    {
        Task<TResponse> Get<TResponse>(string url, string shopid = null);
        Task<TResponse> Post<TResponse, TData>(string url, TData data);

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

        public async Task<TResponse> Post<TResponse, TData>(string url, TData data)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(HOST + url),
                Content = content
            };

            HttpResponseMessage response = await _client.SendAsync(request);

            string responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResponse>(responseContent);
        }
    }
}