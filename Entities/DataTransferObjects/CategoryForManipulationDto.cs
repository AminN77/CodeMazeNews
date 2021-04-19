using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Entities.Models;

namespace Entities.DataTransferObjects
{
    public abstract class CategoryForManipulationDto
    {
        [Required(ErrorMessage = "Title is a required field.")]
        [MaxLength(60, ErrorMessage = "Maximum length for the Title is 60 characters.")]
        public string Title { get; set; }

        public IEnumerable<News> News { get; set; }
    }
}