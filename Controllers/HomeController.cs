using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Films()
        {
            return RedirectToAction("Index", "Films");
        }

        public IActionResult Genres()
        {
            return RedirectToAction("Index", "Genres");
        }

        public IActionResult Countries()
        {
            return RedirectToAction("Index", "Countries");
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
