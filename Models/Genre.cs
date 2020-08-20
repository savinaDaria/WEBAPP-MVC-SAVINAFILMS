using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAVINAFILMS
{
    public partial class Genre
    {
        public Genre()
        {
            FilmGenre = new HashSet<FilmGenre>();
        }

        public int GenreId { get; set; }
        [Required(ErrorMessage = "Потрібно заповнити поле")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Довжина значення від 2 до 50 символів")]
        [RegularExpression(@"^([A-Za-z]([a-z])+)|([А-ЯІЇЩЄа-яіїщє][а-яіїєщ]+)|([A-Z]+)|([А-ЯЇІЄЩ]+)$", ErrorMessage = "Неправильний формат назви жанру")]
        
        [Display(Name = "Назва жанру")]
        public string Name { get; set; }
        [Display(Name = "Опис жанру")]
        public string Description { get; set; }
  

        public virtual ICollection<FilmGenre> FilmGenre { get; set; }
    }
}
