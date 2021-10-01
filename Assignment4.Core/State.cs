using System.ComponentModel.DataAnnotations;

namespace Assignment4.Core
{
    [Required]
    public enum State
    {
        
        New,
        Active,
        Resolved,
        Closed,
        Removed
        
    }
}
