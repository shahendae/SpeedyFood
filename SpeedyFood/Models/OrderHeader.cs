using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }
        public DateTime OrderDate { get; set; }
        public double OrderTotalBeforeCoupon { get; set; }

        [Display(Name = "Order Total")]
        public double OrderTotal { get; set; }
        public string CouponCode { get; set; }

        [Display(Name = "Date & Time")]
        public DateTime PickUpDateAndTime { get; set; }
        public string Status { get; set; }
        [Display(Name = "Any Comments:")]
        public string Comments { get; set; }

        [Display(Name = "Name")]
        public string PickUpName { get; set; }

        [Display(Name = "Phone")]
        public string PhoneNumber { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
