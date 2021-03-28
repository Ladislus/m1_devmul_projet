using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PizzaIllico.Mobile.Controls;
using PizzaIllico.Mobile.Dtos.Pizzas;
using PizzaIllico.Mobile.Services;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    public class CartViewModel : ViewModelBase
    {
        private readonly ICartService _cartService = DependencyService.Get<ICartService>();
        private readonly IDialogService _dialogService = DependencyService.Get<IDialogService>();

        // Contenu du panier
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

        // Boolean permettant de savoir si le bouton de commande doit être afficher (panier vide ou non)
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

        // Fonction de suppression d'une pizza dans le panier
        public async void DeletePizza(PizzaItem pizza)
        {
#if DEBUG
            Console.WriteLine("DeletePizza " + pizza.Id);
#endif
            // Demande de confirmation
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

        // Fonction pour commander le contenu du panier
        public async void OrderCart()
        {
            // Si l'utilisateur est connecté
            if (!string.IsNullOrEmpty(await SecureStorage.GetAsync("access_token")))
            {
                // Demande de confirmation
                var response = await _dialogService.DisplayAlertAsync(
                    "Commander",
                    "Êtes vous sûr de vouloir commander pour un total de " + Price + "€ ?",
                    "Oui",
                    "Non"
                );
                if (response)
                {
                    // Lancement de la commande
                    await _cartService.Order();
                    // Mise à jour de la vue
                    SetCart();
                }
            }
            else
            {
                DependencyService.Get<IToast>().LongAlert("Veuillez vous connecter pour commander");
            }

        }

        // Fonction de mise à jour de la vue
        private void SetCart()
        {
            // Fetch de toutes les pizzas dans le panier (CartService)
            ObservableCollection<PizzaItem> items = new();
            foreach (var pizza in _cartService.Orders.Values.SelectMany(pizzas => pizzas))
            {
                items.Add(pizza);
            }

            // Mise à jour des property (passage par les SetProperty())
            Cart = items;
            Price = _cartService.Price;
            ShouldBeVisible = Cart.Count > 0;
        }
    }
}