using System;
using System.Collections.Generic;
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
            try
            {
                string access_token = await SecureStorage.GetAsync("access_token");
                string refresh_token = await SecureStorage.GetAsync("refresh_token");
                if (access_token == null && refresh_token == null)
                {
                    IsConnected = "False";
                    _mainVue.Children.Remove(_profileTab);
                }
                else
                {
                    IsConnected = "True";
                }
            }
            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
            }
        }
    }
}