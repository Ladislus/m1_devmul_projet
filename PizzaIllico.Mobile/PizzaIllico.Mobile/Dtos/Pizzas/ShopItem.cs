using System;
using Newtonsoft.Json;
using PizzaIllico.Mobile.Services;
using PizzaIllico.Mobile.ViewModels;
using Storm.Mvvm;
using Xamarin.Essentials;
using Xamarin.Forms.Internals;

namespace PizzaIllico.Mobile.Dtos.Pizzas
{
    public class ShopItem : ViewModelBase
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("address")]
        public string Address { get; set; }
        
        [JsonProperty("latitude")]
        public double Latitude { get; set; }
        
        [JsonProperty("longitude")]
        public double Longitude { get; set; }
        
        [JsonProperty("minutes_per_kilometer")]
        public double MinutesPerKilometer { get; set; }

        // Mise en place d'un attribut pour stocké la distance calculé entre le telephone et le resto
        private double _distance; 
        // Propriété pour la distance
        public double DistanceResto
        {
            get
            {
                GeoLocService geoloc = new GeoLocService();  
                Location pos = geoloc.GetLastPosAsync().Result; // Récupération de la dernière position du téléphone
                Location resto = new Location(Latitude, Longitude); // Récupération de position du resto
                _distance = pos.CalculateDistance(resto, DistanceUnits.Kilometers); // Calcul de la distance entre le resto et le telephone 
                return _distance;
            }
            set
            {
                SetProperty(ref _distance, value);
            } 
            
        }
        

	}
}