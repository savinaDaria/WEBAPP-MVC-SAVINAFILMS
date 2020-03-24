using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SAVINAFILMS
{
    public partial class FilmGenre
    {
        public int Id { get; set; }
        public int FilmId { get; set; }
        public int GenreId { get; set; }
        [Display(Name = "Фільм")]
        public virtual Film Film { get; set; }
        [Display(Name = "Жанр")]
        public virtual Genre Genre { get; set; }
    }
}
