using Storm.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    class ConnexionViewModel : ViewModelBase
    {
        private String _login;
        private String _motdepasse;

        public ConnexionViewModel()
        {
            CommandeConnexion = new Command(connexion);
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
        public void connexion()
        {
            Console.WriteLine(Login);
            Console.WriteLine(Motdepasse);
        }




    }
}
