using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Pizzas;
using PizzaIllico.Mobile.Pages;
using PizzaIllico.Mobile.Services;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using Storm.Mvvm.Services;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    public class RestoDetailsViewModel : ViewModelBase
    {

        private string _nomResto, _adresse;
        private long _id;
        private ObservableCollection<PizzaItem> _pizzas;

        public ObservableCollection<PizzaItem> Pizzas
        {
            get => _pizzas;
            set => SetProperty(ref _pizzas, value);
        }
        public string NomResto
        {
            get => _nomResto;
            set => SetProperty(ref _nomResto, value);
        }        
        public string AdressResto
        {
            get => _adresse;
            set => SetProperty(ref _adresse, value);
        }

        public override void Initialize(Dictionary<string, object> navigationParameters)
        {
            base.Initialize(navigationParameters);
            ShopItem resto = (ShopItem) GetNavigationParameter<Object>("resto");
            NomResto = resto.Name;
            _id = resto.Id;
            AdressResto = resto.Address;
        }
        
        public override async Task OnResume()
        {
            await base.OnResume();

            IPizzaApiService service = DependencyService.Get<IPizzaApiService>();

            Response<List<PizzaItem>> response = await service.ListPizzas((int)this._id);

            Console.WriteLine($"Appel HTTP : {response.IsSuccess}");
            if (response.IsSuccess)
            {
                Pizzas = new ObservableCollection<PizzaItem>(response.Data);
                Console.WriteLine($"Appel HTTP : {response.Data.Count}");
            }
        }
    }
}