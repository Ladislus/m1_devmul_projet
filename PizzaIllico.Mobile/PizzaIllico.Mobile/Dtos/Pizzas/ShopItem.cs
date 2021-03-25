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

        private double _distance;
        public double DistanceResto
        {
            get
            {
                GeoLocService geoloc = new GeoLocService();
                Location pos = geoloc.GetLastPosAsync().Result;
                Location resto = new Location(this.Latitude, this.Longitude);
                _distance = pos.CalculateDistance(resto, DistanceUnits.Kilometers);
                return _distance;
            }
            set
            {
                SetProperty(ref _distance, value);
            } 
            
        }
        

	}
}