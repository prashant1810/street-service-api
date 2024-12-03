using System.ComponentModel.DataAnnotations;

namespace PTV.Services.StreetAPI.Models
{
    public class AddPointRequest
    {
        [Required]
        [Range(-180.0, 180.0, ErrorMessage = "Longitude must be between -180 and 180.")]
        public double Longitude { get; set; }
        [Required]
        [Range(-90.0, 90.0, ErrorMessage = "Latitude must be between -90 and 90.")]
        public double Latitude { get; set; }
    }
}
