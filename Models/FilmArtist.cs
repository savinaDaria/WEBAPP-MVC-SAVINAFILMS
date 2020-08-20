using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SAVINAFILMS
{
    public partial class FilmArtist
    {
        public int Id { get; set; }
        [Display(Name = "Фільм")]
        public int FilmId { get; set; }
        [Display(Name = "Актор")]
        public int ArtistId { get; set; }
        [Display(Name = "Опис ролі")]
        public string Description { get; set; }
        [Display(Name = "Актор")]
        public virtual Artist Artist { get; set; }
        [Display(Name = "Фільм")]
        public virtual Film Film { get; set; }
    }
}
