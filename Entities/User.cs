using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class User
    {
        [Column("UserId")]
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "FullName is a required field.")]
        [MaxLength(60, ErrorMessage = "Maximum length for the FullName is 60 characters.")]
        public string FullName { get; set; }
        
        [Required(ErrorMessage = "Email is a required field")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        
        public virtual ICollection<News> News { get; set; } 
    }
}