using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class News
    {
        [Column("NewsId")]
        public Guid Id { get; set; }
      
        [Required(ErrorMessage = "Title is a required field.")]
        [MaxLength(60, ErrorMessage = "Maximum length for rhe Title is 60 characters")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "Description is a required field.")]
        public string Description { get; set; }
        
        public string ImageUrl { get; set; }
        
        public virtual User User { get; set; }
        
        public virtual Category Category { get; set; }
    }
}