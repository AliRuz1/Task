using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tryy.Models
{
    public class UserCreateModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public RoleModel Roles { get; set; }
         public bool IsBlocked { get; set; }
        public int RoleId { get; private set; }
    }
}