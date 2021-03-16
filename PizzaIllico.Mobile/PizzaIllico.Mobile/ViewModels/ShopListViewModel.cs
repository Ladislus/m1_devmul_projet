using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Pizzas;
using PizzaIllico.Mobile.Pages;
using PizzaIllico.Mobile.Services;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    public class ShopListViewModel : ViewModelBase
    {
	    private ObservableCollection<ShopItem> _shops;

	    public ObservableCollection<ShopItem> Shops
	    {
		    get => _shops;
		    set => SetProperty(ref _shops, value);
	    }

		public ICommand SelectedCommand { get; }

	    public ShopListViewModel()
	    {
		    SelectedCommand = new Command<ShopItem>(SelectedActionAsync);
	    }

	    private async void SelectedActionAsync(ShopItem obj)
	    {
		    Dictionary<string, Object> data = new Dictionary<string, Object>();
		    data.Add("resto",obj);
		    await DependencyService.Get<INavigationService>().PushAsync<RestoDetailsPage>(data);
	    }

		public override async Task OnResume()
        {
	        await base.OnResume();

	        IPizzaApiService service = DependencyService.Get<IPizzaApiService>();

	        Response<List<ShopItem>> response = await service.ListShops();

			Console.WriteLine($"Appel HTTP : {response.IsSuccess}");
	        if (response.IsSuccess)
	        {
		        Console.WriteLine($"Appel HTTP : {response.Data.Count}");
				Shops = new ObservableCollection<ShopItem>(response.Data);
	        }
        }
    }
}