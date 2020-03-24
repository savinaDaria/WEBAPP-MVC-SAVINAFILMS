using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SAVINAFILMS;

namespace SAVINAFILMS.Controllers
{
    public class CountriesController : Controller
    {
        private readonly lab_films_picContext _context;

        public CountriesController(lab_films_picContext context)
        {
            _context = context;
        }

        // GET: Countries
        public async Task<IActionResult> Index(int? c_id)
        {
            return View(await _context.Country.ToListAsync());
        }

        public async Task<IActionResult> Detail_c(int? c_id)
        {
            if (c_id == null)
            {
                return NotFound();
            }
            var c_films = _context.Film.Where(d => d.CountryId == c_id).Include(f => f.Country).Include(f => f.Director).Include(f => f.Picture).ToList();
            if (c_films.Count() == 0)
            {
                return RedirectToAction("Create", "Films", new { c_id });
            }
            
            return RedirectToAction("Index", "Films", new { c_id });
        }
        // GET: Countries/Details/5
        public async Task<IActionResult> Details(int? c_id)
        {
            if (c_id == null)
            {
                return NotFound();
            }

            var country = await _context.Country
                .FirstOrDefaultAsync(m => m.CountryId == c_id);
            if (country == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Films", new { c_id });
        }


        // GET: Countries/Create
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Diagram()
        {
            return View();
        }


        public IActionResult Back(int? f_id, int? a_id)
        {
            if(f_id!=null)
            {
                return RedirectToAction("Create","Films", new { f_id});
             }
            else
                return RedirectToAction("Create", "Artists", new { a_id });
        }

        // POST: Countries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? f_id,int? a_id,[Bind("CountryId,Name")] Country country)
        {
            if (ModelState.IsValid && f_id!=null)
            {
                _context.Add(country);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create","Films", new { f_id});
            }
            if (ModelState.IsValid && a_id != null)
            {
                _context.Add(country);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "Artists", new { a_id });
            }
            return View(country);
        }


        // GET: Countries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Country.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        // POST: Countries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CountryId,Name")] Country country)
        {
            if (id != country.CountryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryExists(country.CountryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        // GET: Countries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Country
                .FirstOrDefaultAsync(m => m.CountryId == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var country = await _context.Country.FindAsync(id);
            _context.Country.Remove(country);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CountryExists(int id)
        {
            return _context.Country.Any(e => e.CountryId == id);
        }
    }
}
