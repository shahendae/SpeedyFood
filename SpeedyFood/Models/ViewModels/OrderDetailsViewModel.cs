﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Models.ViewModels
{
    public class OrderDetailsViewModel
    {
        public List<ShoppingCart> ShoppingCarts { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
