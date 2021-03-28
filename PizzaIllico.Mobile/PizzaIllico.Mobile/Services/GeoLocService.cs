using System;
using System.Threading.Tasks;
using PizzaIllico.Mobile.Controls;
using Xamarin.Essentials;
using Xamarin.Forms;

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
#if DEBUG
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
#endif
                    return location;
                }
            }
            catch (FeatureNotSupportedException)
            {
                DependencyService.Get<IToast>().LongAlert("La feature n'est pas supportée");
            }
            catch (FeatureNotEnabledException)
            {
                DependencyService.Get<IToast>().LongAlert("La feature n'est pas activée");
            }
            catch (PermissionException)
            {
                DependencyService.Get<IToast>().LongAlert("Permissions insuffisantes");
            }
            catch (Exception)
            {
                DependencyService.Get<IToast>().LongAlert("Erreur inconnue");
            }
            return new Location(0.0, 0.0);
        }
    }
}