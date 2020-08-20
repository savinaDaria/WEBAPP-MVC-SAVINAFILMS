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
    public class DirectorsController : Controller
    {
        private readonly lab_films_picContext _context;

        public DirectorsController(lab_films_picContext context)
        {
            _context = context;
        }

        // GET: Directors
        public async Task<IActionResult> Index(int? d_id, int? compId, int c, string text1)
        {
            if (text1 != null)
            {
                FormattableString query = $"SELECT * FROM Director WHERE Director.director_id IN (SELECT D.director_id FROM Director AS D INNER JOIN Film ON D.director_id = Film.director_id GROUP BY D.director_id HAVING COUNT(Film.film_id) = (SELECT COUNT(Film.film_id) FROM Film INNER JOIN Director ON Director.director_id = Film.director_id WHERE Director.[name]={@text1}));";
                var ci = _context.Director.FromSqlInterpolated(query);
                var q = await ci.ToListAsync();
                List<Director> dir1 = new List<Director>();
                foreach (var k in q)
                {
                    var dir = _context.Director.Where(f => f.DirectorId== k.DirectorId).Include(f => f.Company).FirstOrDefault();
                    dir1.Add(dir);
                }
                return View(dir1);
            }
            if (c!=0)
            {
                FormattableString query = $"SELECT * FROM Director WHERE Director.director_id NOT IN(SELECT Film.director_id FROM Film); ";
                var ci = _context.Director.FromSqlInterpolated(query);
                var q = await ci.ToListAsync();
                return View(q);
            }
            if (d_id == null && compId==null)
            {
                var lab_films_picContext = _context.Director.Include(d => d.Company);
                return View(await lab_films_picContext.ToListAsync());
            }
            if(d_id != null && compId == null)
            {
                var director = _context.Director.Where(d=>d.DirectorId==d_id).Include(d => d.Company);
                return View(await director.ToListAsync());
            }
            else
            {
                var director = _context.Director.Where(d => d.CompanyId == compId).Include(d => d.Company);
                return View(await director.ToListAsync());
            }
            
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var director = await _context.Director
                .Include(d => d.Company)
                .FirstOrDefaultAsync(m => m.DirectorId == id);
            if (director == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Directors", new { d_id=id});
        }
        // GET: Directors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var director = await _context.Director
                .Include(d => d.Company)
                .FirstOrDefaultAsync(m => m.DirectorId == id);
            if (director == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index","Films",new { dirId = id});
        }

        public IActionResult Back(int? f_id)
        {
            return RedirectToAction("Create", "Films", new { f_id });
        }
        // GET: Directors/Create
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Company, "CompanyId", "Name");
            return View();
        }

        public IActionResult create_company(int? d_id)
        {
            return RedirectToAction("Create", "Companies", new { d_id });
        }

        // POST: Directors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? f_id,[Bind("DirectorId,Name,Birth,Death,Sex,CompanyId")] Director director)
        {
           
            if (ModelState.IsValid)
            {
                _context.Add(director);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "Films", new { f_id });
            }
            ViewData["CompanyId"] = new SelectList(_context.Company, "CompanyId", "Name", director.CompanyId);
            return View(director);
        }

        // GET: Directors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var director = await _context.Director.FindAsync(id);
            if (director == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Company, "CompanyId", "Name", director.CompanyId);
            return View(director);
        }

        // POST: Directors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DirectorId,Name,Birth,Death,Sex,CompanyId")] Director director)
        {
            if (id != director.DirectorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(director);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DirectorExists(director.DirectorId))
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
            ViewData["CompanyId"] = new SelectList(_context.Company, "CompanyId", "Name", director.CompanyId);
            return View(director);
        }

        // GET: Directors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var director = await _context.Director
                .Include(d => d.Company)
                .FirstOrDefaultAsync(m => m.DirectorId == id);
            if (director == null)
            {
                return NotFound();
            }

            return View(director);
        }

        // POST: Directors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var film = _context.Film.Where(f => f.DirectorId == id)
                      .Include(f => f.Country)
                      .Include(f => f.Picture).Select(f=>f.FilmId).ToList();
            for(int i=0;i<film.Count;i++)
            {
                var filmGenre = _context.FilmGenre.Where(f => f.FilmId == film[i])
                      .Include(f => f.Film)
                      .Include(f => f.Genre).ToList();
                _context.FilmGenre.RemoveRange(filmGenre);
                await _context.SaveChangesAsync();
            }

            for (int j = 0; j < film.Count; j++)
            {
                var filmArtist = _context.FilmArtist.Where(f => f.FilmId == film[j])
                      .Include(f => f.Film)
                      .Include(f => f.Artist).ToList();
                _context.FilmArtist.RemoveRange(filmArtist);
                await _context.SaveChangesAsync();
            }
            var film1 =  _context.Film.Where(f => f.DirectorId == id)
                      .Include(f => f.Country)
                      .Include(f => f.Picture).ToList();
            _context.Film.RemoveRange(film1);
            await _context.SaveChangesAsync();

            var director = await _context.Director.FindAsync(id);
            _context.Director.Remove(director);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DirectorExists(int id)
        {
            return _context.Director.Any(e => e.DirectorId == id);
        }
    }
}
