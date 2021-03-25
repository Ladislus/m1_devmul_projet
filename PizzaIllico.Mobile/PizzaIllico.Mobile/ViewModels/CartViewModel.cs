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

            Console.WriteLine("DeletePizza " + pizza.Id);

            var response = await DependencyService.Get<IDialogService>()
                .DisplayAlertAsync(
                    "Supprimer",
                    "Êtes vous sûr de vouloir supprimer " + pizza.Name + " de la liste ?",
                    "Oui",
                    "Non"
                );
            if (response)
            {
                DependencyService.Get<ICartService>().RemovePizza(pizza);
                SetCart();
            }
        }

        public async void OrderCart()
        {
            var response = await DependencyService.Get<IDialogService>()
                .DisplayAlertAsync(
                    "Commander",
                    "Êtes vous sûr de vouloir commander pour un total de " + Price + "€ ?",
                    "Oui",
                    "Non"
                );
            if (response)
            {
                DependencyService.Get<ICartService>().Order();
                SetCart();
            }
        }

        private void SetCart()
        {
            ICartService cartService = DependencyService.Get<ICartService>();
            ObservableCollection<PizzaItem> items = new();
            foreach (var pizza in cartService.Orders.Values.SelectMany(pizzas => pizzas))
            {
                items.Add(pizza);
            }

            Cart = items;
            Price = cartService.Price;
            ShouldBeVisible = Cart.Count > 0;

#if DEBUG
            Console.WriteLine("Called SetCart");
            Console.WriteLine("Cart size : " + Cart.Count);
#endif
        }
    }
}