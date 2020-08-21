using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public int OrderHeaderId { get; set; }
        public int MenuItemId { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
        public virtual OrderHeader OrderHeader { get; set; }
        public virtual MenuItem MenuItem { get; set; }
    }
}
