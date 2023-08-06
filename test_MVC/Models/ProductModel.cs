using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using test_MVC.Models;

namespace test_MVC.Models
{
    public class ProductModel
    {

            [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public string NoteGeneral { get; set; }
        public string? NoteSpecial { get; set; }
            public CategoryModel Category { get; set; }

        
  
    }
}
