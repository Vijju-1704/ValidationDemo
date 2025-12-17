using Microsoft.AspNetCore.Mvc;
using ValidationDemo.DTOs;
using ValidationDemo.Services;

namespace ValidationDemo.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductApiService _apiService;

        public ProductsController(IProductApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _apiService.GetAllProductsAsync();
            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _apiService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            if (ModelState.IsValid)
            {
                var created = await _apiService.CreateProductAsync(dto);
                if (created != null)
                {
                    TempData["Success"] = "Product created successfully!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "Failed to create product";
            }
            return View(dto);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _apiService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var updateDto = new UpdateProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Category = product.Category,
                IsActive = product.IsActive
            };

            return View(updateDto);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateProductDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var updated = await _apiService.UpdateProductAsync(id, dto);
                if (updated != null)
                {
                    TempData["Success"] = "Product updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "Failed to update product";
            }
            return View(dto);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _apiService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _apiService.DeleteProductAsync(id);
            if (deleted)
            {
                TempData["Success"] = "Product deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to delete product";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}