using PizzaIllico.Mobile.Pages;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    class InscriptionViewModel : ViewModelBase
    {
        private String _login, _motdepasse, _motdepasse2;

        public InscriptionViewModel()
        {
            GotoConnexion = new Command(gotoConnexionAsync);
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
        public String Motdepasse2
        {
            get => _motdepasse2;
            set => SetProperty(ref _motdepasse2, value);
        }
        public ICommand GotoConnexion
        {
            get;
        }
        public async void gotoConnexionAsync()
        {
            await DependencyService.Get<INavigationService>().PopAsync();
        }
    }
}
