﻿using System.ComponentModel.DataAnnotations;

namespace Street.Services.CrudAPI.Models.Dto
{
    public class StreetDto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Geometry { get; set; }
        [Required]
        public int Capacity { get; set; }
    }
}
