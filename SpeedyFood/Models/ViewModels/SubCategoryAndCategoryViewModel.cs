using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Models.ViewModels
{
    public class SubCategoryAndCategoryViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public SubCategory SubCategory { get; set; }
        public List<string> SubCategoriesList { get; set; }
        public string StatusMessg { get; set; }
    }
}
