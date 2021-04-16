using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects
{
    public abstract class NewsForManipulationDto
    {
        [Required(ErrorMessage = "Title is a required field.")]
        [MaxLength(60, ErrorMessage = "Maximum length for rhe Title is 60 characters")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "Description is a required field.")]
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}