using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SautinSoft.Document;
using SautinSoft.Document.Tables;
using SAVINAFILMS;

namespace SAVINAFILMS.Controllers
{
    public class GenresController : Controller
    {
        private readonly lab_films_picContext _context;
        public DocumentCore dc = new DocumentCore();
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
                    List<List<string>> Errors = new List<List<string>>();
                    foreach (IXLWorksheet worksheet in workBook.Worksheets)
                    {
                        List<string> ErrorsExcelFile = new List<string>();
                        Genre newgenre;
                        var g = (from genre in _context.Genre
                                 where genre.Name.Contains(worksheet.Name)
                                 select genre).ToList();


                        if (g.Count > 0)
                        {
                            newgenre = g[0];
                        }
                        else
                        {
                            newgenre = new Genre
                            {
                                Name = worksheet.Name,
                                Description = "Завантажено з Excel-файлу "
                            };

                            _context.Genre.Add(newgenre);

                        }
                        await _context.SaveChangesAsync();
                        foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                        {

                            Film film = new Film();
                            FilmGenre filmGenre = new FilmGenre();
                            string name = row.Cell(1).Value.ToString();
                            if (name.Length > 50 || name.Length < 3)
                            {
                                string template = "Назва листа альбому:{0}. Рядок: {1}. {2}";
                                 string message = "Довжина назви фільму <3 або >50 символів.";
                                string ErrorMessage = string.Format(template, worksheet.Name, row, message);
                                ErrorsExcelFile.Add(ErrorMessage);
                                continue;
                            }
                            var f = (from flm in _context.Film where flm.Name.Contains(name) select flm).ToList();
                            if (f.Count() > 0)
                            {
                                film = f[0];
                                var fg = (from flm in _context.FilmGenre where (flm.FilmId == film.FilmId && flm.Genre.Name == newgenre.Name) select flm).ToList();
                                if (fg.Count == 0)
                                {
                                    filmGenre.Film = film;
                                    filmGenre.Genre = newgenre;
                                    _context.FilmGenre.Add(filmGenre);
                                }
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                film.Name = row.Cell(1).Value.ToString();

                                if (row.Cell(6).Value.ToString().Length < 3 || Convert.ToInt32(row.Cell(6).Value) > DateTime.Now.Year || Convert.ToInt32(row.Cell(6).Value) < 1600)
                                {
                                    string template = "Назва листа альбому: {0}. " +
                                 "Рядок: {1}. " +
                                 "{2}";
                                    string message = "  Рік виходу фільму <1600 або > поточного року або поле незаповнено.";
                                    string ErrorMessage = string.Format(template, worksheet.Name, row, message);
                                    ErrorsExcelFile.Add(ErrorMessage);
                                    continue;
                                }
                                film.Release = Convert.ToInt32(row.Cell(6).Value);
                                var match1 = Regex.Match(row.Cell(10).Value.ToString(), @"^(0|[1-9]+[0-9]*)(\.[0-9]+( )?)?( млн| млрд)$");
                                if (!match1.Success || row.Cell(10).Value.ToString().Length<2 )
                                {
                                    string template = "Назва листа альбому:{0}. Рядок: {1}. {2}";
                                    string message = " Бюджет фільму-десятковий дріб у неправильному форматі або поле незаповнено.";
                                    string ErrorMessage = string.Format(template, worksheet.Name, row, message); 
                                    ErrorsExcelFile.Add(ErrorMessage);
                                    continue;
                                }
                                film.Budget = row.Cell(10).Value.ToString();
                                film.Description = row.Cell(11).Value.ToString();
                                filmGenre.Film = film;
                                filmGenre.Genre = newgenre;

                                string name1 = row.Cell(9).Value.ToString();
                                if (name1.Length > 50 || name1.Length < 3)
                                {
                                    string template = "Назва листа альбому:{0}. Рядок: {1}. {2}";
                                    string message = "Довжина назви країни <3 або >50 символів, або поле незаповнено";
                                    string ErrorMessage = string.Format(template, worksheet.Name, row, message); 
                                    ErrorsExcelFile.Add(ErrorMessage);
                                    continue;
                                }
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

                                string name2 = row.Cell(2).Value.ToString();
                                var match5= Regex.Match(row.Cell(2).Value.ToString(), @"^([A-Z][a-z]+)\ ([A-Z][a-z]+)(\ ?([A-Z][a-z]+)?)|([А-ЯІЇЄЩ][а-яіїщє]+)\ ([А-ЯІЇЄЩ][а-яіїщє]+)(\ ?([А-ЯІЇЄЩ][а-яіїщє]+)?)$");
                                if (!match5.Success|| name2.Length > 50 || name2.Length < 3)
                                {
                                    string template = "Назва листа альбому:{0}. Рядок: {1}. {2}";
                                    string message = "Довжина імені режисера <3 або >50 символів, поле незаповнено або неправильний формат ";
                                    string ErrorMessage = string.Format(template, worksheet.Name, row, message); 
                                    ErrorsExcelFile.Add(ErrorMessage);
                                    continue;
                                }
                                Director director;
                                var n = (from aut in _context.Director
                                         where aut.Name.Contains(row.Cell(2).Value.ToString())
                                         select aut).ToList();
                                if (n.Count() > 0)
                                {
                                    director = n[0];
                                }
                                else
                                {
                                    director = new Director();
                                    director.Name = row.Cell(2).Value.ToString();
                                    var match = Regex.Match(row.Cell(3).Value.ToString(), @"ч|ж");
                                    if (!match.Success)
                                    {

                                        string template = "Назва листа альбому:{0}. Рядок: {1}. {2}";
                                        string message = "У поле Стать введено не ж(жінка) або ч(чоловік), або поле незаповнено";
                                        string ErrorMessage = string.Format(template, worksheet.Name, row, message);
                                        ErrorsExcelFile.Add(ErrorMessage);
                                        continue;
                                    }
                                    director.Sex = row.Cell(3).Value.ToString();
                                    var result = true;
                                    int year = DateTime.Now.Year - 18;
                                    int month = DateTime.Now.Month;
                                    int day = DateTime.Now.Day;
                                    int min_year = 1700;
                                    int var = Convert.ToDateTime(row.Cell(4).Value).Year;
                                    int var1 = Convert.ToDateTime(row.Cell(4).Value).Month;
                                    int var2 = Convert.ToDateTime(row.Cell(4).Value).Day;
                                    if (var > year || var < min_year || (var == year && var1 > month) || (var == year && var1 == month && var2 > day)) result = false;
                                    if (result == false || Convert.ToDateTime(row.Cell(4).Value).ToString().Length == 0)
                                    {
                                        string template = "Назва листа альбому:{0}. Рядок: {1}. {2}";
                                        string message = "Неправильна дата народження режисера або поле незаповнено";
                                        string ErrorMessage = string.Format(template, worksheet.Name, row, message);
                                        ErrorsExcelFile.Add(ErrorMessage);
                                        continue;
                                    }

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

                                string name4 = row.Cell(7).Value.ToString();
                                if (name1.Length > 50 || name1.Length < 3)
                                {
                                    string template = "Назва листа альбому:{0}. Рядок: {1}. {2}";
                                    string message = "Довжина назви кінокомпанії <3 або >50 символів, або поле незаповнено";
                                    string ErrorMessage = string.Format(template, worksheet.Name, row, message);
                                    ErrorsExcelFile.Add(ErrorMessage);
                                    continue;
                                }
                                Company company;
                                var co = (from aut in _context.Company
                                          where aut.Name.Contains(row.Cell(7).Value.ToString())
                                          select aut).ToList();
                                if (co.Count() > 0)
                                {
                                    company = co[0];
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

                                _context.Film.Add(film);
                                _context.FilmGenre.Add(filmGenre);
                                await _context.SaveChangesAsync();
                            }

                        }

                        if (ErrorsExcelFile.Count != 0)
                        {
                            Errors.Add(ErrorsExcelFile);
                        }
                    }
                    await _context.SaveChangesAsync();
                    if (Errors.Count != 0)
                    {
                        string name = fileExcel.FileName;
                        ExportErrorsWord(Errors,name);
                    }
                }

            }
            return RedirectToAction("Index", "Films");
        }

