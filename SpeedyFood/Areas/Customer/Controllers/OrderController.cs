using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeedyFood.Models;
using SpeedyFood.Models.ViewModels;
using SpeedyFood.Repository;

namespace SpeedyFood.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> ConfirmOrder(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderViewModel orderViewModel = new OrderViewModel()
            {
                OrderHeader = await _unitOfWork.OrderHeader.GetOrderHeaderWithApplicationUser(id, claims.Value),
                OrderDetailsList = _unitOfWork.OrderDetails.GetOrderDetailsWithMenuItems(id)
            };

            return View(orderViewModel);
        }
        [Authorize]
        public async Task<IActionResult> OrderHistory()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<OrderViewModel> orderVMList = new List<OrderViewModel>();

            List<OrderHeader> orderHeadersList = await _unitOfWork.OrderHeader.GetOrderHeadersWithApplicationUser(claims.Value);
            foreach(var item in orderHeadersList)
            {
                OrderViewModel orderVM = new OrderViewModel()
                {
                    OrderHeader = item,
                    OrderDetailsList = _unitOfWork.OrderDetails.GetOrderDetailsWithMenuItems(item.Id)
                };

                orderVMList.Add(orderVM);
            }

            return View(orderVMList);
        }

        public async Task<IActionResult> GetOrderDetails(int Id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderViewModel orderVM = new OrderViewModel()
            {
                OrderHeader = await _unitOfWork.OrderHeader.GetOrderHeaderWithApplicationUser(Id, claims.Value),
                OrderDetailsList = _unitOfWork.OrderDetails.GetOrderDetailsWithMenuItems(Id)
            };

            return PartialView("_OrderDetailsPartial", orderVM);
        }
        public async Task<IActionResult> GetOrderStatus(int Id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderHeader orderHeader = await _unitOfWork.OrderHeader.GetOrderHeaderWithApplicationUser(Id, claims.Value);

            return PartialView("_OrderStatusPartial", orderHeader.Status);
        }
    }
}
