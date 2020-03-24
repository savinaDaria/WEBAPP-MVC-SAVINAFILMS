using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SAVINAFILMS;

namespace SAVINAFILMS.Controllers
{
    public class GenresController : Controller
    {
        private readonly lab_films_picContext _context;

        public GenresController(lab_films_picContext context)
        {
            _context = context;
        }

        // GET: Genres
        public async Task<IActionResult> Index()
        {
            return View(await _context.Genre.ToListAsync());
        }

        // GET: Genres/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var genres = await _context.Genre.FirstOrDefaultAsync(m => m.GenreId == id);
            if (genres == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "FilmGenres", new { id = genres.GenreId, name = genres.Name });
        }

        // GET: Genres/Create
        public IActionResult Create()
        {
            return View();
        }
       
 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        { 

            if (ModelState.IsValid)
            {
                if (fileExcel != null)
                {
                    using var stream = new FileStream(fileExcel.FileName, FileMode.Create);
                    await fileExcel.CopyToAsync(stream);
                    using XLWorkbook workBook = new XLWorkbook(stream, XLEventTracking.Disabled);
                    foreach (IXLWorksheet worksheet in workBook.Worksheets)
                    {
                        Genre newgenre;
                        var c = (from cat in _context.Genre
                                 where cat.Name.Contains(worksheet.Name)
                                 select cat).ToList();
                        if (c.Count > 0)
                        {
                            newgenre = c[0];
                        }
                        else
                        {
                            newgenre = new Genre
                            {
                                Name = worksheet.Name,
                                Description = "from file"
                            };
                    
                            _context.Genre.Add(newgenre);
                        }
                        
                        foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                        {

                            Film film = new Film();
                            FilmGenre filmGenre = new FilmGenre();
                            film.Name = row.Cell(1).Value.ToString();
                            film.Release = Convert.ToInt32(row.Cell(6).Value);
                            film.Budget = row.Cell(10).Value.ToString();
                            film.Description = row.Cell(11).Value.ToString();
                            filmGenre.Film = film;
                            filmGenre.Genre = newgenre;

                            if (row.Cell(9).Value.ToString().Length > 0)
                            {
                                Country country;
                                var a = (from aut in _context.Country
                                         where aut.Name.Contains(row.Cell(9).Value.ToString())
                                         select aut).ToList();
                                if (a.Count() > 0)
                                {
                                    country = a[0];
                                }
                                else
                                {
                                    country = new Country
                                    {
                                        Name = row.Cell(9).Value.ToString()
                                    };
                                    _context.Add(country);
                                }
                                film.Country = country;
                            }
                            if (row.Cell(2).Value.ToString().Length > 0)
                            {
                                Director director;
                                var a = (from aut in _context.Director
                                         where aut.Name.Contains(row.Cell(2).Value.ToString())
                                         select aut).ToList();
                                if (a.Count() > 0)
                                {
                                    director = a[0];
                                }
                                else
                                {
                                    director = new Director();
                                    director.Name = row.Cell(2).Value.ToString();
                                    director.Sex = row.Cell(3).Value.ToString();
                                    director.Birth = Convert.ToDateTime(row.Cell(4).Value);
                                    if (!string.IsNullOrEmpty(row.Cell(5).Value.ToString()))
                                    {
                                        director.Death = Convert.ToDateTime(row.Cell(5).Value);

                                    }
                                    else
                                    {
                                        director.Death = null;
                                    }
                                    _context.Add(director);
                                }
                                film.Director = director;
                            }

                            if (row.Cell(7).Value.ToString().Length > 0)
                            {
                                Company company;
                                var a = (from aut in _context.Company
                                         where aut.Name.Contains(row.Cell(7).Value.ToString())
                                         select aut).ToList();
                                if (a.Count() > 0)
                                {
                                    company = a[0];
                                }
                                else
                                {
                                    company = new Company
                                    {
                                        Name = row.Cell(7).Value.ToString(),
                                        Year = Convert.ToInt32(row.Cell(8).Value)
                                    };
                                    _context.Add(company);
                                }
                                film.Director.Company = company;
                            }
                            _context.Film.Add(film);
                            _context.FilmGenre.Add(filmGenre);

                            if (row.Cell(7).Value.ToString().Length == 0 || row.Cell(2).Value.ToString().Length == 0 || row.Cell(9).Value.ToString().Length == 0)
                            {
                               return NotFound();   
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Create1(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Film
                .Include(f => f.Country)
                .Include(f => f.Director)
                .FirstOrDefaultAsync(m => m.FilmId == id);

            return RedirectToAction("Create", "FilmGenres", new { genreId = id});
        }

        // POST: Genres/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GenreId,Description,Name")] Genre genre)
        {
            if (ModelState.IsValid)
            {
                _context.Add(genre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(genre);
        }

        // GET: Genres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genre.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }
            return View(genre);
        }

        // POST: Genres/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GenreId,Description,Name")] Genre genre)
        {
            if (id != genre.GenreId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GenreExists(genre.GenreId))
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
            return View(genre);
        }

        // GET: Genres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genre
                .FirstOrDefaultAsync(m => m.GenreId == id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        // POST: Genres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genre = await _context.Genre.FindAsync(id);
            _context.Genre.Remove(genre);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GenreExists(int id)
        {
            return _context.Genre.Any(e => e.GenreId == id);
        }
    }
}
