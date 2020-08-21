using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpeedyFood.Models;
using SpeedyFood.Models.ViewModels;
using SpeedyFood.Repository;

namespace SpeedyFood.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel()
            {
                OrderHeader = new OrderHeader()
            };
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var shoppingCarts = _unitOfWork.ShoppingCart.Find(m => m.ApplicationUserId == claims.Value).ToList();
            if (shoppingCarts != null)
            {
                orderDetailsViewModel.ShoppingCarts = shoppingCarts;
            }
            foreach (var item in orderDetailsViewModel.ShoppingCarts)
            {
                item.MenuItem = await _unitOfWork.MenuItem.GetById(item.MenuItemId);
                orderDetailsViewModel.OrderHeader.OrderTotalBeforeCoupon =
                    orderDetailsViewModel.OrderHeader.OrderTotalBeforeCoupon + (item.MenuItem.Price * item.Count);
                if(item.MenuItem.Description.Length > 100)
                {
                    item.MenuItem.Description = item.MenuItem.Description.Substring(0, 99) + "...";
                }
            }

            orderDetailsViewModel.OrderHeader.OrderTotal = orderDetailsViewModel.OrderHeader.OrderTotalBeforeCoupon;

            return View(orderDetailsViewModel);
        }
    }
}
