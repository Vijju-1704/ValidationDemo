// YourMvcProject/Controllers/ProductsController.cs
using Microsoft.AspNetCore.Mvc;
using MyWebApi.DTOs;
using ValidationDemo.Services;
using ValidationDemo.Services;
using MyWebApi.DTOs;

namespace YourMvcProject.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IApiService ApiService;
        private readonly ILogger<ProductsController> Logger;

        public ProductsController(IApiService apiService, ILogger<ProductsController> logger)
        {
            ApiService = apiService;
            Logger = logger;
        }

        // GET: Products
        // GET: Products
        public async Task<IActionResult> Index(string? category, bool? isActive)
        {
            List<string> categories = new();

            try
            {
                categories = await ApiService.GetCategoriesAsync() ?? new List<string>();
                var products = await ApiService.GetProductsAsync(category, isActive) ?? new List<ProductDto>();

                ViewBag.Categories = categories;
                ViewBag.SelectedCategory = category;
                ViewBag.IsActive = isActive;

                return View(products);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading products");

                ViewBag.Categories = categories;
                ViewBag.SelectedCategory = category;
                ViewBag.IsActive = isActive;

                TempData["ErrorMessage"] = "An error occurred while loading products";
                return View(new List<ProductDto>());
            }
        }


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var product = await ApiService.GetProductByIdAsync(id);

                if (product == null)
                {
                    TempData["ErrorMessage"] = $"Product with ID {id} not found";
                    return RedirectToAction(nameof(Index));
                }

                return View(product);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading product details for ID {ProductId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading product details";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await ApiService.GetCategoriesAsync() ?? new List<string>();
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductDto productDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = await ApiService.GetCategoriesAsync() ?? new List<string>();
                    return View(productDto);
                }

                var createdProduct = await ApiService.CreateProductAsync(productDto);

                if (createdProduct != null)
                {
                    TempData["SuccessMessage"] = "Product created successfully";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Failed to create product");
                ViewBag.Categories = await ApiService.GetCategoriesAsync() ?? new List<string>();
                return View(productDto);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating product");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the product");
                ViewBag.Categories = await ApiService.GetCategoriesAsync() ?? new List<string>();
                return View(productDto);
            }
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var product = await ApiService.GetProductByIdAsync(id);

                if (product == null)
                {
                    TempData["ErrorMessage"] = $"Product with ID {id} not found";
                    return RedirectToAction(nameof(Index));
                }

                var updateDto = new UpdateProductDto
                {
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Category = product.Category,
                    StockQuantity = product.StockQuantity,
                    IsActive = product.IsActive
                };

                ViewBag.Categories = await ApiService.GetCategoriesAsync() ?? new List<string>();
                ViewBag.ProductId = id;
                return View(updateDto);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading product for edit, ID {ProductId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading the product";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateProductDto productDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = await ApiService.GetCategoriesAsync() ?? new List<string>();
                    ViewBag.ProductId = id;
                    return View(productDto);
                }

                var success = await ApiService.UpdateProductAsync(id, productDto);

                if (success)
                {
                    TempData["SuccessMessage"] = "Product updated successfully";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Failed to update product");
                ViewBag.Categories = await ApiService.GetCategoriesAsync() ?? new List<string>();
                ViewBag.ProductId = id;
                return View(productDto);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error updating product ID {ProductId}", id);
                ModelState.AddModelError(string.Empty, "An error occurred while updating the product");
                ViewBag.Categories = await ApiService.GetCategoriesAsync() ?? new List<string>();
                ViewBag.ProductId = id;
                return View(productDto);
            }
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await ApiService.GetProductByIdAsync(id);

                if (product == null)
                {
                    TempData["ErrorMessage"] = $"Product with ID {id} not found";
                    return RedirectToAction(nameof(Index));
                }

                return View(product);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading product for deletion, ID {ProductId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading the product";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var success = await ApiService.DeleteProductAsync(id);

                if (success)
                {
                    TempData["SuccessMessage"] = "Product deleted successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete product";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting product ID {ProductId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the product";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}