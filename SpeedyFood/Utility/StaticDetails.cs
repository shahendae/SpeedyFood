using SpeedyFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Utility
{
    public static class StaticDetails
    {
        public const string DefaultFoodImage = "default_food.png";

        public const string KitchenUser = "Kitchen";
        public const string ManagerUser = "Manager";
        public const string CustomerEndUser = "Customer";
        public const string FrontDeskUser = "FrontDesk";

        public const string ssCouponCode = "ssCouponCode";
        public const string ssCartCount = "ssCartCount";

        public const string StatusSubmitted = "Submitted";
        public const string StatusInProgress = "Being Prepared";
        public const string StatusReady = "Ready for Pickup";
        public const string StatusCompleted = "Completed";
        public const string StatusCancelled = "Cancelled";


        public static double CountDiscount(Coupon coupon, double OriginalTotal)
        {
            if(coupon == null)
            {
                return OriginalTotal;
            }
            else
            {
                if(coupon.MinAmount > OriginalTotal)
                {
                    return OriginalTotal;
                }
                else
                {
                    if(Convert.ToInt32(coupon.CouponType) == (int)Coupon.ECouponType.Money)
                    {
                        return Math.Round(OriginalTotal - coupon.Discount, 2);
                    }
                    else
                    {
                        if(Convert.ToInt32(coupon.CouponType) == (int)Coupon.ECouponType.Percent)
                        {
                            return Math.Round(OriginalTotal - (OriginalTotal * coupon.Discount / 100), 2);
                        }
                    }
                }
            }

            return OriginalTotal;
        }
    }
}
