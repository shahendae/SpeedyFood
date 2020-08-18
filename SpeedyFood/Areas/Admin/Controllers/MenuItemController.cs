using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SpeedyFood.Models;
using SpeedyFood.Models.ViewModels;
using SpeedyFood.Repository;
using SpeedyFood.Utility;

namespace SpeedyFood.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.ManagerUser)]

    public class MenuItemController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        //for saving image in server 
        private readonly IWebHostEnvironment _webHost;

        public MenuItemController(IUnitOfWork unitOfWork, IWebHostEnvironment webHost)
        {
            _unitOfWork = unitOfWork;
            _webHost = webHost;
        }
        public async Task<IActionResult> Index()
        {
            var menuItem = await _unitOfWork.MenuItem.GetMenuItemsWithCategoryAndSubCategory();
            return View(menuItem);
        }
        public async Task<IActionResult> Create()
        {
            var menuItemCategoryVM = new MenuItemCategoryViewModel()
            {
                Categories = await _unitOfWork.Category.GetAll(),
                MenuItem = new MenuItem()
            };
            return View(menuItemCategoryVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuItemCategoryViewModel model)
        {
            model.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            if (ModelState.IsValid)
            {
                await _unitOfWork.MenuItem.Add(model.MenuItem);
                _unitOfWork.Complete();
                // save image on server side inside wwwroot file
                //Get the root path
                string webRootPath = _webHost.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                var menuItemFromDb = await _unitOfWork.MenuItem.GetById(model.MenuItem.Id);

                if(files.Count() > 0)
                {
                    //get images inside wwwroot
                    var uploads = Path.Combine(webRootPath, "images");
                    //get file extension 
                    var extension = Path.GetExtension(files[0].FileName);
                    // create image with name of the id 
                    using(var filesStream = new FileStream(Path.Combine(uploads, model.MenuItem.Id + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filesStream);
                    }
                    menuItemFromDb.Image = @"\images\" + model.MenuItem.Id + extension;
                }
                else
                {
                    var uploads = Path.Combine(webRootPath, @"images\" + StaticDetails.DefaultFoodImage);
                    System.IO.File.Copy(uploads, webRootPath + @"\images\" + model.MenuItem.Id + ".png");
                    menuItemFromDb.Image = @"\images\" + model.MenuItem.Id + ".png";
                }
                _unitOfWork.Complete();
                return RedirectToAction(nameof(Index));

            }
            MenuItemCategoryViewModel menuItemVM = new MenuItemCategoryViewModel()
            {
                Categories = await _unitOfWork.Category.GetAll(),
                MenuItem = model.MenuItem
            };
            return View(menuItemVM);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var menuItem = await _unitOfWork.MenuItem.GetMenuItemWithCategoryAndSubCategory((int)id);
            if(menuItem == null)
            {
                return NotFound();
            }
            var menuItemCategoryVM = new MenuItemCategoryViewModel()
            {
                Categories = await _unitOfWork.Category.GetAll(),
                MenuItem = menuItem,
                SubCategories = _unitOfWork.SubCategory.Find(m => m.CategoryId == menuItem.CategoryId).ToList()
            };
            return View(menuItemCategoryVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MenuItemCategoryViewModel model)
        {
            model.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            if (ModelState.IsValid)
            {
                // save image on server side inside wwwroot file
                string webRootPath = _webHost.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                var menuItemFromDb = await _unitOfWork.MenuItem.GetById(model.MenuItem.Id);

                if (files.Count() > 0)
                {
                    var uploads = Path.Combine(webRootPath, "images");
                    var extension = Path.GetExtension(files[0].FileName);
                    //Delete the original image 
                    var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                    // Add new Image 
                    using (var filesStream = new FileStream(Path.Combine(uploads, model.MenuItem.Id + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filesStream);
                    }

                    menuItemFromDb.Image = @"\images\" + model.MenuItem.Id + extension;
                }

                menuItemFromDb.Name = model.MenuItem.Name;
                menuItemFromDb.Price = model.MenuItem.Price;
                menuItemFromDb.Description = model.MenuItem.Description;
                menuItemFromDb.Spicyness = model.MenuItem.Spicyness;
                menuItemFromDb.CategoryId = model.MenuItem.CategoryId;
                menuItemFromDb.SubCategoryId = model.MenuItem.SubCategoryId;

                _unitOfWork.Complete();

                return RedirectToAction(nameof(Index));
            }

            MenuItemCategoryViewModel menuItemVM = new MenuItemCategoryViewModel()
            {
                Categories = await _unitOfWork.Category.GetAll(),
                MenuItem = model.MenuItem,
                SubCategories = _unitOfWork.SubCategory.Find(m => m.CategoryId == model.MenuItem.CategoryId).ToList()
            };

            return View(menuItemVM);
        }

        public async Task<IActionResult> Details(int id)
        {
            var menuItem = await _unitOfWork.MenuItem.GetMenuItemWithCategoryAndSubCategory(id);
            if (menuItem == null)
            {
                return NotFound();
            }
            return View(menuItem);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var menuItem = await _unitOfWork.MenuItem.GetMenuItemWithCategoryAndSubCategory((int)id);
            if (menuItem == null)
            {
                return NotFound();
            }
            return View(menuItem);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var menuItem = await _unitOfWork.MenuItem.GetById(id);
            if(menuItem == null)
            {
                return View();
            }
            var webRootPath = _webHost.WebRootPath;
            var imagePath = Path.Combine(webRootPath, menuItem.Image.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            _unitOfWork.MenuItem.Delete(menuItem);
            _unitOfWork.Complete();
            return RedirectToAction(nameof(Index));
        }
    }
}
