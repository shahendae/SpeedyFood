using SpeedyFood.Data;
using SpeedyFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Repository
{
    public class ShoppingCartRepository : GenericRepository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public int CountApplicationUser(string id)
        {
            return _context.ShoppingCarts
                .Where(m => m.ApplicationUserId == id)
                .ToList()
                .Count();
        }
    }
}
