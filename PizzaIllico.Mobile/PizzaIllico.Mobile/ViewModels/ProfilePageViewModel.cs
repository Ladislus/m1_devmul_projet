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
        private ICommand _onDeco;

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

        public void onDeco()
        {
            ErrorMsg = "onDeco";
            try
            {
                SecureStorage.Remove("access_token");
                SecureStorage.Remove("refresh_token");
                SecureStorage.Remove("token_type");

            }
            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
            }
        }
        public async override Task OnResume()
        {
            await base.OnResume();
            try
            {
                string access_token = await SecureStorage.GetAsync("access_token");
                string refresh_token = await SecureStorage.GetAsync("refresh_token");
                ErrorMsg = access_token;
                if (access_token == "" && refresh_token == "")
                {
                    await DependencyService.Get<INavigationService>().PushAsync<ConnexionPage>();
                }
            }
            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
            }
        }
    }
}