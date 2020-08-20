using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace SAVINAFILMS.Controllers
{
    public class FilmsController : Controller
    {
        private readonly lab_films_picContext _context;

        public FilmsController(lab_films_picContext context)
        {
            _context = context;
        }

        // GET: Films
        public async Task<IActionResult> Index(int? id, int? dirId, int? c_id, string text1, string char1, int year1)
        {
            if (char1 != null)
            {
                FormattableString query = $"SELECT * FROM Film WHERE Film.release ={year1} AND Film.film_id IN(SELECT Film.film_id FROM Film INNER JOIN Director ON Director.director_id = Film.director_id WHERE Director.sex= {@char1}); ";
                var c = _context.Film.FromSqlInterpolated(query);
                var q = await c.ToListAsync();
                List<Film> film1= new List<Film>();
                foreach(var k in q)
                {
                    var films = _context.Film.Where(f => f.FilmId == k.FilmId).Include(f => f.Country).Include(f => f.Director).FirstOrDefault();
                    film1.Add(films);
                }
                return View(film1);
            }
            if (text1 != null)
            {
                FormattableString query = $"SELECT * FROM Film WHERE Film.film_id in(SELECT FilmArtist.film_id FROM FilmArtist INNER JOIN Artist ON FilmArtist.artist_id = Artist.artist_id WHERE Artist.[name]={@text1});"; 
                var c = _context.Film.FromSqlInterpolated(query);
                var q = await c.ToListAsync();
                List<Film> film1 = new List<Film>();
                foreach (var k in q)
                {
                    var films = _context.Film.Where(f => f.FilmId == k.FilmId).Include(f => f.Country).Include(f => f.Director).FirstOrDefault();
                    film1.Add(films);
                }
                return View(film1);
            }
            if (id == null) 
            {
                if (dirId != null)
                {
                    ViewBag.DirectorId = dirId;
                    var dir_films = _context.Film.Where(d => d.DirectorId == dirId).Include(f => f.Country).Include(f => f.Director).Include(f => f.Picture);
                    return View(await dir_films.ToListAsync());
                }
                if (c_id != null)
                { 
                    ViewBag.CountryId = c_id;
                    var c_films = _context.Film.Where(d => d.CountryId==c_id).Include(f => f.Country).Include(f => f.Director).Include(f => f.Picture);
                    if(c_films==null)
                    {
                        return RedirectToAction("Create", "Films", new { c_id });
                    }
                    return View(await c_films.ToListAsync());
                }
                     var films = _context.Film.Include(f => f.Country).Include(f => f.Director).Include(f => f.Picture);
                     return View(await films.ToListAsync());
            }
            else
            {
                ViewBag.FilmId = id;
                var films = _context.Film.Where(f => f.FilmId == id).Include(f => f.Country).Include(f => f.Director).Include(f => f.Picture);
                return View(await films.ToListAsync());
            }
        }

        public async Task<IActionResult> Direct(int? idFilm)
        {
            var film = await _context.Film.Where(f => f.FilmId == idFilm).FirstOrDefaultAsync();
            return RedirectToAction("Detail", "Directors", new { id = film.DirectorId});
        }

        // GET: Films/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Film
                .Include(f => f.Country)
                .Include(f => f.Director)
                .FirstOrDefaultAsync(m => m.FilmId == id);
            if (film == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index","FilmGenres",new { f_id=id, name=film.Name});
        }

        public async Task<IActionResult> Detail_d(int? idFilm)
        {
            var film = await _context.Film
                .Include(f => f.Country)
                .Include(f => f.Director)
                .FirstOrDefaultAsync(m => m.FilmId == idFilm);
            if (film == null)
            {
                return NotFound();
            }

            return RedirectToAction("Detail", "Directors", new { id = film.DirectorId});
        }
        public async Task<IActionResult> Detail_c(int? idFilm)
        {
            if (idFilm == null)
            {
                return NotFound();
            }

            var film = await _context.Film
                .Include(f => f.Country)
                .Include(f => f.Director)
                .FirstOrDefaultAsync(m => m.FilmId == idFilm);
            return RedirectToAction("Index", "Films", new { c_id = film.CountryId });
        }
        public IActionResult create_country(int? f_id)
        {
            return RedirectToAction("Create", "Countries", new { f_id });
        }
        public IActionResult create_director(int? f_id)
        {
            return RedirectToAction("Create", "Directors", new { f_id });
        }
        // GET: Films/Create
        public IActionResult Create()
        {
            ViewData["CountryId"] = new SelectList(_context.Country, "CountryId", "Name");
            ViewData["DirectorId"] = new SelectList(_context.Director, "DirectorId", "Name");
            return View();
        }

        // POST: Films/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? c_id, [Bind("FilmId,Name,Release,Budget,DirectorId,CountryId,Description")] Film film)
        {
            if (ModelState.IsValid)
            {
                _context.Add(film);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if(c_id!= null)
            {
                _context.Add(film);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index","Films",new { c_id});
            }
            ViewData["CountryId"] = new SelectList(_context.Country, "CountryId", "Name", film.CountryId);
            ViewData["DirectorId"] = new SelectList(_context.Director, "DirectorId", "Name", film.DirectorId);
            return View(film);
        }

        // GET: Films/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Film.FindAsync(id);
            if (film == null)
            {
                return NotFound();
            }
            ViewData["CountryId"] = new SelectList(_context.Country, "CountryId", "Name", film.CountryId);
            ViewData["DirectorId"] = new SelectList(_context.Director, "DirectorId", "Name", film.DirectorId);
            return View(film);
        }

        // POST: Films/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FilmId,Name,Release,Budget,DirectorId,CountryId,Description")] Film film)
        {
            if (id != film.FilmId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(film);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmExists(film.FilmId))
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
            ViewData["CountryId"] = new SelectList(_context.Country, "CountryId", "Name", film.CountryId);
            ViewData["DirectorId"] = new SelectList(_context.Director, "DirectorId", "Name", film.DirectorId);
            return View(film);
        }

        // GET: Films/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Film
                .Include(f => f.Country)
                .Include(f => f.Director)
                .FirstOrDefaultAsync(m => m.FilmId == id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // POST: Films/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var filmGenre = _context.FilmGenre.Where(f => f.FilmId == id).Include(f => f.Film).Include(f => f.Genre).ToList();
            _context.FilmGenre.RemoveRange(filmGenre);
            await _context.SaveChangesAsync();

            var filmArtist = _context.FilmArtist.Where(f => f.FilmId == id).Include(f => f.Film).Include(f => f.Artist).ToList();
            _context.FilmArtist.RemoveRange(filmArtist);
            await _context.SaveChangesAsync();

            var film = await _context.Film.FindAsync(id);
                _context.Film.Remove(film);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); 
        }

        private bool FilmExists(int id)
        {
            return _context.Film.Any(e => e.FilmId == id);
        }
    }
}
