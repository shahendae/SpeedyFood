using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpeedyFood.Models;
using SpeedyFood.Models.ViewModels;
using SpeedyFood.Repository;
using SpeedyFood.Utility;

namespace SpeedyFood.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderDetailsViewModel orderDetailsViewModel { get; set; }
        public ShoppingCartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            orderDetailsViewModel = new OrderDetailsViewModel()
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

            if(HttpContext.Session.GetString(StaticDetails.ssCouponCode) != null)
            {
                orderDetailsViewModel.OrderHeader.CouponCode = HttpContext.Session.GetString(StaticDetails.ssCouponCode);
                var couponFromDb = _unitOfWork.Coupon.Find(m => m.Name.ToLower() == orderDetailsViewModel.OrderHeader.CouponCode.ToLower()).FirstOrDefault();
                orderDetailsViewModel.OrderHeader.OrderTotal = StaticDetails.CountDiscount(couponFromDb, orderDetailsViewModel.OrderHeader.OrderTotalBeforeCoupon);
            }

            return View(orderDetailsViewModel);
        }

        public IActionResult ApplyCoupon()
        {
            if(orderDetailsViewModel.OrderHeader.CouponCode == null)
            {
                orderDetailsViewModel.OrderHeader.CouponCode = "";
            }

            HttpContext.Session.SetString(StaticDetails.ssCouponCode, orderDetailsViewModel.OrderHeader.CouponCode);

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
            shoppingCart.Count = shoppingCart.Count + 1;
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
            orderDetailsViewModel = new OrderDetailsViewModel()
            {
                OrderHeader = new OrderHeader()
            };
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ApplicationUser applicationUser = _unitOfWork.User.Find(m => m.Id == claims.Value).FirstOrDefault();

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
            }

            orderDetailsViewModel.OrderHeader.OrderTotal = orderDetailsViewModel.OrderHeader.OrderTotalBeforeCoupon;
            orderDetailsViewModel.OrderHeader.PickUpName = applicationUser.Name;
            orderDetailsViewModel.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            orderDetailsViewModel.OrderHeader.PickUpDateAndTime = DateTime.Now;

            if (HttpContext.Session.GetString(StaticDetails.ssCouponCode) != null)
            {
                orderDetailsViewModel.OrderHeader.CouponCode = HttpContext.Session.GetString(StaticDetails.ssCouponCode);
                var couponFromDb = _unitOfWork.Coupon.Find(m => m.Name.ToLower() == orderDetailsViewModel.OrderHeader.CouponCode.ToLower()).FirstOrDefault();
                orderDetailsViewModel.OrderHeader.OrderTotal = StaticDetails.CountDiscount(couponFromDb, orderDetailsViewModel.OrderHeader.OrderTotalBeforeCoupon);
            }

            return View(orderDetailsViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            orderDetailsViewModel.ShoppingCarts = _unitOfWork.ShoppingCart.Find(m => m.ApplicationUserId == claims.Value).ToList();
            orderDetailsViewModel.OrderHeader.ApplicationUserId = claims.Value;
            orderDetailsViewModel.OrderHeader.OrderDate = DateTime.Now;
            orderDetailsViewModel.OrderHeader.Status = StaticDetails.StatusInProgress;
            orderDetailsViewModel.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusPending;

            await _unitOfWork.OrderHeader.Add(orderDetailsViewModel.OrderHeader);
            _unitOfWork.Complete();

            foreach(var item in orderDetailsViewModel.ShoppingCarts)
            {
                item.MenuItem = await _unitOfWork.MenuItem.GetById(item.MenuItemId);
                OrderDetails orderDetails = new OrderDetails()
                {
                    OrderHeaderId = orderDetailsViewModel.OrderHeader.Id,
                    MenuItemId = item.MenuItemId,
                    Price = item.MenuItem.Price,
                    Count = item.Count
                };
                await _unitOfWork.OrderDetails.Add(orderDetails);
                orderDetailsViewModel.OrderHeader.OrderTotalBeforeCoupon += (orderDetails.MenuItem.Price * orderDetails.Count);
            }

            if (HttpContext.Session.GetString(StaticDetails.ssCouponCode) != null)
            {
                orderDetailsViewModel.OrderHeader.CouponCode = HttpContext.Session.GetString(StaticDetails.ssCouponCode);
                var couponFromDb = _unitOfWork.Coupon.Find(m => m.Name.ToLower() == orderDetailsViewModel.OrderHeader.CouponCode.ToLower()).FirstOrDefault();
                orderDetailsViewModel.OrderHeader.OrderTotal = StaticDetails.CountDiscount(couponFromDb, orderDetailsViewModel.OrderHeader.OrderTotalBeforeCoupon);
            }
            else
            {
                orderDetailsViewModel.OrderHeader.OrderTotal = orderDetailsViewModel.OrderHeader.OrderTotalBeforeCoupon;
            }

            _unitOfWork.ShoppingCart.RemoveList(orderDetailsViewModel.ShoppingCarts);
            HttpContext.Session.SetInt32(StaticDetails.ssCartCount, 0);

            _unitOfWork.Complete();

            return RedirectToAction("Index", "Home");
        }
    }
}
