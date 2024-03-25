using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Selfcheckoutwebsite.Models
{
    public class Makeproducts
    {/*admin's side of adding products to the database*/
        public int Purchase_Id { get; set; }
        public string Product_Name { get; set; }
        public double Item_Code { get; set; }
        public int Quantity { get; set; }
        public int Unit_Price { get; set; }
    }
}