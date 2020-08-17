using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string CouponType { get; set; }
        public enum ECouponType { Percent=0, Money=1 }
        public double Discount { get; set; }
        [Display(Name = "Minimum Amount")]
        public double MinAmount { get; set; }
        // to save image on database
        public byte[] Image { get; set; }
        public bool IsActive { get; set; }
    }
}
