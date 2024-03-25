using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Data.SqlClient;
using Selfcheckoutwebsite.Models; /*getting the model*/
using System.Data;

namespace Selfcheckoutwebsite.Controllers
{
    public class FirstPageController : Controller
    {
        SqlConnection con = new SqlConnection(Properties.Settings.Default.Constr);
        // GET: firstPage
        public ActionResult Start()
        {
            return View();
        }
        //this is the controller for the signup or login page
        public ActionResult SignlogView()
        {
            return View();
        }

        public ActionResult SignupClient()
        {
            return View("Signup");
        }
        //this is the controller for signup page
        public ActionResult Signup(ClientMaster client) /*Client master is the class (Model) and Client is the name of the variable we saved for the client master*/
        {
            con.Open();

            SqlCommand com = new SqlCommand("Client_Registration", con); /*client_registration is an stored procedure, a stored procedure is made in the database , we make a store procedure so that we can make the procedure and use it anywhere in the code for many times  */
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@fullname", client.Full_Name); /* @fullname is the database procedure which will save the data in the database through the 'Fullname' that we got from the frontend view*/
            com.Parameters.AddWithValue("@email", client.Email);
            com.Parameters.AddWithValue("@password", client.Password);
            com.ExecuteNonQuery();
            con.Close();
            // return Content("<script language='javascript' type='text/javascript'>alert('Registration Successfully!');</script>");
            return View("Login");


        }

        public ActionResult Login(Roles roles) /*Roles: model class*/
        {
            con.Close();
            if (roles != null) /*roles: the model class ,If the value is not null, then run the query*/
            {
                con.Open(); /*open connection*/
                SqlCommand com = new SqlCommand("LoginClient", con); /*LoginClient : procedure name , con: connection string, we declred in the properties */
                com.CommandType = CommandType.StoredProcedure; /*our commandType is our stored procedure*/
                /*We will add values from frontend to sql server*/
                com.Parameters.AddWithValue("@Email", roles.Email); /*sql stored procedure will check in database of the valuse entered is already in the database */
                com.Parameters.AddWithValue("@Password", roles.Password);

                object result = com.ExecuteScalar();
                if (result != null)
                {
                    string Role_Type = result.ToString();

                    Session["UserRole"] = Role_Type;
                    if (Role_Type == "admin")
                    {
                        List<SearchProducts> products = GetProducts();
                        ViewBag.Products = products;

                        List<Roles> uroles = GetUsers();
                        ViewBag.Totalusers = uroles;

                        con.Close();
                        return View("~/Views/Home/Dashboard.cshtml");
                    }

                    con.Close();
                    return View("ScanEnter");
                }
            }
            //mehtod to display the total number of products 
            List<SearchProducts> GetProducts()
            {
                List<SearchProducts> products = new List<SearchProducts>();
                //con.Close();
                con.Close();
                con.Open();
                using (SqlCommand com = new SqlCommand("SELECT Quantity FROM Purchase_master", con))
                {
                    using (SqlDataReader reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SearchProducts product = new SearchProducts
                            {
                                // Assuming your Product table has columns like ProductId, ProductName, Quantity, etc.
                                //ProductId = Convert.ToInt32(reader["ProductId"]),
                                //ProductName = reader["ProductName"].ToString(),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                // Add other columns as needed
                            };
                            products.Add(product);
                        }
                    }
                }
                return products;
            }


            //method to display total number of users
            List<Roles> GetUsers()
            {
                List<Roles> uroles = new List<Roles>();
                //con.Close();
                con.Close();
                con.Open();
                using (SqlCommand com = new SqlCommand("SELECT Username, Password, Role_Type  FROM Roles", con))
                {
                    using (SqlDataReader reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Roles role = new Roles
                            {
                                // Assuming your Product table has columns like ProductId, ProductName, Quantity, etc.
                                //ProductId = Convert.ToInt32(reader["ProductId"]),
                                Email = reader["Username"] is DBNull ? string.Empty : reader["Username"].ToString(),
                                Password = reader["Password"] is DBNull ? string.Empty : reader["Password"].ToString(),
                                Role_Type = reader["Role_Type"] is DBNull ? string.Empty : reader["Role_Type"].ToString(),
                                // Add other columns as needed
                            };
                            uroles.Add(role);  
                        }
                        
                    }
                }
                //con.Close();
                return uroles;
            }
            
