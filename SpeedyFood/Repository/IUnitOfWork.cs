﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Repository
{
    public interface IUnitOfWork: IDisposable
    {
        ICategoryRepository Category { get; }
        ISubCategoryRepository SubCategory { get; }
        IMenuItemRepository MenuItem { get; }
        ICouponRepository Coupon { get; }
        int Complete();
    }
}
