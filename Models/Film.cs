using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SAVINAFILMS
{
    public partial class Film
    {
        public Film()
        {
            FilmArtist = new HashSet<FilmArtist>();
            FilmGenre = new HashSet<FilmGenre>();
        }
        
        
        public int FilmId { get; set; }
        [Required(ErrorMessage = "Потрібно заповнити поле")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Довжина значення від 2 до 50 символів")]
        [Display(Name = "Назва фільму")]
       
        public string Name { get; set; }
        [Required(ErrorMessage = "Потрібно заповнити поле")]
        [Range(1600, 2020  , ErrorMessage = "Введіть від 1600 до поточного")]
        [Display(Name = "Рік виходу")]
        public int Release { get; set; }
        [Required(ErrorMessage = "Потрібно заповнити поле")]
        [RegularExpression(@"^[1-9][0-9]+$", ErrorMessage = "Введіть число ")]
        [DataType(DataType.Currency)]
        [Display(Name = "Бюджет в $")]
        public string Budget { get; set; }

        [Display(Name = "Режисер")]
        public int DirectorId { get; set; }

        [Display(Name = "Країна")]
        public int CountryId { get; set; }

        [Display(Name = "Опис фільму")]
        public string Description { get; set; }

        [Display(Name = "Країна")]
        public virtual Country Country { get; set; }
        [Display(Name = "Режисер")]
        public virtual Director Director { get; set; }
        public virtual Picture Picture { get; set; }
        public virtual ICollection<FilmArtist> FilmArtist { get; set; }
        public virtual ICollection<FilmGenre> FilmGenre { get; set; }
    }
}
