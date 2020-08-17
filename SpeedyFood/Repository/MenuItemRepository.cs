using Microsoft.EntityFrameworkCore;
using SpeedyFood.Data;
using SpeedyFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Repository
{
    public class MenuItemRepository : GenericRepository<MenuItem>, IMenuItemRepository
    {
        private readonly ApplicationDbContext _context;

        public MenuItemRepository(ApplicationDbContext context): base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<MenuItem>> GetMenuItemsWithCategoryAndSubCategory()
        {
            return await _context.MenuItems
                .Include(m => m.Category)
                .Include(m => m.SubCategory)
                .ToListAsync();
        }
        public async Task<MenuItem> GetMenuItemWithCategoryAndSubCategory(int id)
        {
            return await _context.MenuItems
                .Include(m => m.Category)
                .Include(m => m.SubCategory)
                .SingleOrDefaultAsync(m => m.Id == id);
        }
    }
}
