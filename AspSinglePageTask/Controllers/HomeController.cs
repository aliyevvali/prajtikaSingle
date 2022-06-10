using AspSinglePageTask.DAL;
using AspSinglePageTask.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AspSinglePageTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                sliders = _context.Sliders.ToList(),
                portfolios = _context.Portfolios.ToList(),
                about = _context.Abouts.ToList()
            };
            return View(homeVM);
        }

        
    }
}
