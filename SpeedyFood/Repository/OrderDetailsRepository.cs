using Microsoft.EntityFrameworkCore;
using SpeedyFood.Data;
using SpeedyFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Repository
{
    public class OrderDetailsRepository: GenericRepository<OrderDetails>, IOrderDetailsRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailsRepository(ApplicationDbContext context): base(context)
        {
            _context = context;
        }
        public List<OrderDetails> GetOrderDetailsWithMenuItems(int orderHeaderId)
        {
            return _context.OrderDetails
                .Include(m => m.MenuItem)
                .Where(m => m.OrderHeaderId == orderHeaderId)
                .ToList();
        }
    }
}
