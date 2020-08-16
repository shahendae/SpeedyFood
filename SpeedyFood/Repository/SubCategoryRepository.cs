using Microsoft.EntityFrameworkCore;
using SpeedyFood.Data;
using SpeedyFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Repository
{
    public class SubCategoryRepository : GenericRepository<SubCategory>, ISubCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public SubCategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SubCategory>> GetSubCategoriesWithCategory()
        {
            return await _context.SubCategories.Include(c => c.Category).ToListAsync();
        }
        public async Task<List<string>> GetSubCategoriesName()
        {
            return await _context.SubCategories
                .OrderBy(m => m.Name)
                .Select(m => m.Name)
                .Distinct()
                .ToListAsync();
        }
        public SubCategory GetSubCategoryWithCategory(int id)
        {
            return _context.SubCategories
                .Include(m => m.Category)
                .Where(m => m.Id == id)
                .FirstOrDefault();

        }
    }
}
