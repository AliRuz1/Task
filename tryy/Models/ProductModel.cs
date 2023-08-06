using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tryy.Models
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

        public static explicit operator ProductModel(CreateModel v)
        {
            return new ProductModel
            {
               // Id = v.Id,
                Name = v.Name,
                Price = v.Price,
                CategoryId = v.CategoryId,
                Description = v.Description,
                NoteGeneral = v.NoteGeneral,
                NoteSpecial = v.NoteSpecial,
                Category = v.Category
            };
        }
    }
}
