using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace SAVINAFILMS.Controllers
{
    public class ArtistsController : Controller
    {
        private readonly lab_films_picContext _context;

        public ArtistsController(lab_films_picContext context)
        {
            
            _context = context;
        }

        // GET: Artists
        
        public async Task<IActionResult> Index(string char1, string text1,int year1 , int year2)
        {
            if (year2 != 0) 
            {
                string var = year2.ToString();
                FormattableString query = $"SELECT * FROM Artist WHERE Artist.artist_id IN( SELECT DISTINCT Artist.artist_id  FROM Artist INNER JOIN FilmArtist ON Artist.artist_id = FilmArtist.artist_id WHERE FilmArtist.film_id IN(SELECT Film.film_id FROM Film WHERE Film.budget> {year2})); ";
                var c = _context.Artist.FromSqlInterpolated(query);
                var q = await c.ToListAsync();
                return View(q);
            }
            if(year1!=0)
            {
                FormattableString query = $"SELECT DISTINCT * FROM Artist WHERE Artist.artist_id IN (SELECT FilmArtist.artist_id FROM FilmArtist INNER JOIN Artist ON FilmArtist.artist_id= Artist.artist_id GROUP BY FilmArtist.artist_id HAVING count(FilmArtist.film_id)>{year1});";
                var c = _context.Artist.FromSqlInterpolated(query);
                var q = await c.ToListAsync();
                return View(q);
            }
            if (char1 != null && text1 != null)
            {
                FormattableString query=$"";
                if (String.Equals(char1,"<")) { query = $"SELECT * FROM [Artist] WHERE [Artist].[artist_id] IN(SELECT [FilmArtist].[artist_id] FROM [FilmArtist] INNER JOIN [Artist] ON [FilmArtist].[artist_id]= [Artist].[artist_id] GROUP BY [FilmArtist].[artist_id] HAVING count([FilmArtist].[film_id])<(SELECT COUNT([FilmArtist].[film_id])FROM [FilmArtist] INNER JOIN [Artist] ON [FilmArtist].[artist_id] = [Artist].[artist_id] WHERE [Artist].[name]={@text1}));"; }
                if (String.Equals(char1, ">")) { query = $"SELECT * FROM [Artist] WHERE [Artist].[artist_id] IN(SELECT [FilmArtist].[artist_id] FROM [FilmArtist] INNER JOIN [Artist] ON [FilmArtist].[artist_id]= [Artist].[artist_id] GROUP BY [FilmArtist].[artist_id] HAVING count([FilmArtist].[film_id])>(SELECT COUNT([FilmArtist].[film_id])FROM [FilmArtist] INNER JOIN [Artist] ON [FilmArtist].[artist_id] = [Artist].[artist_id] WHERE [Artist].[name]={@text1}));"; }
                if (String.Equals(char1, "<=")) { query = $"SELECT * FROM [Artist] WHERE [Artist].[artist_id] IN(SELECT [FilmArtist].[artist_id] FROM [FilmArtist] INNER JOIN [Artist] ON [FilmArtist].[artist_id]= [Artist].[artist_id] GROUP BY [FilmArtist].[artist_id] HAVING count([FilmArtist].[film_id])<=(SELECT COUNT([FilmArtist].[film_id])FROM [FilmArtist] INNER JOIN [Artist] ON [FilmArtist].[artist_id] = [Artist].[artist_id] WHERE [Artist].[name]={@text1}));"; }
                if (String.Equals(char1, ">=")) { query = $"SELECT * FROM [Artist] WHERE [Artist].[artist_id] IN(SELECT [FilmArtist].[artist_id] FROM [FilmArtist] INNER JOIN [Artist] ON [FilmArtist].[artist_id]= [Artist].[artist_id] GROUP BY [FilmArtist].[artist_id] HAVING count([FilmArtist].[film_id])>=(SELECT COUNT([FilmArtist].[film_id])FROM [FilmArtist] INNER JOIN [Artist] ON [FilmArtist].[artist_id] = [Artist].[artist_id] WHERE [Artist].[name]={@text1}));"; }
                if (String.Equals(char1, "=")) { query = $"SELECT * FROM [Artist] WHERE [Artist].[artist_id] IN(SELECT [FilmArtist].[artist_id] FROM [FilmArtist] INNER JOIN [Artist] ON [FilmArtist].[artist_id]= [Artist].[artist_id] GROUP BY [FilmArtist].[artist_id] HAVING count([FilmArtist].[film_id])=(SELECT COUNT([FilmArtist].[film_id])FROM [FilmArtist] INNER JOIN [Artist] ON [FilmArtist].[artist_id] = [Artist].[artist_id] WHERE [Artist].[name]={@text1}));"; }
                var c = _context.Artist.FromSqlInterpolated(query);
                var q = await c.ToListAsync();
                return View(q);
            }
            else
            {
                var lab_films_picContext = _context.Artist.Include(a => a.Country);
                return View(await lab_films_picContext.ToListAsync());
            }
        }

        // GET: Artists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist.Include(a => a.Country).FirstOrDefaultAsync(m => m.ArtistId == id);
            if (artist == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "FilmArtists", new { id = artist.ArtistId, name = artist.Name });
        }

        // GET: Artists/Create
        public IActionResult Create()
        {
            var l = _context.Film.Include(f => f.Country);
            ViewData["CountryId"] = new SelectList(_context.Country, "CountryId", "Name");
            return View();
        }

        // POST: Artists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArtistId,Name,Birth,Death,Sex,CountryId")] Artist artist)
        {
            int year = DateTime.Now.Year;
            int min_year = 1700;
            int var = artist.Birth.Year;
            if( var>=year|| var<min_year)
            {
                ModelState.Clear();
                return RedirectToAction("Index", "Artists");
            }
            
            if (ModelState.IsValid)
            {
                _context.Add(artist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryId"] = new SelectList(_context.Country, "CountryId", "Name", artist.CountryId);
            return View(artist);
        }
        public IActionResult create_country(int? a_id)
        {
            return RedirectToAction("Create", "Countries", new { a_id });
        }

        // GET: Artists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }
            ViewData["CountryId"] = new SelectList(_context.Country, "CountryId", "Name", artist.CountryId);
            return View(artist);
        }

        // POST: Artists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArtistId,Name,Birth,Death,Sex,CountryId")] Artist artist)
        {
            if (id != artist.ArtistId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(artist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtistExists(artist.ArtistId))
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
            ViewData["CountryId"] = new SelectList(_context.Country, "CountryId", "Name", artist.CountryId);
            return View(artist);
        }

        // GET: Artists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist
                .Include(a => a.Country)
                .FirstOrDefaultAsync(m => m.ArtistId == id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // POST: Artists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var FilmArt = _context.FilmArtist.Where(f => f.ArtistId == id).Include(f => f.Film).Include(f => f.Artist).ToList();
            _context.FilmArtist.RemoveRange(FilmArt);
            await _context.SaveChangesAsync();
            var artist = await _context.Artist.FindAsync(id);
            _context.Artist.Remove(artist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArtistExists(int id)
        {
            return _context.Artist.Any(e => e.ArtistId == id);
        }
    }
}
