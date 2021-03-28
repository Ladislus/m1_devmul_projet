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
        // Attribut pour ma map
        private Map _mapPos;

        // Propriété pour la gestion de ma map
        public Map MapPos
        {
            get => _mapPos;
            set => SetProperty(ref _mapPos, value);
        }
        
        public ICommand SelectedCommand { get; set; }

        public override async Task OnResume()
        {
            await base.OnResume();
            // Récupération de la dernière geoloc
            IGeoLocService geoloc = new GeoLocService();
            Location lastPos = geoloc.GetLastPosAsync().Result;
            // Récupération de la pos du resto 
            Position position = new Position(lastPos.Latitude, lastPos.Longitude);
            MapSpan mapSpan = new MapSpan(position, 0.01, 0.01);
            // Mise en place de la map avec l'affichage de la position de l'user
            // et en mode hybride (satellite et plan) 
            MapPos = new Map(mapSpan)
            {
                IsShowingUser = true,
                MapType = MapType.Hybrid
            };
            // Récupération de la liste des shops
            IPizzaApiService service = DependencyService.Get<IPizzaApiService>();
            Response<List<ShopItem>> response = await service.ListShops();
            
            ObservableCollection<ShopItem> Shops;
#if DEBUG
            Console.WriteLine($"Appel HTTP tom : {response.IsSuccess}");
#endif
            // si la requete pour avoir les shops c'est bien passé 
            if (response.IsSuccess)
            {
#if DEBUG
                Console.WriteLine($"Appel HTTP tom: {response.Data.Count}");
#endif
                // Mise des données récupérées dans une observable list de shopItem
                Shops = new ObservableCollection<ShopItem>(response.Data);
                // Pour tous les restos
                for (int i = 0; i < Shops.Count; i++)
                {
                    ShopItem shopItem = Shops[i];
#if DEBUG
                    Console.WriteLine($"addresse Maps " + shopItem.Address);
#endif
                    // Créer un Pin qui représente le resto
                    Pin pin = new Pin
                    {
                        Label = shopItem.Name,
                        Address = shopItem.Address,
                        Type = PinType.Place,
                        Position = new Position(shopItem.Latitude, shopItem.Longitude)
                    };
                    // Ajout d'un évenement sur le pin quand il est cliqué
                    pin.Clicked += async (object sender, EventArgs e) =>
                    {
                        // Affichage de la page de détail du resto
                        Dictionary<string, Object> data = new Dictionary<string, Object>();
                        data.Add("resto",shopItem);
                        await DependencyService.Get<INavigationService>().PushAsync<RestoDetailsPage>(data);
                    };
                    // Ajout du pin sur la map
                    MapPos.Pins.Add(pin);
                }
            }
        }
    }
}