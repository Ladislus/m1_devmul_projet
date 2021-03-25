using System.Collections.Generic;
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
        void RemovePizza(long shopId, PizzaItem pizza);
        void Order();
    }

    public class CartService : ICartService
    {
        private readonly Dictionary<long, List<PizzaItem>> _orders = new();
        private readonly IApiService _apiService;

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

        public CartService()
        {
            _apiService = DependencyService.Get<IApiService>();
        }

        public void AddPizza(long shopId, PizzaItem pizza)
        {
            if (!_orders.ContainsKey(shopId))
            {
                _orders.Add(shopId, new List<PizzaItem>());
            }
            _orders[shopId].Add(pizza);
        }

        public void RemovePizza(long shopId, PizzaItem pizza)
        {
            if (_orders.ContainsKey(shopId))
            {
                _orders[shopId].Remove(pizza);
            }
        }

        public async void Order()
        {
            foreach (var pair in _orders)
            {
                List<long> pizzaids = new();
                foreach (var pizza in pair.Value)
                {
                    pizzaids.Add(pizza.Id);
                }

                Response<OrderItem> response = await _apiService.Post<Response<OrderItem>, CreateOrderRequest>(
                    Urls.DO_ORDER,
                    new CreateOrderRequest
                    {
                        PizzaIds = pizzaids
                    }
                    );
            }
        }
    }
}