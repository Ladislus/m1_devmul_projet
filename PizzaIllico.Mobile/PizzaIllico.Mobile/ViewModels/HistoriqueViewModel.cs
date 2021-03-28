using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Pizzas;
using PizzaIllico.Mobile.Services;
using Storm.Mvvm;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    public class HistoriqueViewModel : ViewModelBase
    {
        private readonly IApiService _apiService = DependencyService.Get<IApiService>();

        // Liste des commandes passées
        private ObservableCollection<OrderItem> _history;
        public ObservableCollection<OrderItem> History
        {
            get => _history;
            set => SetProperty(ref _history, value);
        }

        public override async Task OnResume()
        {
#if DEBUG
            Console.WriteLine("OnResume History");
#endif
            await base.OnResume();

            // Request à l'API
            var response = await _apiService.Get<Response<ObservableCollection<OrderItem>>>(Urls.LIST_ORDERS, null, true);
            if (response.IsSuccess)
            {
#if DEBUG
                Console.WriteLine("[DEBUG] HISTORY FETCH SUCCESS !");
#endif
                // Mise à jour de la vue avec les commandes passées
                History = new ObservableCollection<OrderItem>(response.Data.OrderByDescending(order => order.Date));
            }
#if DEBUG
            else
            {
                Console.WriteLine("[DEBUG] HISTORY FETCH FAILED !");
            }
#endif
        }
    }
}