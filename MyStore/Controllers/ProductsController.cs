using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyStore.Data;
using MyStore.Filters;
using MyStore.Model;

namespace MyStore.Controllers
{
    /// <summary>
    /// The ProductsController handles CRUD operations for managing products in the application
    /// </summary>
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;// Injecting ApplicationDbContext for data operations
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
          .Include(p => p.CategoryMaster)
          .ToListAsync();
            return View(products);
        }

        // Admin-only action to create a new product
        [AllowedRoles("Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryMasterId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [AllowedRoles("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //For Checking the validation errors
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                
                Console.WriteLine(error.ErrorMessage);
            }
            ViewData["CategoryMasterId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryMasterId);
            return View(product);
        }


        [AllowedRoles("Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            // Populating the category dropdown list with the selected category
            ViewData["CategoryMasterId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryMasterId);
            return View(product);
        }

        [AllowedRoles("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,CategoryMasterId,Price")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;// Re-throwing the exception if the concurrency issue is not resolved
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            // Repopulating the category dropdown list if validation fails
            ViewData["CategoryMasterId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryMasterId);
            return View(product);
        }

        [AllowedRoles("Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                .Include(p => p.CategoryMaster)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [AllowedRoles("Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Helper method to check if a product exists
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}

