using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PizzaIllico.Mobile.Controls;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Pizzas;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.Services
{
    public interface ICartService
    {
        Dictionary<long, List<PizzaItem>> Orders { get; }
        double Price { get; }
        void AddPizza(long shopId, PizzaItem pizza);
        void RemovePizza(PizzaItem pizza);
        Task Order();
    }

    public class CartService : ICartService
    {
        // Liste des pizzas dans le panier (ID du shop -> Liste des pizzas de ce shop)
        private Dictionary<long, List<PizzaItem>> _orders = new();
        private readonly IApiService _apiService = DependencyService.Get<IApiService>();
        private readonly IToast _toast = DependencyService.Get<IToast>();

        // Property lié à _orders
        public Dictionary<long, List<PizzaItem>> Orders => _orders;

        // Property de calcul de prix
        public double Price
        {
            get
            {
                double price = 0;
                foreach (var pair in _orders)
                {
                    foreach (var pizza in pair.Value)
                    {
                        price += pizza.Price;
                    }
                }

                return price;
            }
        }

        // Fonction d'ajout d'un pizza dans le panier
        public void AddPizza(long shopId, PizzaItem pizza)
        {
            if (!_orders.ContainsKey(shopId))
            {
                _orders.Add(shopId, new List<PizzaItem>());
            }
            _orders[shopId].Add(pizza);
        }

        // Fonction de suppression d'un pizza dans le panier
        public void RemovePizza(PizzaItem pizza)
        {
            foreach (var pizzas in _orders.Values)
            {
                pizzas.Remove(pizza);
            }
        }

        // Fonction de lancement de la request de commande du panier
        public async Task Order()
        {
            // Liste des requêtes qui ont réussi (pour ne pas modifier la liste que l'on parcours)
            List<long> toRemove = new();

            // Récupère les ID des pizzas
            foreach (var pair in _orders)
            {
                List<long> pizzaids = new();
                foreach (var pizza in pair.Value)
                {
                    pizzaids.Add(pizza.Id);
                }

                // Request de commande
                Response<OrderItem> response = await _apiService.Post<Response<OrderItem>, CreateOrderRequest>(
                    Urls.DO_ORDER.Replace("{shopId}", pair.Key.ToString()),
                    new CreateOrderRequest
                    {
                        PizzaIds = pizzaids
                    },
                    true
                    );
                if (response.IsSuccess)
                {
#if DEBUG
                    Console.WriteLine("Request " + pair.Key + " successfull");
#endif
                    // Si la request réussie, ajout dans la liste des pizzas à retirer du panier
                    toRemove.Add(pair.Key);
                }
                else
                {
                    // Pizza plus en stock
                    if (response.ErrorCode == "PIZZA_OUT_OF_STOCK")
                    {
                        _toast.LongAlert(response.ErrorMessage);
                        toRemove.Add(pair.Key);
                    }
                    _toast.LongAlert("Erreur inconnue");
                }
            }

            // Suppression des pizzas commandées
            foreach (var shopId in toRemove)
            {
                _orders.Remove(shopId);
            }
        }
    }
}