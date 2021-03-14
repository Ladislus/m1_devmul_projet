using PizzaIllico.Mobile.Pages;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    class ConnexionViewModel : ViewModelBase
    {
        private String _login;
        private String _motdepasse;

        //public async Task ReplaceAsync(Page page)
        //{
        //    var current = DependencyService.Get<ICurrentPageService>().CurrentPage;
        //    Page lastPage = current.Navigation.NavigationStack.Last();

        //    await DependencyService.Get<INavigationService>().PushAsync(page);

        //    current.Navigation.RemovePage(lastPage);
        //}

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
        
        public void connexion()
        {
            Console.WriteLine(Login);
            Console.WriteLine(Motdepasse);
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
