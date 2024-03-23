using System.Diagnostics;
using ApplicationDbContextNamespace.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreApp.Models;
using PublicationNamespace.Models;
using Tesis;

namespace NetCoreApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly ApplicationDbContext _context; // Replace with your DbContext class name

    public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int? pageNumber)
    {
        var publications = _context.Publications
                                    .Where(p => !p.IsBlocked)
                                    .Include(p => p.Author)
                                    .OrderByDescending(p => p.CreationDate);

        int pageSize = 3;
        return View(await PaginatedList<Publication>.CreateAsync(publications.AsNoTracking(), pageNumber ?? 1, pageSize));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
