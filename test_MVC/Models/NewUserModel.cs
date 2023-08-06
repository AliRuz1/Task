using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace test_MVC.Models
{
    public class NewUserModel
    {
            public string Email { get; set; }
    public string Password { get; set; }
    public List<RoleModel> Roles { get; set; }
    public bool IsBlocked { get; set; }
    }
}