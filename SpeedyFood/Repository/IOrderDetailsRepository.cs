﻿using SpeedyFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Repository
{
    public interface IOrderDetailsRepository : IGenericRepository<OrderDetails>
    {
        List<OrderDetails> GetOrderDetailsWithMenuItems(int orderHeaderId);
    }
}
