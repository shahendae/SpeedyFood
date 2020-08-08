using Microsoft.EntityFrameworkCore;
using SpeedyFood.Data;
using SpeedyFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Category>> GetAll()
        {
           return await _db.Category.ToListAsync();
        }
        public async Task AddCategory(Category category)
        {
            _db.Category.Add(category);
            await _db.SaveChangesAsync();
        }
        public async Task<Category> GetCategoryById(int id)
        {
            return await _db.Category.FindAsync(id);
        }
        public async Task EditCategory(Category category)
        {
            _db.Category.Update(category);
            await _db.SaveChangesAsync();
        }
        public async Task DeleteCategory(Category category)
        {
            _db.Category.Remove(category);
            await _db.SaveChangesAsync();
        }
    }
}
