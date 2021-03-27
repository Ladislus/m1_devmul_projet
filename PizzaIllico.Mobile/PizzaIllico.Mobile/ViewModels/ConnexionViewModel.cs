using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Authentications;
using PizzaIllico.Mobile.Pages;
using PizzaIllico.Mobile.Services;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Essentials;
namespace PizzaIllico.Mobile.ViewModels
{
    class ConnexionViewModel : ViewModelBase
    {
        private String _login;
        private String _motdepasse, _errorMsg;


        public ConnexionViewModel()
        {
            CommandeConnexion = new Command(connexion);
            GotoInscription = new Command(gotoInscriptionAsync);
        }

        public String Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }        
        public String ErrorMsg
        {
            get => _errorMsg;
            set => SetProperty(ref _errorMsg, value);
        }
        public String Motdepasse
        {
            get => _motdepasse;
            set => SetProperty(ref _motdepasse, value);
        }

        public ICommand CommandeConnexion
        {
            get;
        }
        public ICommand GotoInscription
        {
            get;
        }
        
        public async void connexion()
        {

            IUserService service = DependencyService.Get<IUserService>();
            Response<LoginResponse> response = await service.Connect(Login, Motdepasse);

            if (response.IsSuccess)
            {
                try
                {
                    await SecureStorage.SetAsync("token_type", response.Data.TokenType);
                    await SecureStorage.SetAsync("access_token", response.Data.AccessToken);
                    await SecureStorage.SetAsync("refresh_token", response.Data.RefreshToken);
                    gotoHomeList();

                }
                catch (Exception ex)
                {
                    // TODO
                    // Possible that device doesn't support secure storage on device.
                }
            }
            else
            {
                ErrorMsg = "Impossible de se connecter, réessayez plus tard.";
            }
        }
        public async void gotoInscriptionAsync()
        {
            await DependencyService.Get<INavigationService>().PushAsync<InscriptionPage>();

        }
        public async void gotoHomeList()
        {
            //await ReplaceAsync(new ShopListPage());
            await DependencyService.Get<INavigationService>().PushAsync<MainNavPage>();

        }



    }
}