            return Content("<script language='javascript' type='text/javascript'>alert('Invalid Username or Password');</script>");
           
        }

        [HttpPost]
        public ActionResult ScanView(Makeproducts search)
        {
            con.Open();
            var command = new SqlCommand("Select * from Purchase_Master where Item_Code = @Item_Code", con);
            command.Parameters.AddWithValue("@Item_Code", search.Item_Code);

            var searchproList = new List<Makeproducts>();

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var searchpro = new Makeproducts
                    {
                        Purchase_Id = (int)reader["Purchase_Id"],
                        Item_Code = (double)reader["Item_Code"],
                        Product_Name = DBNull.Value == null ? null : reader["Product_Name"].ToString(),
                        Unit_Price = (int)reader["Unit_Price"],
                        Quantity = (int)reader["Quantity"],
                    };

                    searchproList.Add(searchpro);
                }
            }

            con.Close();

            if (searchproList.Count > 0)
            {
                // If rows are found, return the first row (you may want to handle multiple rows differently)
                return View("Eachproduct", searchproList[0]);
            }
            else
            {
                // If no rows are found, you may want to handle this case (redirect to an error page, for example)
                return View("NoDataFound"); // Create a view called "NoDataFound" for handling this case
            }
        }
        public ActionResult NoDataFound()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EnterBarcode(SearchProducts search)
        {
            con.Open(); /*checking connection with SQL*/
            //var command = new SqlCommand($"Select * from Client_Master where DateCol between '${df.Pickup_Date}' and '${df.Release_Date}'", con);
            var command = new SqlCommand("Select * from Product_Master where Item_Code = '" + search.Item_Code + "'", con); /*we wrote here an SQL command , as we need the whole data from the database , we wrote SELECT* means that all the columns inside the table 'Product Master' and the condition is , our item code should be equal to the item code inside the SQL columns . Search is the variable of the model we wrote. */

            // var command = new SqlCommand($"Select * from Client_Master where DateCol between cast(${df.Pickup_Date}as DateTime) and cast(${df.Release_Date} as DateTime)", con);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read()) /*while searching for the  many columns inside the sql find the item code*/
                {
                    var searchpro = new SearchProducts
                    { /*creating a list */
                        Product_Id = (int)reader["Product_Id"], /*the orange one's are coming from the sql and saving it inside the same name variable which is our front-end*/
                        Item_Code = (double)reader["Item_Code"], /*the item code from the database which we were looking for will be saved inside the variable called 'item_code' and this will go to the model searchproducts and will go to the 'eachproduct' page where the whole data will be shown on the screen*/

                        Product_Name = DBNull.Value == null ? null : reader["Product_Name"].ToString(),
                        Unit_Price = (int)reader["Unit_Price"],
                        Quantity = (int)reader["Quantity"],
                    };
                    //  search.Add(SearchProducts);
                    return View("Eachproduct", searchpro); /*when the action is completed , it will take us to the 'EachProduct' page , with 'searchpro' list */
                }

            }
            return View("NoDataFound");
        }
        public ActionResult ScanenterbarcodeView()
        {
            return View();
        }
        //This is Controller for login page
        public ActionResult Signinclient()
        {
            return View("Login");
        }
        public ActionResult EnterbarcodeView()
        {
            return View();
        }
        //This is the About us page controller 
        public ActionResult about()
        {
            return View();
        }
        //this is the contact us page view controller 
        public ActionResult Contactus() 
        { 
            return View();
        }
        //this is the contact us controller for feedback submission, the user will add their feedback through this controller 
        public ActionResult Contactusfeedback(Feedback feedback) /*Client master is the class (Model) and Client is the name of the variable we saved for the client master*/
        {
                {
                    con.Open();

                    SqlCommand com = new SqlCommand("feedback_insert", con); /*feedback_insert is an stored procedure, a stored procedure is made in the database , we make a store procedure so that we can make the procedure and use it anywhere in the code for many times  */
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@Email", feedback.Email); /* @email is the database procedure which will save the data in the database through the 'Email' that we got from the frontend view*/
                    com.Parameters.AddWithValue("@Name", feedback.Name);
                    com.Parameters.AddWithValue("@Message", feedback.Message);
                    com.ExecuteNonQuery();
                    con.Close();
                    // return Content("<script language='javascript' type='text/javascript'>alert('Registration Successfully!');</script>");
                    return View("Contactus"); }
            // this code handles the submission of feedback from a form, calls a stored procedure to insert the feedback
            // into the database, and then redirects the user to the "Contactus" view.
        }


        //this is the controller of Services page
        public ActionResult Services()
        {
            return View();
        }
        public ActionResult ScanEnter()
        {
            return View();
        }
        public ActionResult ScanBarcode()
        {
            return View();
        }
        //this is the enter barcode controller 
        public ActionResult EnterBarcode()
        {
            return View();
        }
        public ActionResult Cart()
        {
            return View();
        }

        //this is the controller for single product page pop up
        public ActionResult Eachproduct()
        {
            return View();
        }
        //this is the  page for continue or checkout
        public ActionResult Continuecheckout()
        {
            return View();
        }
        //this is the page for bill
        public ActionResult Bill()
        {
            return View();
        }

        //this is the page for Online payment
        public ActionResult Onlinepayment()
        {
            return View();
        }

        public ActionResult OnlineCashPay()
        {
            return View();
        }

        public ActionResult PaymentOptions()
        {
            return View();
        }

        public ActionResult PayWithCash() //action for Cash Payment
        {
            ViewBag.PaymentMethod = "Cash";
            return View("Bill");
        }

        public ActionResult PayWithOnline() //action for Online Payment
        {
            ViewBag.PaymentMethod = "K-net/Online";
            return View("Bill");
        }
        public ActionResult WrongBarcode()
        {
            return View();
        }

    }

   

}