
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System.Security.Claims;
using ApplicationDbContextNamespace.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using UserNamespace.Models;
using CommentNamespace.Models;
using PublicationNamespace.Models;

namespace CommentNamespace.Controllers
{
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context; // Replace with your DbContext class name

        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CommentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                int authorId;
                if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out authorId))
                {
                    var comment = new Comment
                    {
                        Content = model.Content,
                        AuthorId = authorId,
                        PublicationId = model.PublicationId,
                        CreationDate = DateTime.Now.ToUniversalTime()
                    };

                    _context.Add(comment);
                    await _context.SaveChangesAsync();

                    // Get all comments for the publication
                    var comments = _context.Comments
                        .Where(c => c.PublicationId == model.PublicationId)
                        .Include(p => p.Author)
                        .OrderByDescending(c => c.CreationDate)
                        .ToList();

                    // Generate the HTML for the comments
                    var commentsHtml = new StringBuilder("<ul class='list-none space-y-4'>");
                    foreach (var c in comments)
                    {
                        commentsHtml.Append($"<li id='comment-{c?.Id}' class='p-4 rounded-md bg-gray-100'>");
                        commentsHtml.Append($"<div class='flex items-center'>");
                        commentsHtml.Append($"<img src='{c?.Author?.ImagePath}' alt='{c?.Author?.Email} profile picture' class='w-10 h-10 rounded-full mr-4'>");
                        commentsHtml.Append("<div class='w-full' >");
                        commentsHtml.Append("<div class='w-full flex justify-between'>");
                        commentsHtml.Append($"<h4 class='text-md font-bold text-gray-800'>{c?.Author?.Email}</h4>");
                        if (c?.Author?.Id == authorId)
                        {
                            commentsHtml.Append("<div class='flex items-center gap-2' >");
                            commentsHtml.Append($"<button class='edit-button' data-comment-id='{c?.Id}'><svg  width='20' height='20' viewBox='0 0 20 20' fill='none' xmlns='http://www.w3.org/2000/svg'><path d='M12.5 5.00001L15 7.50001M10.8334 16.6667H17.5M4.16671 13.3333L3.33337 16.6667L6.66671 15.8333L16.3217 6.17835C16.6342 5.8658 16.8097 5.44195 16.8097 5.00001C16.8097 4.55807 16.6342 4.13423 16.3217 3.82168L16.1784 3.67835C15.8658 3.36589 15.442 3.19037 15 3.19037C14.5581 3.19037 14.1343 3.36589 13.8217 3.67835L4.16671 13.3333Z' stroke='black' stroke-linecap='round' stroke-linejoin='round' ></path></svg> </button>");
                            commentsHtml.Append($"<button class='delete-button' data-comment-id='{c?.Id}'><svg width='26' height='26' viewBox='0 0 26 26'  fill='none' xmlns='http://www.w3.org/2000/svg' > <path d='M8.24954 21.6667C7.76565 21.6667 7.3529 21.4962 7.01129 21.1553C6.6704 20.8144 6.49996 20.4017 6.49996 19.9171V6.49999H5.41663V5.41666H9.74996V4.58249H16.25V5.41666H20.5833V6.49999H19.5V19.9171C19.5 20.4154 19.3331 20.8314 18.9995 21.1651C18.6651 21.4995 18.2487 21.6667 17.7504 21.6667H8.24954ZM18.4166 6.49999H7.58329V19.9171C7.58329 20.1114 7.64576 20.271 7.77071 20.3959C7.89565 20.5209 8.05526 20.5833 8.24954 20.5833H17.7504C17.9165 20.5833 18.0692 20.514 18.2086 20.3753C18.3473 20.2359 18.4166 20.0832 18.4166 19.9171V6.49999ZM10.6253 18.4167H11.7086V8.66666H10.6253V18.4167ZM14.2913 18.4167H15.3746V8.66666H14.2913V18.4167Z' fill='black'></path> </svg></button>");
                            commentsHtml.Append("</div>");
                            commentsHtml.Append($"<div id='delete-confirmation-{c?.Id}' class='delete-confirmation' style='display: none;'>");
                            commentsHtml.Append("<p>¿Estás seguro de que quieres eliminar este comentario?</p>");
                            commentsHtml.Append($"<button class='delete-confirmation-button' data-comment-id='{c?.Id}'><svg>...</svg></button>");
                            commentsHtml.Append("</div>");
                        }
                        commentsHtml.Append("</div>");
                        commentsHtml.Append($"<p id='comment-{c?.Id}-content' class='text-gray-600'>{c?.Content}</p>");
                        commentsHtml.Append("</div></div>");
                        if (c?.Author?.Id == authorId)
                        {
                            commentsHtml.Append($"<form id='edit-form-{c?.Id}' class='edit-form p-2' data-comment-id='{c?.Id}' style='display: none;'>");
                            commentsHtml.Append($"<textarea>{c?.Content}</textarea>");
                            commentsHtml.Append("<button type='submit'>Guardar</button>");
                            commentsHtml.Append("</form>");
                        }
                        commentsHtml.Append("</li>");
                    }
                    commentsHtml.Append("</ul>");
                    return Json(new { success = true, message = "Comment created successfully.", commentsHtml = commentsHtml.ToString() });
                }
            }

            return StatusCode(400, new { success = false, message = "Failed to create comment.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromBody] CommentEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var comment = await _context.Comments.FindAsync(id);
                if (comment == null)
                {
                    return StatusCode(404, new { success = false, message = "Comment not Found.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
                }

                comment.Content = model.Content;
                _context.Update(comment);
                await _context.SaveChangesAsync();

                return Json(new { success = true, content = comment.Content });
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return StatusCode(400, new { success = false, message = "Model is not valid.", errors = errors });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return StatusCode(404, new { success = false, message = "Comment not found." });
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Comment deleted successfully." });
        }
    }
}