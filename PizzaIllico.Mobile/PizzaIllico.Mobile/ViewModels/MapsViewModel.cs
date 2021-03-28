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
using Xamarin.Essentials;
using MapDrive =  Xamarin.Essentials.Map;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Map = Xamarin.Forms.Maps.Map;


namespace PizzaIllico.Mobile.ViewModels
{
    public class MapsViewModel : ViewModelBase
    {
        private Map _mapPos;

        public Map MapPos
        {
            get => _mapPos;
            set => SetProperty(ref _mapPos, value);
        }
        
        public ICommand SelectedCommand { get; set; }

        public override async Task OnResume()
        {
            await base.OnResume();

            IGeoLocService geoloc = new GeoLocService();
            Location lastPos = geoloc.GetLastPosAsync().Result;
            Position position = new Position(lastPos.Latitude, lastPos.Longitude);
            MapSpan mapSpan = new MapSpan(position, 0.01, 0.01);
            MapPos = new Map(mapSpan)
            {
                IsShowingUser = true,
                MapType = MapType.Hybrid
            };

            IPizzaApiService service = DependencyService.Get<IPizzaApiService>();

            Response<List<ShopItem>> response = await service.ListShops();
            ObservableCollection<ShopItem> Shops;

            Console.WriteLine($"Appel HTTP tom : {response.IsSuccess}");
            if (response.IsSuccess)
            {
                Console.WriteLine($"Appel HTTP tom: {response.Data.Count}");
                Shops = new ObservableCollection<ShopItem>(response.Data);
                for (int i = 0; i < Shops.Count; i++)
                {
                    ShopItem shopItem = Shops[i];
                    Console.WriteLine($"addresse Maps " + shopItem.Address);
                    Pin pin = new Pin
                    {
                        Label = shopItem.Name,
                        Address = shopItem.Address,
                        Type = PinType.Place,
                        Position = new Position(shopItem.Latitude, shopItem.Longitude)
                    };
                    pin.Clicked += async (object sender, EventArgs e) =>
                    {
                        Dictionary<string, Object> data = new Dictionary<string, Object>();
                        data.Add("resto",shopItem);
                        await DependencyService.Get<INavigationService>().PushAsync<RestoDetailsPage>(data);
                    };
                    MapPos.Pins.Add(pin);
                }
            }
        }
    }
}