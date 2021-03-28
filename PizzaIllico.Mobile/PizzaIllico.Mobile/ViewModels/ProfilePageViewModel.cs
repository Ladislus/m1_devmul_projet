using System;
using System.Threading.Tasks;
using System.Windows.Input;
using PizzaIllico.Mobile.Pages;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    public class ProfilePageViewModel : ViewModelBase
    {

        private string _errorMsg;

        public ICommand OnDeco
        {
            get;
        }

        public string ErrorMsg
        {
            get => _errorMsg;
            set => SetProperty(ref _errorMsg, value);
        }

        public ProfilePageViewModel()
        {
            OnDeco = new Command(onDeco);
        }

        private void onDeco()
        {
            ErrorMsg = "onDeco";

            SecureStorage.Remove("access_token");
            SecureStorage.Remove("refresh_token");
            SecureStorage.Remove("token_type");
        }
        public override async Task OnResume()
        {
            await base.OnResume();
            string accessToken = await SecureStorage.GetAsync("access_token");
            string refreshToken = await SecureStorage.GetAsync("refresh_token");
            ErrorMsg = accessToken;
            if (accessToken == "" && refreshToken == "")
            {
                await DependencyService.Get<INavigationService>().PushAsync<ConnexionPage>();
            }
        }
    }
}