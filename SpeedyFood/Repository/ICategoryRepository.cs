using SpeedyFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Repository
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAll();
        Task AddCategory(Category category);
        Task<Category> GetCategoryById(int id);
        Task EditCategory(Category category);
        Task DeleteCategory(Category category);
    }
}
