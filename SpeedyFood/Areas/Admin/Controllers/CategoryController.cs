using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpeedyFood.Models;
using SpeedyFood.Repository;

namespace SpeedyFood.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAll();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if(ModelState.IsValid)
            {
                await _categoryRepository.AddCategory(category);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(category);
            }
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            Category category = await _categoryRepository.GetCategoryById((int)id);
            if(category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.EditCategory(category);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(category);
            }
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Category category = await _categoryRepository.GetCategoryById((int)id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            Category category = await _categoryRepository.GetCategoryById(id);
            if(category == null)
            {
                return View();
            }
            await _categoryRepository.DeleteCategory(category);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Category category = await _categoryRepository.GetCategoryById((int)id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
    }
}
