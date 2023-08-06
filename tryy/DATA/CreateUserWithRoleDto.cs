using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tryy.Models;

namespace tryy.DATA
{
    public class CreateUserWithRoleDto
    {
            public UserModel User { get; set; }
    public RoleModel Role { get; set; }
    }
}