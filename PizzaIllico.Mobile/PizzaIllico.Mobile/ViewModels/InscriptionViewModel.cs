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
            CommandInscription = new Command(inscription);
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
        public ICommand CommandInscription
        {
            get;
        }

        public void inscription()
        {
            Console.WriteLine(Login);
            Console.WriteLine(Motdepasse);
            Console.WriteLine(Motdepasse2);
            gotoHomeList();
        }
        public async void gotoHomeList()
        {

            await DependencyService.Get<INavigationService>().PushAsync<ShopListPage>();


        }
        public async void gotoConnexionAsync()
        {
            await DependencyService.Get<INavigationService>().PushAsync<ConnexionPage>();


        }
    }
}
