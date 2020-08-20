using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace SAVINAFILMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private readonly lab_films_picContext _context;
        public ChartsController(lab_films_picContext context)
        {
            _context = context;
        }

        [HttpGet("JsonData")]
        public JsonResult JsonData()
        {
            var countries = _context.Country.Include(f => f.Film).ToList();
            List<object> cFilms = new List<object>();
            cFilms.Add(new[] { "Країна", "Кількість фільмів" });
            foreach(var c in countries)
            {
                cFilms.Add(new object[] { c.Name,c.Film.Count()});
            }
            return new JsonResult(cFilms);
        }

        [HttpGet("JsonData")]
        public JsonResult JsonData1()
        {var directors = _context.Director.Include(f => f.Film).ToList();
            List<object> dFilms = new List<object>();
            dFilms.Add(new[] { "Країна", "Кількість фільмів" });
            foreach (var d in  directors)
            {
                dFilms.Add(new object[] { d.Name, d.Film.Count() });
            }
            return new JsonResult(dFilms);
        }


    }

}