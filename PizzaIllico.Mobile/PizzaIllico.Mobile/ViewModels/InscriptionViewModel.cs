﻿using PizzaIllico.Mobile.Pages;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using System;
using System.Windows.Input;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Authentications;
using PizzaIllico.Mobile.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    class InscriptionViewModel : ViewModelBase
    {
        private String _login, _motdepasse, _motdepasse2, _prenom, _nom, _phoneNum, _errorMsg;

        public InscriptionViewModel()
        {
            CommandInscription = new Command(Inscription);
            GotoConnexion = new Command(gotoConnexionAsync);
        }
        public String Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }         
        public String PhoneNum
        {
            get => _phoneNum;
            set => SetProperty(ref _phoneNum, value);
        }   
        public String ErrorMsg
        {
            get => _errorMsg;
            set => SetProperty(ref _errorMsg, value);
        }  
        public String Prenom
        {
            get => _prenom;
            set => SetProperty(ref _prenom, value);
        }        
        public String Nom
        {
            get => _nom;
            set => SetProperty(ref _nom, value);
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

        public async void Inscription()
        {
            if (Motdepasse == Motdepasse2)
            {
                //Si les deux mdp sont identiques --> On inscrit
                IUserService service = DependencyService.Get<IUserService>();
                Response<LoginResponse> response = await service.Register(Login, Prenom, Nom, PhoneNum, Motdepasse);
                if (response.IsSuccess)
                {
                    await SecureStorage.SetAsync("access_token", response.Data.AccessToken);
                    await SecureStorage.SetAsync("refresh_token", response.Data.RefreshToken);
                    gotoHomeList();
                }
                else
                {
                    ErrorMsg = "Oups ! Nous n'arrivons pas a vous inscrire. Réessayez plus tard.";
                }


            }
            else
            {
#if DEBUG
                Console.WriteLine("Les deux mots de passe ne correspondent pas.");
#endif
                ErrorMsg = "Les deux mots de passe ne correspondent pas.";
            }

        }
        public async void gotoHomeList()
        {

            await DependencyService.Get<INavigationService>().PushAsync<MainNavPage>();


        }
        public async void gotoConnexionAsync()
        {
            await DependencyService.Get<INavigationService>().PushAsync<ConnexionPage>();


        }
    }
}
