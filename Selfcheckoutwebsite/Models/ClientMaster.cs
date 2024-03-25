using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Selfcheckoutwebsite.Models
{
    //this is class model class we made for the client
    public class ClientMaster
    { /*this is for the signup page  */
        public int Client_Id { get; set; }
        public string Full_Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}