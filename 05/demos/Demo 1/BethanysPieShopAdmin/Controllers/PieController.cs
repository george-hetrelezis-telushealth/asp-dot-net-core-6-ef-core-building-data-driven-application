using BethanysPieShopAdmin.Models;
using BethanysPieShopAdmin.Models.Repositories;
using BethanysPieShopAdmin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BethanysPieShopAdmin.Controllers
{
    public class PieController : Controller
    {
        private readonly IPieRepository _pieRepository;
        private readonly ICategoryRepository _categoryRepository;


        public PieController(IPieRepository pieRepository, ICategoryRepository categoryRepository)
        {
            _pieRepository = pieRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var pies = await _pieRepository.GetAllPiesAsync();
            return View(pies);
        }

        public async Task<IActionResult> Details(int id)
        {
            var pie = await _pieRepository.GetPieByIdAsync(id);
            return View(pie);
        }

        public async Task<IActionResult> Add()
        {
            try
            {
                IEnumerable<Category>? allCategories = await _categoryRepository.GetAllCategoriesAsync();
                IEnumerable<SelectListItem> selectListItems = new SelectList(allCategories, "CategoryId", "Name", null);

                PieAddViewModel pieAddViewModel = new() { Categories = selectListItems };
                return View(pieAddViewModel);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"There was an error: {ex.Message}";
            }
            return View(new PieAddViewModel());
           
        }

        [HttpPost]
        public async Task<IActionResult> Add(PieAddViewModel pieAddViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Pie pie = new()
                    {
                        CategoryId = pieAddViewModel.Pie.CategoryId,
                        ShortDescription = pieAddViewModel.Pie.ShortDescription,
                        LongDescription = pieAddViewModel.Pie.LongDescription,
                        Price = pieAddViewModel.Pie.Price,
                        AllergyInformation = pieAddViewModel.Pie.AllergyInformation,
                        ImageThumbnailUrl = pieAddViewModel.Pie.ImageThumbnailUrl,
                        ImageUrl = pieAddViewModel.Pie.ImageUrl,
                        InStock = pieAddViewModel.Pie.InStock,
                        IsPieOfTheWeek = pieAddViewModel.Pie.IsPieOfTheWeek,
                        Name = pieAddViewModel.Pie.Name
                    };

                    await _pieRepository.AddPieAsync(pie);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Adding the pie failed, please try again! Error: {ex.Message}");
            }

            var allCategories = await _categoryRepository.GetAllCategoriesAsync();

            IEnumerable<SelectListItem> selectListItems = new SelectList(allCategories, "CategoryId", "Name", null);

            pieAddViewModel.Categories = selectListItems;

            return View(pieAddViewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allCategories = await _categoryRepository.GetAllCategoriesAsync();

            IEnumerable<SelectListItem> selectListItems = new SelectList(allCategories, "CategoryId", "Name", null);

            var selectedPie = await _pieRepository.GetPieByIdAsync(id.Value);

            PieEditViewModel pieEditViewModel = new() { Categories = selectListItems, Pie = selectedPie };
            return View(pieEditViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(PieEditViewModel pieEditViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _pieRepository.UpdatePieAsync(pieEditViewModel.Pie);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Updating the category failed, please try again! Error: {ex.Message}");
            }

            var allCategories = await _categoryRepository.GetAllCategoriesAsync();

            IEnumerable<SelectListItem> selectListItems = new SelectList(allCategories, "CategoryId", "Name", null);

            pieEditViewModel.Categories = selectListItems;

            return View(pieEditViewModel);
        }
    }
}
