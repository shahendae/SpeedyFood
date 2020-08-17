using SpeedyFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Repository
{
    public interface IMenuItemRepository : IGenericRepository<MenuItem>
    {
        Task<IEnumerable<MenuItem>> GetMenuItemsWithCategoryAndSubCategory();
        Task<MenuItem> GetMenuItemWithCategoryAndSubCategory(int id);
    }
}
