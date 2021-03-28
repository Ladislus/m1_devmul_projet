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
            // Essayer de récupérer la dernière géolocation
            try
            {
                // Stockage de la dernier geoloc
                Location location = await Geolocation.GetLastKnownLocationAsync();
                // si trouvé et différent de null
                if (location != null)
                {
#if DEBUG
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
#endif
                    // Retourner la geoloc
                    return location;
                }
            }
            // Gestion des erreurs possibles
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
            // Si loc pas trouvé renvoie d'un position de 0 en lattitude et 0 en longitude
            return new Location(0.0, 0.0);
        }
    }
}