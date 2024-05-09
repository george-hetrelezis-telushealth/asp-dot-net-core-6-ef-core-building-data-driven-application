using BethanysPieShopAdmin.Models;
using BethanysPieShopAdmin.Models.Repositories;
using BethanysPieShopAdmin.Utilities;
using BethanysPieShopAdmin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BethanysPieShopAdmin.Controllers
{
    public class PieController : Controller
    {
        private readonly IPieRepository _pieRepository;
        private readonly ICategoryRepository _categoryRepository;
        private int pageSize = 5;


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

        public async Task<IActionResult> IndexPaging(int? pageNumber)
        {
            var pies = await _pieRepository.GetPiesPagedAsync(pageNumber, pageSize);

            pageNumber ??= 1;

            var count = await _pieRepository.GetAllPiesCountAsync();

            return View(new PaginatedList<Pie>(pies.ToList(), count, pageNumber.Value, pageSize));
        }

        public async Task<IActionResult> IndexPagingSorting(string sortBy, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortBy;

            ViewData["IdSortParam"] = String.IsNullOrEmpty(sortBy) || sortBy == "id_desc" ? "id" : "id_desc";
            ViewData["NameSortParam"] = sortBy == "name" ? "name_desc" : "name";
            ViewData["PriceSortParam"] = sortBy == "price" ? "price_desc" : "price";

            pageNumber ??= 1;

            var pies = await _pieRepository.GetPiesSortedAndPagedAsync(sortBy, pageNumber, pageSize);

            var count = await _pieRepository.GetAllPiesCountAsync();

            return View(new PaginatedList<Pie>(pies.ToList(), count, pageNumber.Value, pageSize));
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


        //[HttpPost]
        //public async Task<IActionResult> Edit(Pie pie)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            await _pieRepository.UpdatePieAsync(pie);
        //            return RedirectToAction(nameof(Index));
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", $"Updating the category failed, please try again! Error: {ex.Message}");
        //    }

        //    var allCategories = await _categoryRepository.GetAllCategoriesAsync();

        //    IEnumerable<SelectListItem> selectListItems = new SelectList(allCategories, "CategoryId", "Name", null);

        //    PieEditViewModel pieEditViewModel = new() { Categories = selectListItems, Pie = pie };

        //    return View(pieEditViewModel);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Pie pie)
        {
            Pie pieToUpdate = await _pieRepository.GetPieByIdAsync(pie.PieId);

            try
            {
                if (pieToUpdate == null)
                {
                    ModelState.AddModelError(string.Empty, "The pie you want to update doesn't exist or was already deleted by someone else.");
                }

                if (ModelState.IsValid)
                {
                    await _pieRepository.UpdatePieAsync(pie);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionPie = ex.Entries.Single();
                var entityValues = (Pie)exceptionPie.Entity;
                var databasePie = exceptionPie.GetDatabaseValues();

                if (databasePie == null)
                {
                    ModelState.AddModelError(string.Empty, "The pie was already deleted by another user.");
                }
                else
                {
                    var databaseValues = (Pie)databasePie.ToObject();

                    if (databaseValues.Name != entityValues.Name)
                    {
                        ModelState.AddModelError("Pie.Name", $"Current value: {databaseValues.Name}");
                    }
                    if (databaseValues.Price != entityValues.Price)
                    {
                        ModelState.AddModelError("Pie.Price", $"Current value: {databaseValues.Price:c}");
                    }
                    if (databaseValues.ShortDescription != entityValues.ShortDescription)
                    {
                        ModelState.AddModelError("Pie.ShortDescription", $"Current value: {databaseValues.ShortDescription}");
                    }
                    if (databaseValues.LongDescription != entityValues.LongDescription)
                    {
                        ModelState.AddModelError("Pie.LongDescription", $"Current value: {databaseValues.LongDescription}");
                    }
                    if (databaseValues.AllergyInformation != entityValues.AllergyInformation)
                    {
                        ModelState.AddModelError("Pie.AllergyInformation", $"Current value: {databaseValues.AllergyInformation}");
                    }
                    if (databaseValues.ImageThumbnailUrl != entityValues.ImageThumbnailUrl)
                    {
                        ModelState.AddModelError("Pie.ImageThumbnailUrl", $"Current value: {databaseValues.ImageThumbnailUrl}");
                    }
                    if (databaseValues.ImageUrl != entityValues.ImageUrl)
                    {
                        ModelState.AddModelError("Pie.ImageUrl", $"Current value: {databaseValues.ImageUrl}");
                    }
                    if (databaseValues.IsPieOfTheWeek != entityValues.IsPieOfTheWeek)
                    {
                        ModelState.AddModelError("Pie.IsPieOfTheWeek", $"Current value: {databaseValues.IsPieOfTheWeek}");
                    }
                    if (databaseValues.InStock != entityValues.InStock)
                    {
                        ModelState.AddModelError("Pie.InStock", $"Current value: {databaseValues.InStock}");
                    }
                    if (databaseValues.CategoryId != entityValues.CategoryId)
                    {
                        ModelState.AddModelError("Pie.CategoryId", $"Current value: {databaseValues.CategoryId}");
                    }

                    ModelState.AddModelError(string.Empty, "The pie was modified already by another user. The database values are now shown. Hit Save again to store these values.");
                    pieToUpdate.RowVersion = databaseValues.RowVersion;

                    ModelState.Remove("Pie.RowVersion");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Updating the category failed, please try again! Error: {ex.Message}");
            }

            var allCategories = await _categoryRepository.GetAllCategoriesAsync();

            IEnumerable<SelectListItem> selectListItems = new SelectList(allCategories, "CategoryId", "Name", null);

            PieEditViewModel pieEditViewModel = new() { Categories = selectListItems, Pie = pieToUpdate };

            return View(pieEditViewModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var selectedCategory = await _pieRepository.GetPieByIdAsync(id);

            return View(selectedCategory);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int? pieId)
        {
            if (pieId == null)
            {
                ViewData["ErrorMessage"] = "Deleting the pie failed, invalid ID!";
                return View();
            }

            try
            {
                await _pieRepository.DeletePieAsync(pieId.Value);
                TempData["PieDeleted"] = "Pie deleted successfully!";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"Deleting the pie failed, please try again! Error: {ex.Message}";
            }

            var selectedPie = await _pieRepository.GetPieByIdAsync(pieId.Value);
            return View(selectedPie);
        }


        public async Task<IActionResult> Search(string? searchQuery, int? searchCategory)
        {
            var allCategories = await _categoryRepository.GetAllCategoriesAsync();

            IEnumerable<SelectListItem> selectListItems = new SelectList(allCategories, "CategoryId", "Name", null);

            if (searchQuery != null)
            {
                var pies = await _pieRepository.SearchPies(searchQuery, searchCategory);

                return View(new PieSearchViewModel()
                {
                    Pies = pies,
                    SearchCategory = searchCategory,
                    Categories = selectListItems,
                    SearchQuery = searchQuery
                });
            }

            return View(new PieSearchViewModel()
            {
                Pies = new List<Pie>(),
                SearchCategory = null,
                Categories = selectListItems,
                SearchQuery = string.Empty
            });
        }
    }
}
