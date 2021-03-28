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
        private Dictionary<long, List<PizzaItem>> _orders = new();
        private readonly IApiService _apiService = DependencyService.Get<IApiService>();
        private readonly IToast _toast = DependencyService.Get<IToast>();

        public Dictionary<long, List<PizzaItem>> Orders => _orders;

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

        public void AddPizza(long shopId, PizzaItem pizza)
        {
            if (!_orders.ContainsKey(shopId))
            {
                _orders.Add(shopId, new List<PizzaItem>());
            }
            _orders[shopId].Add(pizza);
        }

        public void RemovePizza(PizzaItem pizza)
        {
            foreach (var pizzas in _orders.Values)
            {
                pizzas.Remove(pizza);
            }
        }

        public async Task Order()
        {

            List<long> toRemove = new();

            foreach (var pair in _orders)
            {
                List<long> pizzaids = new();
                foreach (var pizza in pair.Value)
                {
                    pizzaids.Add(pizza.Id);
                }

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
                    toRemove.Add(pair.Key);
                }
                else
                {
                    if (response.ErrorCode == "PIZZA_OUT_OF_STOCK")
                    {
                        _toast.LongAlert(response.ErrorMessage);
                        toRemove.Add(pair.Key);
                    }
                    _toast.LongAlert("Erreur inconnue");
                }
            }

            foreach (var shopId in toRemove)
            {
                _orders.Remove(shopId);
            }
        }
    }
}