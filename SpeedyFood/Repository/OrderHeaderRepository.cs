using Microsoft.EntityFrameworkCore;
using SpeedyFood.Data;
using SpeedyFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Repository
{
    public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderHeaderRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public async Task<OrderHeader> GetOrderHeaderWithApplicationUser(int HeaderId, string UserId)
        {
            return await _context.OrderHeaders
                .Include(m => m.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == HeaderId && m.ApplicationUserId == UserId);
        }
    }
}
