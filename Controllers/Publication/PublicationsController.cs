using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using PublicationNamespace.Models;
using ApplicationDbContextNamespace.Data;
using UserNamespace.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace PublicationNamespace.Controllers
{
    public class PublicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<CustomUser> _userManager;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public PublicationsController(IWebHostEnvironment hostingEnvironment, UserManager<CustomUser> userManager, ApplicationDbContext context)
        {
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _context = context;
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreatePublicationViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid && currentUser != null)
            {
                // Handle the image upload
                var filePath = Path.GetTempFileName();

                string ImagePath = "";
                if (model.Image != null)
                {
                    // Define a unique name for the file
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);

                    // Define the path to the folder where you want to save the images
                    var imagesFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");

                    // If the folder doesn't exist, create it
                    if (!Directory.Exists(imagesFolder))
                    {
                        Directory.CreateDirectory(imagesFolder);
                    }

                    // Define the full path to the file
                    filePath = Path.Combine(imagesFolder, fileName);

                    // Save the file
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await model.Image.CopyToAsync(stream);
                    }

                    // Save the relative path to the file in the database
                    ImagePath = "/images/" + fileName;
                }

                // Create a new publication
                var publication = new Publication
                {
                    Title = model.Title,
                    ImagePath = ImagePath,
                    Content = model.Content,
                    AuthorId = currentUser.Id,
                };

                // Add and save the new publication to your database
                _context.Publications.Add(publication);
                await _context.SaveChangesAsync();

                // Redirect the user to the list of publications after a successful creation
                return RedirectToAction("Index", "Home");
            }

            // If the model is not valid, return the user to the form
            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publication = await _context.Publications.Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (publication == null)
            {
                return NotFound();
            }
            publication.Comments = await _context.Comments
                .Where(c => c.PublicationId == id)
                .Include(c => c.Author)
                .ToListAsync();

            return View(publication);
        }

        [Authorize]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var publication = await _context.Publications.FindAsync(id);
            if (publication == null)
            {
                return StatusCode(404, new { success = false, message = "Publication not found." });
            }

            _context.Publications.Remove(publication);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Publication deleted successfully." });
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publication = await _context.Publications.FindAsync(id);
            if (publication == null)
            {
                return NotFound();
            }

            var model = new EditPublicationViewModel
            {
                Id = publication.Id,
                Title = publication?.Title ?? string.Empty,
                Content = publication?.Content ?? string.Empty,
                ImagePath = publication?.ImagePath ?? string.Empty,
                Image = publication?.Image,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditPublicationViewModel model)
        {
            var publication = await _context.Publications.FindAsync(id);
            if (publication == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var filePath = Path.GetTempFileName();
                string ImagePath = "";
                if (model.Image != null)
                {
                    // Define a unique name for the file
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);

                    // Define the path to the folder where you want to save the images
                    var imagesFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");

                    // If the folder doesn't exist, create it
                    if (!Directory.Exists(imagesFolder))
                    {
                        Directory.CreateDirectory(imagesFolder);
                    }

                    // Define the full path to the file
                    filePath = Path.Combine(imagesFolder, fileName);

                    // Save the file
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await model.Image.CopyToAsync(stream);
                    }

                    // Save the relative path to the file in the database
                    ImagePath = "/images/" + fileName;
                    publication.ImagePath = ImagePath;
                    publication.Image = model.Image;
                }

                publication.Title = model.Title;
                publication.Content = model.Content;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PublicationExists(publication.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("Details", new { id = publication.Id });
            }
            model.ImagePath = publication.ImagePath;
            model.Content = publication.Content?? string.Empty;
            model.Title = publication.Title ?? string.Empty;
            model.Image = publication.Image;
            return View(model);
        }

        private bool PublicationExists(int id)
        {
            return _context.Publications.Any(e => e.Id == id);
        }
    }
}