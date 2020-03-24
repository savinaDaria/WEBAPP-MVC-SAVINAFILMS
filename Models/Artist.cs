using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
namespace SAVINAFILMS
{
    public partial class Artist
    {
        public Artist()
        {
            FilmArtist = new HashSet<FilmArtist>();
        }

      
        public int ArtistId { get; set; }
        [Required(ErrorMessage = "Потрібно заповнити поле")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Довжина значення від 2 до 50 символів")]
        [Display(Name = "Ім'я")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Потрібно заповнити поле")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [Remote("CheckDate","Home",ErrorMessage ="Введіть рік від 1700 до поточної дати")]
        [Display(Name = "Дата народження")]
        public DateTime Birth { get; set; }


        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [Remote("CheckDate", "Home", ErrorMessage = "Введіть рік від 1700 до поточного")]
        [Display(Name = "Дата смерті")]
        public DateTime? Death { get; set; }
        [Required(ErrorMessage = "Потрібно заповнити поле")]
        [RegularExpression(@"ч|ж", ErrorMessage = "Оберіть ж(жінка) або ч(чоловік)")]
        [Display(Name = "Стать")]
        public string Sex { get; set; }
       
        [Display(Name = "Країна")]
        public int CountryId { get; set; }

        public virtual Country Country { get; set; }
        public virtual ICollection<FilmArtist> FilmArtist { get; set; }
      
    }
}
