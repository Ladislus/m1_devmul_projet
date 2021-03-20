using System;
using System.Threading.Tasks;
using PizzaIllico.Mobile.Dtos;
using Xamarin.Essentials;

namespace PizzaIllico.Mobile.Services
{
    public interface IGeoLocService
    {
        Task<Location> GetLastPosAsync();
    }

    public class GeoLocService : IGeoLocService
    {
        public async Task<Location> GetLastPosAsync()
        {
            try
            {
                Location location = await Geolocation.GetLastKnownLocationAsync();

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
    }
}