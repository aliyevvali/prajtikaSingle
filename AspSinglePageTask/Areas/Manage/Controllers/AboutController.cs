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
    public class AboutController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _ev;

        public AboutController(AppDbContext context, IWebHostEnvironment ev)
        {
            _context = context;
            _ev = ev;
        }
        public IActionResult Index()
        {
            List<About> about = _context.Abouts.ToList();
            return View(about);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> Create(About about)
        {
            if (about.Photo.CheckSize(700) || !about.Photo.CheckType("image/"))
            {
                return RedirectToAction(nameof(Index));
            }
            about.Image = await about.Photo.SaveFileAsync(Path.Combine(_ev.WebRootPath, "assets", "imgs", "about"));
            await _context.Abouts.AddAsync(about);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int id)
        {
            About about = _context.Abouts.FirstOrDefault(x => x.Id == id);
            if (about == null) return NotFound();
            return View(about);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(About about)
        {

            About exabout = _context.Abouts.FirstOrDefault(x => x.Id == about.Id);
            if (exabout == null) return NotFound();
            string newFileName = null;


            if (about.Photo != null)
            {
                if (about.Photo.ContentType != "image/jpeg" && about.Photo.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "Image can be only .jpeg or .png");
                    return View();
                }
                if (about.Photo.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "Image size must be lower than 2mb");
                    return View();
                }
                string fileName = about.Photo.FileName;
                if (fileName.Length > 64)
                {
                    fileName = fileName.Substring(fileName.Length - 64, 64);
                }
                newFileName = Guid.NewGuid().ToString() + fileName;

                string path = Path.Combine(_ev.WebRootPath, "assets/imgs/about", newFileName);
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    about.Photo.CopyTo(stream);
                }
            }
            if (newFileName != null)
            {
                string deletePath = Path.Combine(_ev.WebRootPath, "assets/imgs/about", exabout.Image);

                if (System.IO.File.Exists(deletePath))
                {
                    System.IO.File.Delete(deletePath);
                }

                exabout.Image = newFileName;
            }


            exabout.QisaAciqlama = about.QisaAciqlama;
            exabout.ShirketAdi = about.ShirketAdi;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Delete(int id)
        {
            About about = _context.Abouts.FirstOrDefault(x => x.Id == id);
            if (about == null) return NotFound();
            _context.Abouts.Remove(about);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
