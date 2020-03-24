using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SautinSoft.Document;
using SautinSoft.Document.Tables;
using SAVINAFILMS;

namespace SAVINAFILMS.Controllers
{
    public class FilmGenresController : Controller
    {
        private readonly lab_films_picContext _context;
        public DocumentCore dc = new DocumentCore();
        public FilmGenresController(lab_films_picContext context)
        {
            _context = context;
        }

        // GET: FilmGenres
        public async Task<IActionResult> Index(int? id, string? name, int? f_id)
        {
            if (id == null && f_id==null) return RedirectToAction("Index", "Genres");
            if (id == null && f_id != null)
            {
                ViewBag.FilmId = f_id;
                ViewBag.Name = name;
                var filmgenres = _context.FilmGenre.Where(f => f.FilmId== f_id).Include(f => f.Film).Include(f => f.Genre);
                return View(await filmgenres.ToListAsync());

            }
            else
            {
                ViewBag.GenreId = id;
                ViewBag.Name = name;
                var filmgenres = _context.FilmGenre.Where(f => f.GenreId == id).Include(f => f.Film).Include(f => f.Genre);
                return View(await filmgenres.ToListAsync());
            }
        }

       
      
        public async Task<IActionResult> ExportWord(int? id )
        {
            string genre = _context.Genre.FirstOrDefault(m => m.GenreId == id).Name;
            var filmG = _context.FilmGenre.Where(f => f.GenreId == id).Include(f => f.Film).Include(f => f.Genre).ToList();
            string docPath = @"C:\2 курс\Films.docx";
            Section section = new Section(dc);
            dc.Sections.Add(section);
            section.PageSetup.PaperType = PaperType.A3;
            Table table = new Table(dc);

            
            for (int r = 0; r < filmG.Count; r++)
            {
                TableRow row = new TableRow(dc);
                var film = _context.Film.Where(f => f.FilmId == filmG[r].FilmId).Include(f => f.Country).Include(f => f.Director).Include(f => f.Director.Company).FirstOrDefault();

                for (int c = 0; c < 11; c++)
                {
                    TableCell cell = new TableCell(dc);
                    row.Cells.Add(cell);
                    cell.CellFormat.Borders.SetBorders(MultipleBorderTypes.Outside, BorderStyle.Dotted, Color.Black, 3.0);
                    Run run = new Run(dc);
                    if (r == 0)
                    {
                        cell.CellFormat.BackgroundColor = Color.DarkMagenta;
                        if (c == 0) run = new Run(dc, "Назва фільму");
                        if (c == 1) run = new Run(dc, "Режисер");
                        if (c == 2) run = new Run(dc, "Стать");
                        if (c == 3) run = new Run(dc, "Дата народження");
                        if (c == 4) run = new Run(dc, "Дата смерті");
                        if (c == 7) run = new Run(dc, "Рік фільму");
                        if (c == 5) run = new Run(dc, "Кінокомпанія");
                        if (c == 6) run = new Run(dc, "Рік заснування");
                        if (c == 10) run = new Run(dc, "Країна");
                        if (c == 9) run = new Run(dc, "Опис фільму");
                        if (c == 8) run = new Run(dc, "Бюджет");
                    }
                    else
                    {
                        if (c == 0) run = new Run(dc, filmG[r].Film.Name);
                        if (c == 1) run = new Run(dc, film.Director.Name.Replace("\t", ""));
                        if (c == 2) run = new Run(dc, film.Director.Sex.Replace("\t", ""));
                        if (c == 3) run = new Run(dc, film.Director.Birth.ToString());
                        if (c == 4) run = new Run(dc, film.Director.Death.ToString());
                        if (c == 5) run = new Run(dc, film.Director.Company.Name);
                        if (c == 6) run = new Run(dc, film.Director.Company.Year.ToString());
                        if (c == 7) run = new Run(dc, filmG[r].Film.Release.ToString());
                        if (c == 8) run = new Run(dc, filmG[r].Film.Budget);
                        if (c == 9) run = new Run(dc, filmG[r].Film.Description);
                        if (c == 10) run = new Run(dc, film.Country.Name);
                    }
                    cell.Blocks.Content.Replace(run.Content);
                    
                }
                table.Rows.Add(row);
            }
            dc.Content.Start.Insert(table.Content);
            
            dc.Save(docPath, new DocxSaveOptions() );
            return RedirectToAction("Index","Films");
        }
        public ActionResult Export(int? id)
        {
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                string genre =  _context.Genre.FirstOrDefault(m => m.GenreId == id).Name;
                var filmG = _context.FilmGenre.Where(f=>f.GenreId==id).Include(f=>f.Film).Include(f=>f.Genre).ToList();
                var worksheet = workbook.Worksheets.Add(genre);
                foreach (var g in filmG)
                {
                    worksheet.Cell("A1").Value = "Назва фільму";
                    worksheet.Cell("B1").Value = "Режисер";
                    worksheet.Cell("C1").Value = "Стать";
                    worksheet.Cell("D1").Value = "Дата народження";
                    worksheet.Cell("E1").Value = "Дата смерті";
                    worksheet.Cell("F1").Value = "Рік фільму";
                    worksheet.Cell("G1").Value = "Кінокомпанія";
                    worksheet.Cell("H1").Value = "Рік заснування";
                    worksheet.Cell("I1").Value = "Країна";
                    worksheet.Cell("J1").Value = "Бюджет";
                    worksheet.Cell("K1").Value = "Опис фільму";
                    worksheet.Row(1).Style.Font.Bold = true;
                    //var films = g.Film.Name.ToList();
                    for (int i = 0; i < filmG.Count; i++)
                    {
                        var film = _context.Film.Where(f => f.FilmId == filmG[i].FilmId).Include(f => f.Country).Include(f => f.Director).Include(f => f.Director.Company).FirstOrDefault();
                        worksheet.Cell(i + 2, 1).Value = filmG[i].Film.Name;
                        worksheet.Cell(i + 2, 9).Value = film.Country.Name;
                        worksheet.Cell(i + 2, 10).Value = filmG[i].Film.Budget;
                        worksheet.Cell(i + 2, 11).Value = filmG[i].Film.Description;
                        worksheet.Cell(i + 2, 2).Value = film.Director.Name;
                        worksheet.Cell(i + 2, 3).Value = film.Director.Sex;
                        worksheet.Cell(i + 2, 4).Value = film.Director.Birth;
                        worksheet.Cell(i + 2, 5).Value = film.Director.Death;
                        worksheet.Cell(i + 2, 6).Value = filmG[i].Film.Release;
                        worksheet.Cell(i + 2, 7).Value = film.Director.Company.Name;
                        worksheet.Cell(i + 2, 8).Value = film.Director.Company.Year;
                        
                    }
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();

                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"library_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
        }
        // GET: FilmGenres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmgenres = await _context.FilmGenre
                .Include(f => f.Film)
                .Include(f => f.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filmgenres == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Films", new { id = filmgenres.FilmId });
        }

        // GET: FilmGenres/Create
        public IActionResult Create(int GenreId, string? name)
        {
            ViewData["FilmId"] = new SelectList(_context.Film, "FilmId", "Name");
            ViewBag.GenreId = GenreId;
            ViewBag.Name = name;
            return View();
        }



        // POST: FilmGenres/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int GenreId, [Bind("Id,FilmId,GenreId")] FilmGenre filmGenre)
        {
            var nameG = _context.Genre.Where(fg => fg.GenreId == GenreId).FirstOrDefault().Name;
            filmGenre.GenreId = GenreId;
            if (ModelState.IsValid)
            {
                _context.Add(filmGenre);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "FilmGenres", new { id = GenreId, name = nameG });
            }
            ViewData["FilmId"] = new SelectList(_context.Film, "FilmId", "Name", filmGenre.FilmId);
            return RedirectToAction("Index", "FilmGenres", new { id = GenreId, name = nameG });
        }

