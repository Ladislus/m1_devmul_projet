using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PizzaIllico.Mobile.Dtos.Pizzas;
using PizzaIllico.Mobile.Services;
using Storm.Mvvm;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    public class CartViewModel : ViewModelBase
    {
        private ObservableCollection<PizzaItem> _cart;
        public ObservableCollection<PizzaItem> Cart
        {
            get => _cart;
            set => SetProperty(ref _cart, value);
        }

        private double _price;
        public double Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public override async Task OnResume()
        {
            await base.OnResume();
            ICartService cartService = DependencyService.Get<ICartService>();
            ObservableCollection<PizzaItem> items = new();
            foreach (var pizza in cartService.Orders.Values.SelectMany(pizzas => pizzas))
            {
                items.Add(pizza);
            }

            Cart = items;
            Price = cartService.Price;
        }
    }
}