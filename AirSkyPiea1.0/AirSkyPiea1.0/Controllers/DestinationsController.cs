using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AirSkyPiea1._0.Data;
using AirSkyPiea1._0.Data.Entities;
using AirSkyPiea1._0.Helpers;
using AirSkyPiea1._0.Models;
using Vereyon.Web;
using static AirSkyPiea1._0.Helpers.ModalHelper;

namespace AirSkyPiea1._0.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DestinationsController : Controller
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IFlashMessage _flashMessage;

        public DestinationsController(DataContext context, ICombosHelper combosHelper, IBlobHelper blobHelper, IFlashMessage flashMessage)
        {
            _context = context;
            _combosHelper = combosHelper;
            _blobHelper = blobHelper;
            _flashMessage = flashMessage;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Destinations
                .Include(p => p.DestinationImages)
                .Include(p => p.DestinationCategories)
                .ThenInclude(pc => pc.Category)
                .ToListAsync());
        }

        [NoDirectAccess]
        public async Task<IActionResult> Create()
        {
            CreateDestinationViewModel model = new()
            {
                Categories = await _combosHelper.GetComboCategoriesAsync(),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDestinationViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;
                if (model.ImageFile != null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "products");
                }

                Destination destination = new()
                {
                    Description = model.Description,
                    Name = model.Name,
                    Price = model.Price,
                    Stock = model.Stock,
                };

                destination.DestinationCategories = new List<DestinationCategory>()
                {
                    new DestinationCategory
                    {
                        Category = await _context.Categories.FindAsync(model.CategoryId)
                    }
                };

                if (imageId != Guid.Empty)
                {
                    destination.DestinationImages = new List<DestinationImage>()
                    {
                        new DestinationImage { ImageId = imageId }
                    };
                }

                try
                {
                    _context.Add(destination);
                    await _context.SaveChangesAsync();
                    _flashMessage.Confirmation("Registro creado.");
                    return Json(new
                    {
                        isValid = true,
                        html = ModalHelper.RenderRazorViewToString(this, "_ViewAllProducts", _context.Destinations
                        .Include(p => p.DestinationImages)
                        .Include(p => p.DestinationCategories)
                        .ThenInclude(pc => pc.Category).ToList())
                    });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un producto con el mismo nombre.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            model.Categories = await _combosHelper.GetComboCategoriesAsync();
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "Create", model) });
        }

        [NoDirectAccess]
        public async Task<IActionResult> Edit(int id)
        {
            Destination destination = await _context.Destinations.FindAsync(id);
            if (destination == null)
            {
                return NotFound();
            }

            EditDestinationViewModel model = new()
            {
                Description = destination.Description,
                Id = destination.Id,
                Name = destination.Name,
                Price = destination.Price,
                Stock = destination.Stock,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateDestinationViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            try
            {
                Destination destination = await _context.Destinations.FindAsync(model.Id);
                destination.Description = model.Description;
                destination.Name = model.Name;
                destination.Price = model.Price;
                destination.Stock = model.Stock;
                _context.Update(destination);
                await _context.SaveChangesAsync();
                _flashMessage.Confirmation("Registro actualizado.");
                return Json(new
                {
                    isValid = true,
                    html = ModalHelper.RenderRazorViewToString(this, "_ViewAllProducts", _context.Destinations
                    .Include(p => p.DestinationImages)
                    .Include(p => p.DestinationCategories)
                    .ThenInclude(pc => pc.Category).ToList())
                });
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                {
                    ModelState.AddModelError(string.Empty, "Ya existe un destino con ese nombre.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                }
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "Edit", model) });
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Destination destination = await _context.Destinations
                .Include(p => p.DestinationImages)
                .Include(p => p.DestinationCategories)
                .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (destination == null)
            {
                return NotFound();
            }

            return View(destination);
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Destination product = await _context.Destinations.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            AddDestinationImageViewModel model = new()
            {
                DestinationId = product.Id,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImage(AddDestinationImageViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "destinos");
                Destination destination = await _context.Destinations.FindAsync(model.DestinationId);
                DestinationImage destinationImage = new()
                {
                    Destination = destination,
                    ImageId = imageId,
                };

                try
                {
                    _context.Add(destinationImage);
                    await _context.SaveChangesAsync();
                    _flashMessage.Confirmation("Imagen agregada.");
                    return Json(new
                    {
                        isValid = true,
                        html = ModalHelper.RenderRazorViewToString(this, "Details", _context.Destinations
                            .Include(p => p.DestinationImages)
                            .Include(p => p.DestinationCategories)
                            .ThenInclude(pc => pc.Category)
                            .FirstOrDefaultAsync(p => p.Id == model.DestinationId))
                    });
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                }
            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddImage", model) });
        }

        public async Task<IActionResult> DeleteImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DestinationImage destinationImage = await _context.DestinationImages
                .Include(pi => pi.Destination)
                .FirstOrDefaultAsync(pi => pi.Id == id);
            if (destinationImage == null)
            {
                return NotFound();
            }

            await _blobHelper.DeleteBlobAsync(destinationImage.ImageId, "destinos");
            _context.DestinationImages.Remove(destinationImage);
            await _context.SaveChangesAsync();
            _flashMessage.Info("Registro borrado.");
            return RedirectToAction(nameof(Details), new { id = destinationImage.Destination.Id });
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddCategory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Destination destination = await _context.Destinations
                .Include(p => p.DestinationCategories)
                .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (destination == null)
            {
                return NotFound();
            }

            List<Category> categories = destination.DestinationCategories.Select(pc => new Category
            {
                Id = pc.Category.Id,
                Name = pc.Category.Name,
            }).ToList();

            AddCategoryDestinationViewModel model = new()
            {
                DestinationId = destination.Id,
                Categories = await _combosHelper.GetComboCategoriesAsync(categories),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(AddCategoryDestinationViewModel model)
        {
            Destination destination = await _context.Destinations
                .Include(p => p.DestinationCategories)
                .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.Id == model.DestinationId);

            if (ModelState.IsValid)
            {
                DestinationCategory destinationCategory = new()
                {
                    Category = await _context.Categories.FindAsync(model.CategoryId),
                    Destination= destination,
                };

                try
                {
                    _context.Add(destinationCategory);
                    await _context.SaveChangesAsync();
                    _flashMessage.Confirmation("Categoría agregada.");
                    return Json(new
                    {
                        isValid = true,
                        html = ModalHelper.RenderRazorViewToString(this, "Details", _context.Destinations
                            .Include(p => p.DestinationImages)
                            .Include(p => p.DestinationCategories)
                            .ThenInclude(pc => pc.Category)
                            .FirstOrDefaultAsync(p => p.Id == model.DestinationId))
                    });
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                }
            }

            List<Category> categories = destination.DestinationCategories.Select(pc => new Category
            {
                Id = pc.Category.Id,
                Name = pc.Category.Name,
            }).ToList();

            model.Categories = await _combosHelper.GetComboCategoriesAsync(categories);
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddCategory", model) });
        }

        public async Task<IActionResult> DeleteCategory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DestinationCategory destinationCategory = await _context.DestinationCategories
                .Include(pc => pc.Destination)
                .FirstOrDefaultAsync(pc => pc.Id == id);
            if (destinationCategory == null)
            {
                return NotFound();
            }

            _context.DestinationCategories.Remove(destinationCategory);
            await _context.SaveChangesAsync();
            _flashMessage.Info("Registro borrado.");
            return RedirectToAction(nameof(Details), new { Id = destinationCategory.Destination.Id });
        }

        [NoDirectAccess]
        public async Task<IActionResult> Delete(int id)
        {
            Destination destination = await _context.Destinations
                .Include(p => p.DestinationCategories)
                .Include(p => p.DestinationImages)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (destination == null)
            {
                return NotFound();
            }

            foreach (DestinationImage destinationImage in destination.DestinationImages)
            {
                await _blobHelper.DeleteBlobAsync(destinationImage.ImageId, "Destinos");
            }

            _context.Destinations.Remove(destination);
            await _context.SaveChangesAsync();
            _flashMessage.Info("Registro borrado.");
            return RedirectToAction(nameof(Index));
        }
    }
}
