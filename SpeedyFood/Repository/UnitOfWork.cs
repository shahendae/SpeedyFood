using SpeedyFood.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public ICategoryRepository Category { get; }
        public ISubCategoryRepository SubCategory { get; }
        public IMenuItemRepository MenuItem { get; }
        public ICouponRepository Coupon { get; }
        public IUserRepository User { get; }
        public IShoppingCartRepository ShoppingCart { get; }
        public IOrderHeaderRepository OrderHeader { get; }
        public IOrderDetailsRepository OrderDetails { get; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Category = new CategoryRepository(_context);
            SubCategory = new SubCategoryRepository(_context);
            MenuItem = new MenuItemRepository(_context);
            Coupon = new CouponRepository(_context);
            User = new UserRepository(_context);
            ShoppingCart = new ShoppingCartRepository(_context);
            OrderHeader = new OrderHeaderRepository(_context);
            OrderDetails = new OrderDetailsRepository(_context);
        }
        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
