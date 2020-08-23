using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpeedyFood.Models;
using SpeedyFood.Models.ViewModels;
using SpeedyFood.Repository;
using SpeedyFood.Utility;

namespace SpeedyFood.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }
        public ShoppingCartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            ShoppingCartVM = new ShoppingCartViewModel()
            {
                OrderHeader = new OrderHeader()
            };
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var shoppingCarts = _unitOfWork.ShoppingCart.Find(m => m.ApplicationUserId == claims.Value).ToList();
            if (shoppingCarts != null)
            {
                ShoppingCartVM.ShoppingCarts = shoppingCarts;
            }
            foreach (var item in ShoppingCartVM.ShoppingCarts)
            {
                item.MenuItem = await _unitOfWork.MenuItem.GetById(item.MenuItemId);
                ShoppingCartVM.OrderHeader.OrderTotalBeforeCoupon =
                    ShoppingCartVM.OrderHeader.OrderTotalBeforeCoupon + (item.MenuItem.Price * item.Count);
                if(item.MenuItem.Description.Length > 100)
                {
                    item.MenuItem.Description = item.MenuItem.Description.Substring(0, 99) + "...";
                }
            }

            ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.OrderHeader.OrderTotalBeforeCoupon;

            if(HttpContext.Session.GetString(StaticDetails.ssCouponCode) != null)
            {
                ShoppingCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(StaticDetails.ssCouponCode);
                var couponFromDb = _unitOfWork.Coupon.Find(m => m.Name.ToLower() == ShoppingCartVM.OrderHeader.CouponCode.ToLower()).FirstOrDefault();
                ShoppingCartVM.OrderHeader.OrderTotal = StaticDetails.CountDiscount(couponFromDb, ShoppingCartVM.OrderHeader.OrderTotalBeforeCoupon);
            }

            return View(ShoppingCartVM);
        }

        public IActionResult ApplyCoupon()
        {
            if(ShoppingCartVM.OrderHeader.CouponCode == null)
            {
                ShoppingCartVM.OrderHeader.CouponCode = "";
            }

            HttpContext.Session.SetString(StaticDetails.ssCouponCode, ShoppingCartVM.OrderHeader.CouponCode);

            return RedirectToAction(nameof(Index));
        }
        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.SetString(StaticDetails.ssCouponCode, string.Empty);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Plus(int cartId)
        {
            var shoppingCart = await _unitOfWork.ShoppingCart.GetById(cartId);
            shoppingCart.Count += 1;
            _unitOfWork.Complete();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Remove(int cartId)
        {
            var shoppingCart = await _unitOfWork.ShoppingCart.GetById(cartId);
            _unitOfWork.ShoppingCart.Delete(shoppingCart);
            _unitOfWork.Complete();

            var count = _unitOfWork.ShoppingCart.CountApplicationUser(shoppingCart.ApplicationUserId);
            HttpContext.Session.SetInt32(StaticDetails.ssCartCount, count);

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Minus(int cartId)
        {
            var shoppingCart = await _unitOfWork.ShoppingCart.GetById(cartId);
            if(shoppingCart.Count == 1)
            {
                _unitOfWork.ShoppingCart.Delete(shoppingCart);
                _unitOfWork.Complete();

                var count = _unitOfWork.ShoppingCart.CountApplicationUser(shoppingCart.ApplicationUserId);
                HttpContext.Session.SetInt32(StaticDetails.ssCartCount, count);
            }
            else
            {
                shoppingCart.Count -= 1;
                _unitOfWork.Complete();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Summary()
        {
            ShoppingCartVM = new ShoppingCartViewModel()
            {
                OrderHeader = new OrderHeader()
            };
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ApplicationUser applicationUser = _unitOfWork.User.Find(m => m.Id == claims.Value).FirstOrDefault();

            var shoppingCarts = _unitOfWork.ShoppingCart.Find(m => m.ApplicationUserId == claims.Value).ToList();
            if (shoppingCarts != null)
            {
                ShoppingCartVM.ShoppingCarts = shoppingCarts;
            }
            foreach (var item in ShoppingCartVM.ShoppingCarts)
            {
                item.MenuItem = await _unitOfWork.MenuItem.GetById(item.MenuItemId);
                ShoppingCartVM.OrderHeader.OrderTotalBeforeCoupon =
                    ShoppingCartVM.OrderHeader.OrderTotalBeforeCoupon + (item.MenuItem.Price * item.Count);
            }

            ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.OrderHeader.OrderTotalBeforeCoupon;
            ShoppingCartVM.OrderHeader.PickUpName = applicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.PickUpDateAndTime = DateTime.Now;

            if (HttpContext.Session.GetString(StaticDetails.ssCouponCode) != null)
            {
                ShoppingCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(StaticDetails.ssCouponCode);
                var couponFromDb = _unitOfWork.Coupon.Find(m => m.Name.ToLower() == ShoppingCartVM.OrderHeader.CouponCode.ToLower()).FirstOrDefault();
                ShoppingCartVM.OrderHeader.OrderTotal = StaticDetails.CountDiscount(couponFromDb, ShoppingCartVM.OrderHeader.OrderTotalBeforeCoupon);
            }

            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM.ShoppingCarts = _unitOfWork.ShoppingCart.Find(m => m.ApplicationUserId == claims.Value).ToList();
            ShoppingCartVM.OrderHeader.ApplicationUserId = claims.Value;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.Status = StaticDetails.StatusInProgress;
            ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusPending;

            await _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Complete();

            foreach(var item in ShoppingCartVM.ShoppingCarts)
            {
                item.MenuItem = await _unitOfWork.MenuItem.GetById(item.MenuItemId);
                OrderDetails orderDetails = new OrderDetails()
                {
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    MenuItemId = item.MenuItemId,
                    Price = item.MenuItem.Price,
                    Count = item.Count
                };
                await _unitOfWork.OrderDetails.Add(orderDetails);
                ShoppingCartVM.OrderHeader.OrderTotalBeforeCoupon += (orderDetails.MenuItem.Price * orderDetails.Count);
            }

            if (HttpContext.Session.GetString(StaticDetails.ssCouponCode) != null)
            {
                ShoppingCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(StaticDetails.ssCouponCode);
                var couponFromDb = _unitOfWork.Coupon.Find(m => m.Name.ToLower() == ShoppingCartVM.OrderHeader.CouponCode.ToLower()).FirstOrDefault();
                ShoppingCartVM.OrderHeader.OrderTotal = StaticDetails.CountDiscount(couponFromDb, ShoppingCartVM.OrderHeader.OrderTotalBeforeCoupon);
            }
            else
            {
                ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.OrderHeader.OrderTotalBeforeCoupon;
            }

            _unitOfWork.ShoppingCart.RemoveList(ShoppingCartVM.ShoppingCarts);
            HttpContext.Session.SetInt32(StaticDetails.ssCartCount, 0);

            _unitOfWork.Complete();

            return RedirectToAction("ConfirmOrder", "Order", new { id = ShoppingCartVM.OrderHeader.Id});
        }
    }
}
