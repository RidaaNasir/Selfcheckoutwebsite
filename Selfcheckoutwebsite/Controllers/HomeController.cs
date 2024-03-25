using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Data.SqlClient;
using Selfcheckoutwebsite.Models;
using System.Data;

namespace Selfcheckoutwebsite.Controllers
{
    public class HomeController : Controller
    {
        SqlConnection con = new SqlConnection(Properties.Settings.Default.Constr);

        //this is the action we created to get the whole data of products in the database o be shown in the inventory table 
        public ActionResult Inventory()
        {
            //We made the recored in list 'clientmaster' is the model class, i want this model class inside the list  
            List<Makeproducts> productsinventorys = new List<Makeproducts>();
            con.Open();
            var sql = "SELECT * FROM [Selfcheckout].[dbo].[Purchase_Master]"; /*every recored in the database */
            using (var command = new SqlCommand(sql, con))
            using (var reader = command.ExecuteReader()) /*we use this because we want data to be read */
            {
                while (reader.Read())
                {
                    var productsinventory = new Makeproducts
                    {
                        Product_Name = (string)reader["Product_Name"], /*the orange one's are the database columns that will be saved in the white one's which are variables in the action and class*/
                        Item_Code = (double)reader["Item_Code"],
                        Quantity = (int)reader["Quantity"],
                        Unit_Price = (int)reader["Unit_Price"]
                    };
                    productsinventorys.Add(productsinventory);
                }
            }
            return View(productsinventorys);
        }


        public ActionResult Ali()
        {
            return View();
        }
        //this is the action we write to add the users saved in the database to be shown on the admin side's 'Users' Page.
        public ActionResult Users()
        {
            //We made the recored in list 'clientmaster' is the model class, i want this model class inside the list  
            List<ClientMaster> clients = new List<ClientMaster>();
            con.Open();
            var sql = "SELECT * FROM [Selfcheckout].[dbo].[Client_Master]"; /*every recored in the database */
            using (var command = new SqlCommand(sql, con))
            using (var reader = command.ExecuteReader()) /*we use this because we want data to be read */
            {
                while (reader.Read())
                {
                    var client = new ClientMaster
                    {
                        Client_Id = (int)reader["Client_Id"], /*the orange one's are the database columns that will be saved in the white one's which are variables in the action and class*/
                        Full_Name = (string)reader["Full_Name"],
                        Email = (string)reader["Email"],
                        Password = (string)reader["Password"]
                    };
                    clients.Add(client);
                }
            }
            return View(clients);
        }


        //public ActionResult Dashboard()
        //{
        //    return View();
        //}

        public ActionResult EditClient(int id) // Pass the client ID as a parameter
        {
            if (id == 0)
            {
                // Handle the case when the ID is not provided, e.g., return a view or a message.
                return View("NoClientSelected"); // Create a view for this case.
            }

            con.Open();
            var command = new SqlCommand($"SELECT * FROM Client_Master WHERE Client_Id = {id}", con);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var client = new ClientMaster
                    {
                        Client_Id = (int)reader["Client_Id"],
                        Full_Name = reader["Full_Name"].ToString(), // Correct the field names
                        Email = reader["Email"].ToString(),
                        Password = reader["Password"].ToString(),
                    };

                    return View(client);
                }
                else
                {
                    // Handle the case when no matching client is found, e.g., return a view or a message.
                    return View("ClientNotFound"); // Create a view for this case.
                }
            }
        }
        public ActionResult Addproducts()
        {
            return View("Makeproducts");
        }
        [HttpPost] /*HttpPost is an attribute used in C# to specify that a controller action should only respond to HTTP POST requests. It's a way to define the HTTP verb that a particular action is designed to handle.*/
        public ActionResult Makeproducts(Makeproducts mk)
        {
            con.Open();

            SqlCommand com = new SqlCommand("Purchase_pro", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@product_name", mk.Product_Name);
            com.Parameters.AddWithValue("@itemcode", mk.Item_Code);
            com.Parameters.AddWithValue("@quantity", mk.Quantity);
            com.Parameters.AddWithValue("@unit_price", mk.Unit_Price);
            com.ExecuteNonQuery();
            con.Close();
            // return Content("<script language='javascript' type='text/javascript'>alert('Registration Successfully!');</script>");

            return View();
        }

        public ActionResult EditInventory()
        {
            return View();
        }
        public ActionResult UpdateRoles()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateRoles(ClientMaster cm)
        {
            con.Close();
            con.Open();

            // Assuming your Roles table has columns Email, Password, Role_Type
            string updateQuery = "UPDATE Client_Master SET Full_Name = @fullname, Email = @Email, Password = @Password WHERE Client_Id = '" + cm.Client_Id + "'";

            using (SqlCommand cmd = new SqlCommand(updateQuery, con))
            {
                cmd.Parameters.AddWithValue("@fullname", cm.Full_Name);
                cmd.Parameters.AddWithValue("@Password", cm.Password); // Use roles.Password instead of Password
                cmd.Parameters.AddWithValue("@Email", cm.Email);
                cmd.ExecuteNonQuery();
            }
            con.Close(); // Close the connection after the operation
            return View("EditClient");
        }


        //saving it in the database and showing on the feedback page 
        public ActionResult Feedback()
        {
            //We made the recored in list 'clientmaster' is the model class, i want this model class inside the list  
            List<Feedback> feedbacks = new List<Feedback>();
            con.Open();
            var sql = "SELECT * FROM [Selfcheckout].[dbo].[Feedback]"; /*every recored in the database */
            using (var command = new SqlCommand(sql, con))
            using (var reader = command.ExecuteReader()) /*we use this because we want data to be read */
            {
                while (reader.Read())
                {
                    var feedback = new Feedback
                    {
                        Email = (string)reader["Email"], /*the orange one's are the database columns that will be saved in the white one's which are variables in the action and class*/
                        Name = (string)reader["Name"],
                        Message = (string)reader["Message"],

                    };
                    feedbacks.Add(feedback);
                }
            }
            return View(feedbacks);
        }
        public ActionResult Dashboard() {

            return View();
    }
        
   } 
}




        







    
