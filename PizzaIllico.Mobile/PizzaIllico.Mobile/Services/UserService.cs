using PizzaIllico.Mobile.Dtos;
using System.Threading.Tasks;
using PizzaIllico.Mobile.Dtos.Accounts;
using PizzaIllico.Mobile.Dtos.Authentications;
using Xamarin.Forms;
using PizzaIllico.Mobile.Dtos.Authentications.Credentials;

namespace PizzaIllico.Mobile.Services
{

    public interface IUserService
    {
        public Task<Response<LoginResponse>> Connect(string login, string motdepasse);
        public Task<Response<LoginResponse>> Register(string login, string prenom, string nom, string phoneNum, string mdp);
        public Task<Response> ChangePassword(SetPasswordRequest obj);


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

        public async Task<Response<LoginResponse>> Register(string login, string prenom, string nom, string phoneNum, string mdp)
        {
            CreateUserRequest data = new CreateUserRequest
            {
                Email = login,
                Password = mdp,
                ClientId = "MOBILE",
                ClientSecret = "UNIV",
                PhoneNumber = phoneNum,
                FirstName = prenom,
                LastName = nom
            };
            return await _apiService.Post<Response<LoginResponse>, CreateUserRequest>(Urls.CREATE_USER, data);
        }

        public async Task<Response> ChangePassword(SetPasswordRequest obj)
        {
            return await _apiService.Patch<Response, SetPasswordRequest>(Urls.SET_PASSWORD, obj, true);
        }
    }
}
