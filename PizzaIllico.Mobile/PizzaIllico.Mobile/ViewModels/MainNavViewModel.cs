using System.Threading.Tasks;
using PizzaIllico.Mobile.Pages;
using Storm.Mvvm;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    public class MainNavViewModel : ViewModelBase
    {
        private string _isConnected;
        public TabbedPage _mainVue;
        public ProfileTabbedPage _profileTab;

        public string IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }
        

        public override async Task OnResume()
        {
            await base.OnResume();

            string accessToken = await SecureStorage.GetAsync("access_token");
            string refreshToken = await SecureStorage.GetAsync("refresh_token");
            if (accessToken == null && refreshToken == null)
            {
                //Si le user courant n'est pas connecter : il ne peux pas acceder au profile
                IsConnected = "False";
                _mainVue.Children.Remove(_profileTab);
            }
            else
            {
                IsConnected = "True";
            }
        }
    }
}