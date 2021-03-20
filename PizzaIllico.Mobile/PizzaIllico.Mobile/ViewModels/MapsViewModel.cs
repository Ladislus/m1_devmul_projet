using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Pizzas;
using PizzaIllico.Mobile.Services;
using Storm.Mvvm;
using Xamarin.Essentials;
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
            set => SetProperty(ref _mapPos,value);
        }

        public override async Task OnResume()
        {
            await base.OnResume();

            GeoLocService geoloc = new GeoLocService();
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
                    Console.WriteLine($"addresse "+Shops[i].Address);
                    MapPos.Pins.Add(new Pin
                    {
                        Label = Shops[i].Name,
                        Address = Shops[i].Address,
                        Type = PinType.Place,
                        Position = new Position(Shops[i].Latitude,Shops[i].Longitude)
                    });
                }
            }
        }
    }
}