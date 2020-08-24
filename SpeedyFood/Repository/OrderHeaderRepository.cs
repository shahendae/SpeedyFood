using Microsoft.EntityFrameworkCore;
using SpeedyFood.Data;
using SpeedyFood.Models;
using SpeedyFood.Utility;
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
        public async Task<List<OrderHeader>> GetOrderHeadersWithApplicationUser(string id)
        {
            return await _context.OrderHeaders
                .Include(m => m.ApplicationUser)
                .Where(m => m.ApplicationUserId == id)
                .OrderByDescending(m => m.PickUpDateAndTime)
                .ToListAsync();
        }
        public async Task<List<OrderHeader>> GetOrderHeadersWithReadyStatus()
        {
            return await _context.OrderHeaders
                .Include(m => m.ApplicationUser)
                .Where(m => m.Status == StaticDetails.StatusReady)
                .ToListAsync();
        }

        public async Task<List<OrderHeader>> SearchOrderHeadersByPickupNames(string searchName)
        {
            return await _context.OrderHeaders
                .Include(m => m.ApplicationUser)
                .Where(m => m.PickUpName.ToLower().Contains(searchName.ToLower()))
                .OrderByDescending(m => m.OrderDate)
                .ToListAsync();
        }
        public async Task<List<OrderHeader>> SearchOrderHeadersByPhone(string searchPhone)
        {
            return await _context.OrderHeaders
                .Include(m => m.ApplicationUser)
                .Where(m => m.PhoneNumber.Contains(searchPhone))
                .OrderByDescending(m => m.OrderDate)
                .ToListAsync();
        }
    }
}
