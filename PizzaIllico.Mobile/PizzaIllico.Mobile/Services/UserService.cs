using System;
using System.Collections.Generic;
using PizzaIllico.Mobile.Dtos;
using System.Threading.Tasks;
using PizzaIllico.Mobile.Dtos.Authentications;
using Xamarin.Forms;
using PizzaIllico.Mobile.Dtos.Authentications.Credentials;

namespace PizzaIllico.Mobile.Services
{

    public interface IUserService
    {
        public Task<Response<LoginResponse>> Connect(string login, string motdepasse);


    }

    class UserService : IUserService
    {
        private readonly IApiService _apiService;

        public UserService()
        {
            
            _apiService = DependencyService.Get<IApiService>();
            
        }
        public async Task<Response<LoginResponse>> Connect(string login, string motdepasse)
        {
            LoginWithCredentialsRequest data = new LoginWithCredentialsRequest
            {
                Login = login,
                Password = motdepasse,
                ClientId = "MOBILE",
                ClientSecret = "UNIV"

            };
            return await _apiService.Post<Response<LoginResponse>, LoginWithCredentialsRequest>(Urls.LOGIN_WITH_CREDENTIALS, data);
        }
    }
}
