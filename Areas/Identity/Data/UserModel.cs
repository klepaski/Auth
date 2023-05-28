using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace task4.Areas.Identity.Data;

public class UserModel : IdentityUser
{
    public DateTime RegistrationDate { get; set; }
    public DateTime LastVisitDate { get; set; }
    public bool isBlocked { get; set; }
}

