using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tryy.Models
{
    public class UserModel
    {
        [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<RoleModel> Roles { get; set; }
         public bool IsBlocked { get; set; } 
    }
}
