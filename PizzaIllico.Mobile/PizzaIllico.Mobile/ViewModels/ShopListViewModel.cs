using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Pizzas;
using PizzaIllico.Mobile.Pages;
using PizzaIllico.Mobile.Services;
using Storm.Mvvm;

using Xamarin.Essentials;

using Storm.Mvvm.Services;

using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    public class ShopListViewModel : ViewModelBase
    {
	    private ObservableCollection<ShopItem> _shops;

        private bool _displayConnection;

        public bool DisplayConnection
        {
            get
            {
                IsConnected();
                return _displayConnection;
            }
            set => SetProperty(ref _displayConnection, value);
        }

        public ObservableCollection<ShopItem> Shops
	    {
            get => _shops;
			set => SetProperty(ref _shops, value);
	    }

		public ICommand SelectedCommand { get; }

		public ICommand GotoConnexion
		{
			get;
		}
	    public ShopListViewModel()
	    { 
		    SelectedCommand = new Command<ShopItem>(SelectedActionAsync);
		    GotoConnexion = new Command(gotoConnexion);
	    }

        private async void IsConnected()
        {
            var token = await SecureStorage.GetAsync("access_token");
            if (!string.IsNullOrEmpty(token) && token != "null")
            {
                DisplayConnection = false;
            }
            else
            {
                DisplayConnection = true;
            }
        }

	    public async void gotoConnexion()
	    {
		    await DependencyService.Get<INavigationService>().PushAsync<ConnexionPage>();

	    }
	    private async void SelectedActionAsync(ShopItem obj)
	    {
		    Console.WriteLine("resto Shops: "+obj.Name);
		    Dictionary<string, Object> data = new Dictionary<string, Object>();
		    data.Add("resto",obj);
		    await DependencyService.Get<INavigationService>().PushAsync<RestoDetailsPage>(data);
	    }

	    public override async Task OnResume()
        {
	        await base.OnResume();

            DisplayConnection = true;

            IPizzaApiService service = DependencyService.Get<IPizzaApiService>();

	        Response<List<ShopItem>> response = await service.ListShops();

			Console.WriteLine($"Appel HTTP : {response.IsSuccess}");
	        if (response.IsSuccess)
	        {
		        Console.WriteLine($"Appel HTTP : {response.Data.Count}");
				Shops = new ObservableCollection<ShopItem>(response.Data);
				Shops = new ObservableCollection<ShopItem>(Shops.OrderBy(a => a.DistanceResto));
	        }
        }
    }
}