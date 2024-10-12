using System.ComponentModel.DataAnnotations;

namespace LecturerLink2.Models
{
    public class Claims
    {

        public int ID { get; set; }

        public int? LecturerID { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string? Firstname { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string? Lastname { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Hours Worked must be a positive number")]
        [Display(Name = "Hours Worked")]
        public double? HouresWorked { get; set; }

        [Required]
        [Display(Name = "Claims Period Start")]
        public DateTime Claimsperiodstart { get; set; }

        [Required]
        [Display(Name = "Claims Period End")]
        public DateTime Claimsperiodend { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Rate Per Hour must be a positive number")]
        public double RatePerHour { get; set; }

        [Display(Name = "Total Amount")]
        public double TotalAmount { get; set; }

        [Display(Name = "Description Of Work")]
        public string? DiscriptionOfWork { get; set; }

        public string? PaymentStatus { get; set; }

        public string? FilePath { get; set; }

    }
}
