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
        private String _motdepasse;


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
            Console.WriteLine(Login);
            Console.WriteLine(Motdepasse);

            IUserService service = DependencyService.Get<IUserService>();
            Response<LoginResponse> response = await service.Connect(Login, Motdepasse);

            Console.WriteLine($"Appel HTTP : {response.IsSuccess}");
            if (response.IsSuccess)
            {
                Console.WriteLine($"Appel HTTP : {response.Data}");
                try
                {
                    await SecureStorage.SetAsync("access_token", response.Data.AccessToken);
                    await SecureStorage.SetAsync("refresh_token", response.Data.RefreshToken);
                }
                catch (Exception ex)
                {
                    // Possible that device doesn't support secure storage on device.
                }
            }
            gotoHomeList();
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
