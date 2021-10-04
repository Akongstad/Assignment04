using System;
using System.ComponentModel.DataAnnotations;

namespace Assignment4.Entities
{
    public class Tag
    {
       public int id {get; set; }

       [Required]
       [StringLength(50)]
       [Key]
       public string name{get; set;} 
    }
}
