using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tryy.Models
{
    public class CreateModel
    {
         public string Name { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public string NoteGeneral { get; set; }
        public string NoteSpecial { get; set; }
            public CategoryModel Category { get; set; }
    }
}