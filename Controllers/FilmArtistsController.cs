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
    public class FilmArtistsController : Controller
    {
        private readonly lab_films_picContext _context;

        public FilmArtistsController(lab_films_picContext context)
        {
            _context = context;
        }

        // GET: FilmArtists
        public async Task<IActionResult> Index(int? id, string? name)
        {
            if (id == null) return RedirectToAction("Index", "Artists");
            ViewBag.ArtistId = id;
            ViewBag.Name = name;
            var filmartists = _context.FilmArtist.Where(f => f.ArtistId == id).Include(f => f.Film).Include(f => f.Artist);
            return View(await filmartists.ToListAsync());
        }

        // GET: FilmArtists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmArtist = await _context.FilmArtist
                .Include(f => f.Artist)
                .Include(f => f.Film)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filmArtist == null)
            {
                return NotFound();
            }
            return RedirectToAction("Index", "Films", new { id = filmArtist.FilmId });
        }

        // GET: FilmArtists/Create
        public IActionResult Create(int ArtistId, string? name)
        {
            ViewData["FilmId"] = new SelectList(_context.Film, "FilmId", "Name");
            ViewBag.ArtistId = ArtistId;
            ViewBag.Name = name;
            return View();
        }

        // POST: FilmArtists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int ArtistId , [Bind("Id,FilmId,ArtistId,Description")] FilmArtist filmArtist)
        {
            var nameA = _context.Artist.Where(fg => fg.ArtistId == ArtistId).FirstOrDefault().Name;
            filmArtist.ArtistId = ArtistId;
            if (ModelState.IsValid)
            {
                _context.Add(filmArtist);
                await _context.SaveChangesAsync();
               
                return RedirectToAction("Index", "FilmArtists", new { id = ArtistId, name = nameA });
            }
            ViewData["FilmId"] = new SelectList(_context.Film, "FilmId", "Name", filmArtist.FilmId);
            return RedirectToAction("Index", "FilmArtists", new { id = ArtistId, name = nameA });

        }

        // GET: FilmArtists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmArtist = await _context.FilmArtist.FindAsync(id);
            if (filmArtist == null)
            {
                return NotFound();
            }
            ViewData["ArtistId"] = new SelectList(_context.Artist, "ArtistId", "Name", filmArtist.ArtistId);
            ViewData["FilmId"] = new SelectList(_context.Film, "FilmId", "Name", filmArtist.FilmId);
            return View(filmArtist);
        }

        // POST: FilmArtists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FilmId,ArtistId,Description")] FilmArtist filmArtist)
        {
            if (id != filmArtist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(filmArtist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmArtistExists(filmArtist.Id))
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
            ViewData["ArtistId"] = new SelectList(_context.Artist, "ArtistId", "Name", filmArtist.ArtistId);
            ViewData["FilmId"] = new SelectList(_context.Film, "FilmId", "Name", filmArtist.FilmId);
            return View(filmArtist);
        }

        // GET: FilmArtists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmArtist = await _context.FilmArtist
                .Include(f => f.Artist)
                .Include(f => f.Film)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filmArtist == null)
            {
                return NotFound();
            }

            return View(filmArtist);
        }

        // POST: FilmArtists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id, int? f_id)
        {
            if (id == null && f_id != null)
            {
                var fG = _context.FilmArtist.Where(f => f.FilmId == f_id).Include(f => f.Artist).ToList();
                _context.FilmArtist.RemoveRange(fG);
                await _context.SaveChangesAsync();
                bool i;
                return RedirectToAction("deleting1", "Films", new { i = true });
            }
            var filmArtist = await _context.FilmArtist.FindAsync(id);
            _context.FilmArtist.Remove(filmArtist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilmArtistExists(int id)
        {
            return _context.FilmArtist.Any(e => e.Id == id);
        }
    }
}
