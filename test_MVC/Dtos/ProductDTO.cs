using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using test_MVC.Models;

namespace test_MVC.Dtos
{
    public class ProductDTO
    {
                public int? Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public string NoteGeneral { get; set; }
        public string NoteSpecial { get; set; }
         public CategoryModel Category { get; set; }

        public static explicit operator ProductDTO(ProductModel v)
        {
            return new ProductDTO { 
                Id = v.Id,
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