        public void ExportErrorsWord(List<List<string>> Errors, string name)
            {
                 Section section = new Section(dc);
                 dc.Sections.Add(section);
                 section.PageSetup.PaperType = PaperType.A4;
            Paragraph par = new Paragraph(dc);
            section.Blocks.Add(par);
            par.ParagraphFormat.Alignment = HorizontalAlignment.Center;
            Run run1 = new Run(dc, name);
            run1.CharacterFormat = new CharacterFormat() { FontName = "Times New Roman", Size = 18.0, FontColor = new Color(112, 173, 71) };
            par.Inlines.Add(run1);

            Paragraph p = new Paragraph(dc);
            p.ParagraphFormat.Alignment = HorizontalAlignment.Left;
            section.Blocks.Add(p);
            section.PageSetup.PageMargins = new PageMargins()
            {
                Top = LengthUnitConverter.Convert(5, LengthUnit.Millimeter, LengthUnit.Point),
                Right = LengthUnitConverter.Convert(5, LengthUnit.Millimeter, LengthUnit.Point),
                Bottom = LengthUnitConverter.Convert(5, LengthUnit.Millimeter, LengthUnit.Point),
                Left = LengthUnitConverter.Convert(5, LengthUnit.Millimeter, LengthUnit.Point)
            };
            int data = 1;
            foreach (List<string>  c in Errors)
            {
                foreach (string e in c)
                {
                    string template = "{0}. Помилка. {1}";
                    string message = string.Format(template, data,e);
                    p.Content.End.Insert(message, new CharacterFormat() { Size = 14.0, FontColor = Color.Black });
                    p.ParagraphFormat.SpaceAfter = 0;
                    p.Inlines.Add(new SpecialCharacter(dc, SpecialCharacterType.LineBreak));
                    p.Inlines.Add(new SpecialCharacter(dc, SpecialCharacterType.LineBreak));
                    data++;
                } 
            }

            string filePath = @"C:/2 курс/ Errors.docx";
            dc.Save(filePath);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
            
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
            var FilmGenre = _context.FilmGenre.Where(f => f.GenreId == id).Include(f => f.Film).Include(f => f.Genre).ToList();
            _context.FilmGenre.RemoveRange(FilmGenre);
            await _context.SaveChangesAsync();
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
