using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    }
}
