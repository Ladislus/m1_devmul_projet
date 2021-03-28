using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PizzaIllico.Mobile.Controls;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Authentications;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.Services
{
    public interface IApiService
    {
        Task<TResponse> Get<TResponse>(string url, string shopid = null, bool requireHeader = false);
        Task<TResponse> Post<TResponse, TData>(string url, TData data, bool requireHeader = false);
        Task<TResponse> Patch<TResponse, TData>(string url, TData data, bool requireHeader = false);
    }

    public class ApiService : IApiService
    {
        private const string HOST = "https://pizza.julienmialon.ovh/";
        private readonly HttpClient _client = new();

        // Fonction pour vérifier qu'un utilisateur est connecté
        private async Task<bool> IsConnected()
        {
#if DEBUG
            Console.WriteLine("[DEBUG] CALL -> IsConnected ?");
#endif
            // Vérifie la présence de "access_token" dans le safestorage
            return !string.IsNullOrEmpty(await SecureStorage.GetAsync("access_token"));
        }

        // Fonction pour déconnecter l'utilisateur
        private void Disconnect()
        {
#if DEBUG
            Console.WriteLine("[DEBUG] CALL ->  Disconnect !");
#endif
            // Clear le safestorage
            SecureStorage.Remove("access_token");
            SecureStorage.Remove("refresh_token");
            SecureStorage.Remove("token_type");
            // Toast d'alerte
            DependencyService.Get<IToast>().LongAlert("Un erreur de connexion est survenue, vous avez été déconnecté, veuillez vous reconnecter");
            // Redirection vers la page d'accueil
            DependencyService.Get<ITabbedService>().get().CurrentPage = DependencyService.Get<ITabbedService>().get().Children[0];
            DependencyService.Get<ITabbedService>().get().Children.RemoveAt(DependencyService.Get<ITabbedService>().get().Children.Count-1);
#if DEBUG
            Console.WriteLine("[DEBUG] Disconnected !");
#endif
        }

        // Fonction pour vérifier que le token est toujours valide
        private async Task<bool> NeedRefresh()
        {
#if DEBUG
            Console.WriteLine("[DEBUG] CALL -> NeedRefresh ?");
#endif
            // Récupération de la date d'expiration, et comparaison avec la date actuelle(+5 secondes pour anticiper le temps d'envoie de la request)
            var ticks = long.Parse(await SecureStorage.GetAsync("expire_in"));
            var dateTime = new DateTime(ticks);
            return DateTime.Now.AddSeconds(5).CompareTo(dateTime) > 0;
        }

        // Fonction pour tester si un utilisateurs est connecté, et que le token est valide
        private async Task TestConnexion()
        {
#if DEBUG
            Console.WriteLine("[DEBUG] CALL ->  TestConnexion");
#endif
            // Si un utilisateur est connecté ...
            if (await IsConnected())
            {
#if DEBUG
                Console.WriteLine("\t[DEBUG] connected !");
#endif
                // Test la validité du token
                if (await NeedRefresh())
                {
#if DEBUG
                    Console.WriteLine("[DEBUG] Need refresh !");
#endif
                    // Si le token n'est plus valide, tente de la refresh
                    await Refresh();
                }
            }
            else
            {
#if DEBUG
                Console.WriteLine("\t[DEBUG] Not connected !");
#endif
                // Si personne n'est connecté, renvoi à la page d'accueil
                Disconnect();
            }
        }

        // Fonction pour tenter de refresh le mot de passe
        private async Task Refresh()
        {
#if DEBUG
            Console.WriteLine("[DEBUG] CALL -> Refresh !");
#endif
            // Request à l'API
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
            // Si la requête à réussie, mets à jours les informations
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
            // Si la request échoue, déconnexion/retour à l'accueil
            else
            {
#if DEBUG
                Console.WriteLine("[DEBUG] FAILED !");

#endif
                Disconnect();
            }
        }

        // Fonction de request à l'API pour refresh le token
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

        // Fonction générique GET
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

        // Fonction générique POST
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

        // Fonction générique PATCH
        public async Task<TResponse> Patch<TResponse, TData>(string url, TData data, bool requireHeader = false)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = new HttpMethod("PATCH"),
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