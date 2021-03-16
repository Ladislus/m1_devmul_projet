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

using Xamarin.Essentials;

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
		    await DependencyService.Get<INavigationService>().PushAsync<ConnexionPage>();

	    }

        public static async Task<Location> GetLastPosAsync()
		{
			try
			{
				var location = await Geolocation.GetLastKnownLocationAsync();

				if (location != null)
				{
					Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
					return location;
				}
			}
			catch (FeatureNotSupportedException fnsEx)
			{
				// Handle not supported on device exception
			}
			catch (FeatureNotEnabledException fneEx)
			{
				// Handle not enabled on device exception
			}
			catch (PermissionException pEx)
			{
				// Handle permission exception
			}
			catch (Exception ex)
			{
				// Unable to get location
			}
			return new Location(0.0, 0.0);
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