using System.ComponentModel.DataAnnotations;

namespace SAVINAFILMS
{
    public class SQLQueries
    {
        [Required(ErrorMessage = "Потрібно заповнити поле")]
        [Range(1600, 2020, ErrorMessage = "Введіть від 1600 до поточного")]
        public int year1;
        [Required(ErrorMessage = "Потрібно заповнити поле")]
        [Range(1600, 2020, ErrorMessage = "Введіть від 1600 до поточного")]
        public int year2;
        public string char1;
        public string text1;
    }
}
