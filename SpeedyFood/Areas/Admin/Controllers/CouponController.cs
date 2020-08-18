using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeedyFood.Models;
using SpeedyFood.Repository;
using SpeedyFood.Utility;

namespace SpeedyFood.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.ManagerUser)]

    public class CouponController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CouponController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var coupons = await _unitOfWork.Coupon.GetAll();
            return View(coupons);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                // Save Image in Database
                var files = HttpContext.Request.Form.Files;
                if(files.Count > 0)
                {
                    //convert files into stream of bytes 
                    byte[] p1 = null;
                    using(var fs = files[0].OpenReadStream())
                    {
                        using(var ms = new MemoryStream())
                        {
                            fs.CopyTo(ms);
                            p1 = ms.ToArray();
                        }
                    }
                    coupon.Image = p1;
                }

                await _unitOfWork.Coupon.Add(coupon);
                _unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }

            return View(coupon);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var coupon = await _unitOfWork.Coupon.GetById((int)id);
            if (coupon == null)
            {
                return NotFound();
            }

            string imgbase64 = Convert.ToBase64String(coupon.Image);
            ViewData["imgSrc"] = imgbase64;
            return View(coupon);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                var couponFromDb = await _unitOfWork.Coupon.GetById(coupon.Id);
                var files = HttpContext.Request.Form.Files;
                if(files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs = files[0].OpenReadStream())
                    {
                        using (var ms = new MemoryStream())
                        {
                            fs.CopyTo(ms);
                            p1 = ms.ToArray();
                        }
                    }

                    coupon.Image = p1;
                    couponFromDb.Image = coupon.Image;
                }

                couponFromDb.Name = coupon.Name;
                couponFromDb.CouponType = coupon.CouponType;
                couponFromDb.IsActive = coupon.IsActive;
                couponFromDb.Discount = coupon.Discount;
                couponFromDb.MinAmount = coupon.MinAmount;

                _unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }

        public async Task<IActionResult> Details(int id)
        {
            var coupon = await _unitOfWork.Coupon.GetById(id);
            if(coupon == null)
            {
                return NotFound();
            }
            string imgbase64 = Convert.ToBase64String(coupon.Image);
            ViewData["imgSrc"] = imgbase64;
            return View(coupon);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var coupon = await _unitOfWork.Coupon.GetById((int)id);
            if (coupon == null)
            {
                return NotFound();
            }
            string imgbase64 = Convert.ToBase64String(coupon.Image);
            ViewData["imgSrc"] = imgbase64;
            return View(coupon);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var coupon = await _unitOfWork.Coupon.GetById(id);
            if (coupon == null)
            {
                return View();
            }
            _unitOfWork.Coupon.Delete(coupon);
            _unitOfWork.Complete();
            return RedirectToAction(nameof(Index));
        }

    }
}
