using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SAVINAFILMS
{
    public partial class Company
    {
        public Company()
        {
            Director = new HashSet<Director>();
        }

        public int CompanyId { get; set; }
        [Required(ErrorMessage = "Потрібно заповнити поле")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Довжина значення від 2 до 50 символів")]
        [Display(Name = "Назва компанії")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Потрібно заповнити поле")]
        [Range(1600, 2020, ErrorMessage = "Введіть від 1600 до поточного")]
        [Display(Name = "Рік заснування")]
        public int? Year { get; set; }

        public virtual ICollection<Director> Director { get; set; }
    }
}
