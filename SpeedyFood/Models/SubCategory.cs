using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Models
{
    public class SubCategory
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Sub Category")]
        public string Name { get; set; }
        [Required]
        [Display(Name ="Category")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
