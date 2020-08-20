using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SAVINAFILMS
{
    public partial class Country
    {
        public Country()
        {
            Artist = new HashSet<Artist>();
            Film = new HashSet<Film>();
        }

        public int CountryId { get; set; }
        [Required(ErrorMessage = "Потрібно заповнити поле")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Довжина значення від 2 до 50 символів")]
        [RegularExpression(@"^([A-Za-z][a-z]+)|([А-Яа-я][а-я]+)|([A-Z]+)|([А-Я]+)$", ErrorMessage = "Неправильний формат назви ")]
        [Display(Name = "Назва країни")]
        public string Name { get; set; }

        public virtual ICollection<Artist> Artist { get; set; }
        public virtual ICollection<Film> Film { get; set; }
    }
}
