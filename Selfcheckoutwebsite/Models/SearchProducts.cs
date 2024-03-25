using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Selfcheckoutwebsite.Models
{ /*Controller works as a bridge between cmodel/database and view */
    public class SearchProducts /*all the values and intergers are 'publc' because we need to use all of these variables outside the class*/
    {/*this is the client scanning the barcode to get the item's information*/
        public int Product_Id { get; set; } /*use the 'names' and 'data types' that are inside the databse*/
        public double Item_Code { get; set; } /*item code coming from the frontend goes to the firstpagecontroller's Action */
        public string Product_Name { get; set; }
        public int Unit_Price { get; set; }
        public int Quantity { get; set; }
    }
}