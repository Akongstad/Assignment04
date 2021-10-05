using System;
using System.ComponentModel.DataAnnotations;

namespace Assignment4.Entities
{
    public class Tag
    {
       public int Id {get; set; }

       [Required]
       [StringLength(50)]
       [Key]
       public string Name{get; set;} 
    }
}
