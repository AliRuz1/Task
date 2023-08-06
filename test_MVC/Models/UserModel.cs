using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace test_MVC.Models
{
    public class UserModel
    {
        [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<RoleModel> Roles { get; set; } = new List<RoleModel>();

          public bool IsBlocked { get; set; }
    }
}
