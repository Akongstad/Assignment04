using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment4.Entities
{
    public class User
    {
        public int iD { get; set; }

        [StringLength(100)]
        [Required]
        public string Name { get; set; }

        [StringLength(100)]
        [Required]
        [EmailAddress]
        [Key]
        //[Remote(action: "VerifyEmail", controller: "UserController")]//,ErrorMessage="Email already in use")]
        public string Email { get; set; }

        public ICollection<int> Tasks { get; set; }
    }
}
