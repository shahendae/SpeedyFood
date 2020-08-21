using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpeedyFood.Models;
using SpeedyFood.Models.ViewModels;
using SpeedyFood.Repository;

namespace SpeedyFood.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel indexVM = new IndexViewModel()
            {
                Categories = await _unitOfWork.Category.GetAll(),
                MenuItems = await _unitOfWork.MenuItem.GetMenuItemsWithCategoryAndSubCategory(),
                Coupons = _unitOfWork.Coupon.Find(m => m.IsActive == true).ToList()
            };

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim != null)
            {
                int count = _unitOfWork.ShoppingCart.CountApplicationUser(claim.Value);
                HttpContext.Session.SetInt32("ssCartCount", count);
            }

            return View(indexVM);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var menuItem = await _unitOfWork.MenuItem.GetMenuItemWithCategoryAndSubCategory(id);
            ShoppingCart shoppingCart = new ShoppingCart()
            {
                MenuItem = menuItem,
                MenuItemId = menuItem.Id
            };

            return View(shoppingCart);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = 0;
            if (ModelState.IsValid)
            {
                //Get user Id 
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                shoppingCart.ApplicationUserId = claim.Value;
                var shoppingCartFromDb = _unitOfWork.ShoppingCart.Find(m => m.MenuItemId == shoppingCart.MenuItemId
                && m.ApplicationUserId == shoppingCart.ApplicationUserId).FirstOrDefault();
                if (shoppingCartFromDb == null)
                {
                    await _unitOfWork.ShoppingCart.Add(shoppingCart);
                }
                else
                {
                    shoppingCartFromDb.Count = shoppingCartFromDb.Count + shoppingCart.Count;
                }
                _unitOfWork.Complete();
                var count = _unitOfWork.ShoppingCart.CountApplicationUser(shoppingCart.ApplicationUserId);
                HttpContext.Session.SetInt32("ssCartCount", count);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var menuItem = await _unitOfWork.MenuItem.GetMenuItemWithCategoryAndSubCategory(shoppingCart.MenuItemId);
                ShoppingCart shoppingCartObj = new ShoppingCart()
                {
                    MenuItem = menuItem,
                    MenuItemId = menuItem.Id
                };

                return View(shoppingCartObj);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
