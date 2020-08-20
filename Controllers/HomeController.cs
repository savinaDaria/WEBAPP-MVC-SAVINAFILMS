using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SAVINAFILMS.Models;

namespace SAVINAFILMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

       
        public JsonResult CheckDate(DateTime Birth)
        {
            var result = true;
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            int min_year = 1700;
            int var = Birth.Year;
            int var1 = Birth.Month;
            int var2 = Birth.Day;
            if (var > year || var < min_year || (var == year && var1 > month) || (var == year && var1 == month && var2 > day))
                result = false;
           
            return Json(result);
        }

        public JsonResult CheckDateDirector(DateTime date)
        {
            var result = true;
            int year = DateTime.Now.Year - 18;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            int min_year = 1700;
            int var = date.Year;
            int var1 = date.Month;
            int var2 = date.Day;
            if (var > year || var < min_year || (var == year && var1 > month) || (var == year && var1 == month && var2 > day)) result = false;
            return Json(result);
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Panel()
        {
            return View();
        }
        

        public IActionResult SQLQueries()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SQLQueries1(int year1, int year2)
        {
            SQLQueries a = new SQLQueries();
            a.year1 = year1;
            a.year2 = year2;
            return RedirectToAction("Index", "Countries", new {year1,year2 });
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SQLQueries2(string char1, string text1)
        {
            SQLQueries a = new SQLQueries();
            a.char1 = char1;
            a.text1 = text1;
            return RedirectToAction("Index", "Artists", new { char1, text1 });

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SQLQueries3(int year1)
        {
            SQLQueries a = new SQLQueries();
            a.year1 = year1;
            return RedirectToAction("Index", "Artists", new { year1 });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SQLQueries4()
        {
            return RedirectToAction("Index", "Directors", new { c=1});

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SQLQueries5(string text1)
        {
            SQLQueries a = new SQLQueries();
            a.text1 = text1;
            return RedirectToAction("Index", "Films", new { text1 });

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SQLQueries6(string text1)
        {
            SQLQueries a = new SQLQueries();
            a.text1 = text1;
            return RedirectToAction("Index", "Directors", new { text1 });

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SQLQueries7(string char1, int year1)
        {
            return RedirectToAction("Index", "Films", new { char1, year1 });

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SQLQueries8(int year2)
        {
            return RedirectToAction("Index", "Artists", new { year2 });

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SQLQueries9(string text1)
        {
            SQLQueries a = new SQLQueries();
            a.text1 = text1;
            return RedirectToAction("Index", "Films", new { text1 });

        }
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Films()
        {
            return RedirectToAction("Index", "Films");
        }
        public IActionResult Users()
        {
            return RedirectToAction("Index", "Users");
        }
        public IActionResult Genres()
        {
            return RedirectToAction("Index", "Genres");
        }

        public IActionResult Roles()
        {
            return RedirectToAction("Index", "Roles");
        }
        public IActionResult FilmGenres()
        {
            return RedirectToAction("Index", "FilmGenres", new { id=0,  f_id=0});
        }
        public IActionResult Countries()
        {
            return RedirectToAction("Index", "Countries");

        }
        public IActionResult FilmArtists()
        {
            return RedirectToAction("Index", "FilmArtists", new { id=0});
        }

        public IActionResult Artists()
        {
            return RedirectToAction("Index", "Artists");
        }

        public IActionResult Directors()
        {
            return RedirectToAction("Index", "Directors");
        }
        public IActionResult Companies()
        {
            return RedirectToAction("Index", "Companies");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
