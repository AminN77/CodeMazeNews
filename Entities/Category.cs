using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class Category
    {
        [Column("CategoryId")]
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Title is a required field.")]
        [MaxLength(60, ErrorMessage = "Maximum length for the Title is 60 characters.")]
        public string Title { get; set; }

        public virtual User User { get; set; }
        
        public virtual ICollection<News> News { get; set; }
    }
}