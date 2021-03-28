using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PizzaIllico.Mobile.Dtos.Pizzas;
using PizzaIllico.Mobile.Services;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    public class CartViewModel : ViewModelBase
    {
        private readonly ICartService _cartService = DependencyService.Get<ICartService>();
        private readonly IDialogService _dialogService = DependencyService.Get<IDialogService>();

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

        private bool _shouldBeVisible;
        public bool ShouldBeVisible
        {
            get => _shouldBeVisible;
            set => SetProperty(ref _shouldBeVisible, value);
        }

        public ICommand OrderCommand { get; }
        public ICommand DeleteCommand { get; }

        public CartViewModel()
        {
            OrderCommand = new Command(OrderCart);
            DeleteCommand = new Command<PizzaItem>(DeletePizza);
        }

        public override async Task OnResume()
        {
            await base.OnResume();
            SetCart();
        }

        public async void DeletePizza(PizzaItem pizza)
        {
#if DEBUG
            Console.WriteLine("DeletePizza " + pizza.Id);
#endif
            var response = await _dialogService.DisplayAlertAsync(
                    "Supprimer",
                    "Êtes vous sûr de vouloir supprimer " + pizza.Name + " de la liste ?",
                    "Oui",
                    "Non"
                );
            if (response)
            {
                _cartService.RemovePizza(pizza);
                SetCart();
            }
        }

        public async void OrderCart()
        {
            var response = await _dialogService.DisplayAlertAsync(
                    "Commander",
                    "Êtes vous sûr de vouloir commander pour un total de " + Price + "€ ?",
                    "Oui",
                    "Non"
                );
            if (response)
            {
                await _cartService.Order();
                SetCart();
            }
        }

        private void SetCart()
        {
            ObservableCollection<PizzaItem> items = new();
            foreach (var pizza in _cartService.Orders.Values.SelectMany(pizzas => pizzas))
            {
                items.Add(pizza);
            }

            Cart = items;
            Price = _cartService.Price;
            ShouldBeVisible = Cart.Count > 0;
        }
    }
}