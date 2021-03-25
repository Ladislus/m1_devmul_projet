using System;
using System.Collections.Generic;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Pizzas;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.Services
{
    public interface ICartService
    {
        Dictionary<ShopItem, List<PizzaItem>> Orders { get; }
        double Price { get; }
        void AddPizza(ShopItem shop, PizzaItem pizza);
        void RemovePizza(ShopItem shop, PizzaItem pizza);
        void Order();
    }

    public class CartService : ICartService
    {
        private readonly Dictionary<ShopItem, List<PizzaItem>> _orders = new();
        private readonly IApiService _apiService;

        public Dictionary<ShopItem, List<PizzaItem>> Orders => _orders;

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

        public void AddPizza(ShopItem shop, PizzaItem pizza)
        {
            if (!_orders.ContainsKey(shop))
            {
                _orders.Add(shop, new List<PizzaItem>());
            }
            _orders[shop].Add(pizza);
        }

        public void RemovePizza(ShopItem shop, PizzaItem pizza)
        {
            if (_orders.ContainsKey(shop))
            {
                _orders[shop].Remove(pizza);
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
                if (response.IsSuccess)
                {
                    Console.WriteLine("\n\n" + response.Data.Shop.Id + " is Ok\n\n");
                }
                else
                {
                    Console.WriteLine("\n\n" + response.Data.Shop.Id + " is false\n\n");
                }
            }
        }
    }
}