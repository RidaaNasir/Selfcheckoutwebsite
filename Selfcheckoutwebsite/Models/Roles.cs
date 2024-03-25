using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Selfcheckoutwebsite.Models
{
    public class Roles
    { /*this is for the login page */
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role_Type { get; set; }
    }
}