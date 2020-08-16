using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SpeedyFood.Models;
using SpeedyFood.Models.ViewModels;
using SpeedyFood.Repository;

namespace SpeedyFood.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [TempData]
        public string StatusMessg { get; set; }

        public SubCategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // GET: SubCategoryController
        public async Task<ActionResult> Index()
        {
            var subCategory = await _unitOfWork.SubCategory.GetSubCategoriesWithCategory();
            return View(subCategory);
        }

        // GET: SubCategoryController/Details/5
        public ActionResult Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var subCategory =  _unitOfWork.SubCategory.GetSubCategoryWithCategory((int)id);
            if(subCategory == null)
            {
                return NotFound();
            }
            return View(subCategory);
        }

        // GET: SubCategoryController/Create
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                Categories = await _unitOfWork.Category.GetAll(),
                SubCategoriesList = await _unitOfWork.SubCategory.GetSubCategoriesName(),
                SubCategory = new SubCategory()
            };
            return View(model);
        }

        // POST: SubCategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if(ModelState.IsValid)
            {
                var ExistingSubCategories = _unitOfWork.SubCategory.Find(m => m.Name == model.SubCategory.Name && m.CategoryId == model.SubCategory.CategoryId);
                if(ExistingSubCategories.Count() > 0)
                {
                    StatusMessg = "Error : Sub Category Exists before";
                }
                else
                {
                    await _unitOfWork.SubCategory.Add(model.SubCategory);
                    _unitOfWork.Complete();
                    return RedirectToAction(nameof(Index));
                }
            }
            var viewModel = new SubCategoryAndCategoryViewModel()
            {
                Categories = await _unitOfWork.Category.GetAll(),
                SubCategoriesList = await _unitOfWork.SubCategory.GetSubCategoriesName(),
                SubCategory = model.SubCategory,
                StatusMessg = StatusMessg
            };
            return View(viewModel);
        }

        // GET: SubCategoryController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var subCategory = _unitOfWork.SubCategory.Find(m => m.Id == id).FirstOrDefault();
            if(subCategory == null)
            {
                return NotFound();
            }
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                Categories = await _unitOfWork.Category.GetAll(),
                SubCategoriesList = await _unitOfWork.SubCategory.GetSubCategoriesName(),
                SubCategory = subCategory
            };
            return View(model);
        }

        // POST: SubCategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var ExistingSubCategories = _unitOfWork.SubCategory.Find(m => m.Name == model.SubCategory.Name && m.CategoryId == model.SubCategory.CategoryId);
                if (ExistingSubCategories.Count() > 0)
                {
                    StatusMessg = "Error : Sub Category Exists before";
                }
                else
                {
                    var subCategory = await _unitOfWork.SubCategory.GetById(model.SubCategory.Id);
                    subCategory.Name = model.SubCategory.Name;
                    _unitOfWork.Complete();
                    return RedirectToAction(nameof(Index));
                }
            }
            var viewModel = new SubCategoryAndCategoryViewModel()
            {
                Categories = await _unitOfWork.Category.GetAll(),
                SubCategoriesList = await _unitOfWork.SubCategory.GetSubCategoriesName(),
                SubCategory = model.SubCategory,
                StatusMessg = StatusMessg
            };
            return View(viewModel);
        }

        // GET: SubCategoryController/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = _unitOfWork.SubCategory.GetSubCategoryWithCategory((int)id);
            if (subCategory == null)
            {
                return NotFound();
            }
            return View(subCategory);
        }

        // POST: SubCategoryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var subCategory = await _unitOfWork.SubCategory.GetById(id);
            if(subCategory == null)
            {
                return View();
            }
            _unitOfWork.SubCategory.Delete(subCategory);
            _unitOfWork.Complete();
            return RedirectToAction(nameof(Index));
        }

        [ActionName("GetSubCategories")]
        public IActionResult GetSubCategories(int id)
        {
            List<SubCategory> subCategories = _unitOfWork.SubCategory.Find(m => m.CategoryId == id).ToList();
            return Json(new SelectList(subCategories, "Id", "Name"));
        }
    }
}
