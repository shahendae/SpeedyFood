using SpeedyFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Repository
{
    public interface ISubCategoryRepository : IGenericRepository<SubCategory>
    {
        Task<IEnumerable<SubCategory>> GetSubCategoriesWithCategory();
        Task<List<string>> GetSubCategoriesName();
        SubCategory GetSubCategoryWithCategory(int id);
    }
}
