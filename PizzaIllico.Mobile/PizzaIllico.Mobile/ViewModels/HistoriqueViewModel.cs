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

        private ObservableCollection<OrderItem> _history;
        public ObservableCollection<OrderItem> History
        {
            get => _history;
            set => SetProperty(ref _history, value);
        }

        public override async Task OnResume()
        {
            Console.WriteLine("OnResume History");

            await base.OnResume();

            var response = await _apiService.Get<Response<ObservableCollection<OrderItem>>>(Urls.LIST_ORDERS, null, true);
            if (response.IsSuccess)
            {
                Console.WriteLine("REQUEST SUCCESSFUL !");
                History = new ObservableCollection<OrderItem>(response.Data.OrderByDescending(order => order.Date));
            }
            else
            {
                Console.WriteLine("ERROR !");
            }
        }
    }
}