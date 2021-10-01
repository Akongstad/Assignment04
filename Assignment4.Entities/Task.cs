using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Assignment4.Entities
{
    public class Task
    {
        public Task(){
            this.tags = new HashSet<Tag>();
        }
        public int iD {get; set;}

        [StringLength(100)]
        [Required]
        public string Title {get; set;}
        public int? userID {get; set;}
        public string? Description {get; set;}
        public enum State {get; set;}
        public ICollection<Tag> tags{get; set;}
    }

    
}
