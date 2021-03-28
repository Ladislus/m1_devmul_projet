using System;
using System.Threading.Tasks;
using System.Windows.Input;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Authentications;
using PizzaIllico.Mobile.Dtos.Authentications.Credentials;
using PizzaIllico.Mobile.Pages;
using PizzaIllico.Mobile.Services;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    public class ProfilePageViewModel : ViewModelBase
    {

        private string _errorMsg;
        private ICommand _onDeco, _onChangeMdp;
        private string _oldMdp, _newMdp, _newMdp2;

        public ICommand OnDeco
        {
            get;
        }

        public ICommand OnChangeMdp
        {
            get;
        }
        public string OldMdp
        {
            get => _oldMdp;
            set => SetProperty(ref _oldMdp, value);
        }        
        public string NewMdp
        {
            get => _newMdp;
            set => SetProperty(ref _newMdp, value);
        }
        public string NewMdp2
        {
            get => _newMdp2;
            set => SetProperty(ref _newMdp2, value);
        }
        public string ErrorMsg
        {
            get => _errorMsg;
            set => SetProperty(ref _errorMsg, value);
        }

        public ProfilePageViewModel()
        {
            OnDeco = new Command(onDeco);
            OnChangeMdp = new Command(onChangeMdp);
        }

        public async void onChangeMdp()
        {
            ErrorMsg = "je change !";
            IUserService service = DependencyService.Get<IUserService>();
            SetPasswordRequest obj = new SetPasswordRequest();
            obj.OldPassword = OldMdp;
            obj.NewPassword = NewMdp2;
            Response response = await service.ChangePassword(obj);
            if (response.IsSuccess)
            {
                ErrorMsg = "Mot de passe changé.";
                
            }
            else
            {
                ErrorMsg = response.ErrorMessage;
            }
            
        }

        private void onDeco()
        {
            ErrorMsg = "onDeco";


            SecureStorage.Remove("access_token");
            SecureStorage.Remove("refresh_token");
            SecureStorage.Remove("token_type");
            TabbedPage main = DependencyService.Get<ITabbedService>().get(); 
            main.CurrentPage = main.Children[0];
            main.Children.RemoveAt(main.Children.Count-1);
        


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