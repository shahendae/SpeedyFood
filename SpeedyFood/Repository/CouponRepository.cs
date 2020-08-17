using SpeedyFood.Data;
using SpeedyFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Repository
{
    public class CouponRepository: GenericRepository<Coupon>, ICouponRepository
    {
        public CouponRepository(ApplicationDbContext context): base(context)
        {

        }
    }
}
