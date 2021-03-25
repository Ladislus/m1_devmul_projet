using System.Collections.ObjectModel;
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

        public double Price { get; }

        public CartViewModel()
        {
            var cartService = DependencyService.Get<ICartService>();
            Price = cartService.Price;

            _cart = new ObservableCollection<PizzaItem>();
            foreach (var pair in cartService.Orders)
            {
                foreach (var pizza in pair.Value)
                {
                    _cart.Add(pizza);
                }
            }
        }
    }
}