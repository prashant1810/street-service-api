using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;
using PTV.Services.StreetAPI.Extensions;

namespace Street.Services.CrudAPI.Models
{
    public class StreetInfo
    {
        [Key]

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Streets's name cannot be empty.")]
        public string Name { get; set; } = String.Empty;
        [Required(ErrorMessage = "Streets's geometry cannot be empty.")]
        [JsonConverter(typeof(LineStringJsonConverter))]
        public LineString Geometry { get; set; }
        [Required(ErrorMessage = "Streets's capacity cannot be empty.")]
        public int Capacity { get; set; }
    }
}