        // GET: FilmGenres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmGenre = await _context.FilmGenre.FindAsync(id);
            if (filmGenre == null)
            {
                return NotFound();
            }
            ViewData["FilmId"] = new SelectList(_context.Film, "FilmId", "Name", filmGenre.FilmId);
            ViewData["GenreId"] = new SelectList(_context.Genre, "GenreId", "Name", filmGenre.GenreId);
            return View(filmGenre);
        }

        // POST: FilmGenres/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FilmId,GenreId")] FilmGenre filmGenre)
        {
            if (id != filmGenre.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(filmGenre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmGenreExists(filmGenre.Id))
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
            ViewData["FilmId"] = new SelectList(_context.Film, "FilmId", "Name", filmGenre.FilmId);
            ViewData["GenreId"] = new SelectList(_context.Genre, "GenreId", "Name", filmGenre.GenreId);
            return View(filmGenre);
        }

        // GET: FilmGenres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmGenre = await _context.FilmGenre
                .Include(f => f.Film)
                .Include(f => f.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filmGenre == null)
            {
                return NotFound();
            }

            ViewBag.IdGenre = _context.FilmGenre.Where(f => f.Id == id).FirstOrDefault().GenreId;

            return View(filmGenre);
        }

        // POST: FilmGenres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id,int? f_id)
        {
            if(id==null && f_id!=null)
            {
                var fG = _context.FilmGenre.Where(f => f.FilmId == f_id).Include(f => f.Genre).ToList();
                _context.FilmGenre.RemoveRange(fG);
                await _context.SaveChangesAsync();
                bool i;
                return RedirectToAction("deleting", "Films", new{ i=true});
            }

            
            var idGenre = _context.FilmGenre.Where(f => f.Id == id).FirstOrDefault().GenreId;
            var nameG = _context.Genre.Where(g => g.GenreId == idGenre).FirstOrDefault().Name;
            var filmGenre = await _context.FilmGenre.FindAsync(id);
            _context.FilmGenre.Remove(filmGenre);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "FilmGenres", new { id = idGenre, name = nameG });
        }

        private bool FilmGenreExists(int id)
        {
            return _context.FilmGenre.Any(e => e.Id == id);
        }
    }
}
