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
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _ev;

        public SliderController(AppDbContext context, IWebHostEnvironment ev)
        {
            _context = context;
            _ev = ev;
        }
        public IActionResult Index()
        {
            List<Slider> sliders = _context.Sliders.ToList();
            return View(sliders);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Slider slider)
        {
            if (slider.Photo.CheckSize(2000) || !slider.Photo.CheckType("image/"))
            {
                return RedirectToAction(nameof(Index));
            }
            slider.Image = await slider.Photo.SaveFileAsync(Path.Combine(_ev.WebRootPath, "assets", "imgs", "slider"));
            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int id)
        {
            Slider slider = _context.Sliders.FirstOrDefault(x => x.Id == id);
            if (slider == null) return NotFound();
            return View(slider);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Slider slider)
        {

            Slider exSlider = _context.Sliders.FirstOrDefault(x => x.Id == slider.Id);
            if (exSlider == null) return NotFound();
            string newFileName = null;


            if (slider.Photo != null)
            {
                if (slider.Photo.ContentType != "image/jpeg" && slider.Photo.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "Image can be only .jpeg or .png");
                    return View();
                }
                if (slider.Photo.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "Image size must be lower than 2mb");
                    return View();
                }
                string fileName = slider.Photo.FileName;
                if (fileName.Length > 64)
                {
                    fileName = fileName.Substring(fileName.Length - 64, 64);
                }
                newFileName = Guid.NewGuid().ToString() + fileName;

                string path = Path.Combine(_ev.WebRootPath, "assets/imgs/slider", newFileName);
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    slider.Photo.CopyTo(stream);
                }
            }
            if (newFileName != null)
            {
                string deletePath = Path.Combine(_ev.WebRootPath, "assets/imgs/slider", exSlider.Image);

                if (System.IO.File.Exists(deletePath))
                {
                    System.IO.File.Delete(deletePath);
                }

                exSlider.Image = newFileName;
            }


            exSlider.Name = slider.Name;
            exSlider.Title = slider.Title;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Delete(int id)
        {
            Slider slider = _context.Sliders.FirstOrDefault(x => x.Id == id);
            if (slider == null) return NotFound();
            _context.Sliders.Remove(slider);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
