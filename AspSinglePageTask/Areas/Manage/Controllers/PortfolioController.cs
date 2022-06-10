using AspSinglePageTask.DAL;
using AspSinglePageTask.Models;
using AspSinglePageTask.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspSinglePageTask.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class PortfolioController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _ev;

        public PortfolioController(AppDbContext context, IWebHostEnvironment ev)
        {
            _context = context;
            _ev = ev;
        }
        public IActionResult Index()
        {
            List<Portfolio> portfolios = _context.Portfolios.ToList();      
            return View(portfolios);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        
        public async  Task<IActionResult> Create(Portfolio portfolio)
        {
            if (portfolio.Photo.CheckSize(700) || !portfolio.Photo.CheckType("image/"))
            {
                return RedirectToAction(nameof(Index));
            }
            portfolio.Image = await portfolio.Photo.SaveFileAsync(Path.Combine(_ev.WebRootPath,"assets","imgs","port"));
            await _context.Portfolios.AddAsync(portfolio);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int id)
        {
            Slider slider = _context.Sliders.FirstOrDefault(x => x.Id == id);
            if (slider == null) return NotFound();
            return View(slider);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Portfolio portfolio)
        {

            Portfolio export = _context.Portfolios.FirstOrDefault(x => x.Id == portfolio.Id);
            if (export == null) return NotFound();
            string newFileName = null;


            if (portfolio.Photo != null)
            {
                if (portfolio.Photo.ContentType != "image/jpeg" && portfolio.Photo.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "Image can be only .jpeg or .png");
                    return View();
                }
                if (portfolio.Photo.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "Image size must be lower than 2mb");
                    return View();
                }
                string fileName = portfolio.Photo.FileName;
                if (fileName.Length > 64)
                {
                    fileName = fileName.Substring(fileName.Length - 64, 64);
                }
                newFileName = Guid.NewGuid().ToString() + fileName;

                string path = Path.Combine(_ev.WebRootPath, "assets/imgs/port", newFileName);
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    portfolio.Photo.CopyTo(stream);
                }
            }
            if (newFileName != null)
            {
                string deletePath = Path.Combine(_ev.WebRootPath, "assets/imgs/port", export.Image);

                if (System.IO.File.Exists(deletePath))
                {
                    System.IO.File.Delete(deletePath);
                }

                export.Image = newFileName;
            }


            export.Name = portfolio.Name;
            export.Des = portfolio.Des;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Delete(int id)
        {
            Portfolio portfolio = _context.Portfolios.FirstOrDefault(x => x.Id == id);
            if (portfolio == null) return NotFound();
            _context.Portfolios.Remove(portfolio);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
