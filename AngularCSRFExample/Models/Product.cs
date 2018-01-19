using System.ComponentModel.DataAnnotations;

namespace AngularCSRFExample.Models
{
    public class Product
    {
        [MinLength(5, ErrorMessage = "Name of the user must be alteast 5 characters long.")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
    }
}
