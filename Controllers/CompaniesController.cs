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
    public class CompaniesController : Controller
    {
        private readonly lab_films_picContext _context;

        public CompaniesController(lab_films_picContext context)
        {
            _context = context;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Company.ToListAsync());
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .FirstOrDefaultAsync(m => m.CompanyId == id);
            if (company == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Directors", new { compId = id });
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Back(int? d_id)
        {
            return RedirectToAction("Create", "Directors", new { d_id });
        }

        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? d_id,[Bind("CompanyId,Name,Year")] Company company)
        {
            if (ModelState.IsValid)
            {
                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "Directors", new { d_id });
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Company.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CompanyId,Name,Year")] Company company)
        {
            if (id != company.CompanyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.CompanyId))
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
            return View(company);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .FirstOrDefaultAsync(m => m.CompanyId == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dir = _context.Director.Where(d => d.CompanyId == id).Include(d => d.Company).ToList();
            foreach(var d in dir)
            {
                var film = _context.Film.Where(f => f.DirectorId == d.DirectorId).Include(f => f.Director).Include(f => f.Country).Include(f => f.Picture).ToList();
                foreach(var c in film)
                {
                    var FilmArt = _context.FilmArtist.Where(f => f.FilmId == c.FilmId).Include(f => f.Film).Include(f => f.Artist).ToList();
                    _context.FilmArtist.RemoveRange(FilmArt);
                    var FilmGenre = _context.FilmGenre.Where(f => f.FilmId == c.FilmId).Include(f => f.Film).Include(f => f.Genre).ToList();
                    _context.FilmGenre.RemoveRange(FilmGenre);
                    await _context.SaveChangesAsync();
                }
                _context.Film.RemoveRange(film);
            }
            _context.Director.RemoveRange(dir);
            await _context.SaveChangesAsync();
            var company = await _context.Company.FindAsync(id);
            _context.Company.Remove(company);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
            return _context.Company.Any(e => e.CompanyId == id);
        }
    }
}
