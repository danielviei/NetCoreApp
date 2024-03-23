using AdminNamespace.Models;
using ApplicationDbContextNamespace.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PublicationNamespace.Models;
using UserNamespace.Models;

namespace AdminNamespace.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<CustomUser> _userManager;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AdminController(ApplicationDbContext context, UserManager<CustomUser> userManager, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }

        [Authorize(Policy = "IsStaff")]
        public ActionResult Index()
        {
            var userCount = _context.Users.Count();
            var publicationCount = _context.Publications.Count();
            var commentCount = _context.Comments.Count();
            int? authorId = null;
            int? maxPublicationCountUser = null;
            CustomUser? userWithMostPublications = null;
            int? maxCommentCountUser = null;
            CustomUser? userWithMostComments = null;

            var groupWithMostPublications = _context.Publications
                .GroupBy(p => p.AuthorId)
                .Select(g => new { AuthorId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .FirstOrDefault();

            if (groupWithMostPublications != null)
            {
                authorId = groupWithMostPublications.AuthorId;
                maxPublicationCountUser = groupWithMostPublications.Count;

                userWithMostPublications = _context.Users.Find(authorId);
            }

            var groupWithMostComments = _context.Comments
                .GroupBy(p => p.AuthorId)
                .Select(g => new { AuthorId = g.Key, Count = g.Count() })
                .OrderByDescending(gp => gp.Count)
                .FirstOrDefault();

            if (groupWithMostComments != null)
            {
                authorId = groupWithMostComments.AuthorId;
                maxCommentCountUser = groupWithMostComments.Count;

                userWithMostComments = _context.Users.Find(authorId);
            }

            var users = _context.Users.ToList();
            var publications = _context.Publications.ToList();
            var commentCounts = _context.Comments
                .GroupBy(c => c.PublicationId)
                .Select(g => new { PublicationId = g.Key, CommentCount = g.Count() })
                .ToList();

            IEnumerable<PublicationWithCommentCount> publicationsWithCommentCount =
                from p in publications
                join c in commentCounts on p.Id equals c.PublicationId into gj
                from subComment in gj.DefaultIfEmpty()
                select new PublicationWithCommentCount { Publication = p, CommentCount = subComment?.CommentCount ?? 0 };

            return View(new AdminViewModel
            {
                UserCount = userCount,
                PublicationCount = publicationCount,
                CommentCount = commentCount,
                UserWithMostPublications = userWithMostPublications,
                UserWithMostComments = userWithMostComments,
                Users = users,
                Publications = publicationsWithCommentCount,
                MaxCommentCountUser = maxCommentCountUser,
                MaxPublicationCountUser = maxPublicationCountUser
            });
        }


        [HttpGet]
        public async Task<IActionResult> EditUsers(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound();
            }

            EditUserModel userViewModel = new EditUserModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Lastname = user.UserName,
                ImagePath = user.ImagePath,
                IsStaff = user.IsStaff,
                IsBlocked = user.IsBlocked,
            };

            return View(userViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsers(int id, EditUserModel model)
        {
            CustomUser? user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.ErrorMessage);
                        Console.WriteLine(error.ErrorMessage);
                    }
                }

                return View(model);
            }

            // Actualiza las propiedades del usuario con los valores del modelo
            Console.WriteLine($"model.Img: {model.Image}");

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
                var filePath = Path.Combine(imagesFolder, fileName);

                // Save the file
                using (var stream = System.IO.File.Create(filePath))
                {
                    await model.Image.CopyToAsync(stream);
                }

                // Save the relative path to the file in the database
                string ImagePath = "/images/" + fileName;

                user.ImagePath = ImagePath;
            }

            user.Name = model.Name;
            user.Email = model.Email;
            user.Lastname = model.Lastname;
            user.IsStaff = model.IsStaff;
            user.IsBlocked = model.IsBlocked;

            await _userManager.UpdateAsync(user);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditPublication(int id)
        {
            var publication = await _context.Publications.FindAsync(id);

            if (publication == null)
            {
                return NotFound();
            }

            EditPublicationViewModel publicationViewModel = new EditPublicationViewModel
            {
                Id = publication.Id,
                Title = publication.Title,
                ImagePath = publication.ImagePath,
                Content = publication.Content,
                AuthorId = publication.AuthorId,
                IsBlocked = publication.IsBlocked,
            };

            return View(publicationViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditPublication(int id, EditPublicationViewModel model)
        {
            Publication? publication = await _context.Publications.FindAsync(id);
            if (publication == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.ErrorMessage);
                        Console.WriteLine(error.ErrorMessage);
                    }
                }

                return View(model);
            }

            // Actualiza las propiedades de la publicación con los valores del modelo
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
                var filePath = Path.Combine(imagesFolder, fileName);

                // Save the file
                using (var stream = System.IO.File.Create(filePath))
                {
                    await model.Image.CopyToAsync(stream);
                }

                // Save the relative path to the file in the database
                string ImagePath = "/images/" + fileName;

                publication.ImagePath = ImagePath;
            }

            publication.Title = model.Title;
            await _context.SaveChangesAsync();

            publication.Content = model.Content;
            await _context.SaveChangesAsync();

            publication.IsBlocked = model.IsBlocked;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeletePublication(int id)
        {
            var publication = await _context.Publications.FindAsync(id);
            if (publication == null)
            {
                return NotFound();
            }

            _context.Publications.Remove(publication);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}