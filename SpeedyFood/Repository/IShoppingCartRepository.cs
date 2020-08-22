﻿using SpeedyFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Repository
{
    public interface IShoppingCartRepository : IGenericRepository<ShoppingCart>
    {
        int CountApplicationUser(string id);
        void RemoveList(List<ShoppingCart> shoppingCarts);
    }
}
