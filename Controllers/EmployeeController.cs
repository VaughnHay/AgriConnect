using AgriConnectLibrary;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace AgriConnect_ST10044023.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult CreateFarmer()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateFarmer(IFormCollection col)
        {
            // Retrieve EmpID from session
            string empID = EmployeeIDSession.GetEmployeeID;

            // Get data from form collection
            string farmerID = col["FarmerID"];
            string name = col["Name"];
            string email = col["Email"];
            string passwordHash = col["PasswordHash"];

            // Create Farmer object and assign EmpID
            Farmer farmer = new Farmer
            {
                FarmerID = farmerID,
                EmpID = empID, // Use empID from session
                Name = name,
                Email = email,
                Password = passwordHash
            };

            // Check if the form data is valid
            if (ModelState.IsValid)
            {
                using (SqlConnection connection = Connections.GetConnection())
                {
                    string cmdInsert = "INSERT INTO AgriFarmers (FarmerID, EmpID, Name, Email, PasswordHash) VALUES (@FarmerID, @EmpID, @Name, @Email, @PasswordHash)";
                    SqlCommand command = new SqlCommand(cmdInsert, connection);
                    command.Parameters.AddWithValue("@FarmerID", farmer.FarmerID);
                    command.Parameters.AddWithValue("@EmpID", farmer.EmpID); // Use empID from session
                    command.Parameters.AddWithValue("@Name", farmer.Name);
                    command.Parameters.AddWithValue("@Email", farmer.Email);
                    command.Parameters.AddWithValue("@PasswordHash", farmer.Password);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                ViewBag.SuccessMessage = "Farmer profile created successfully.";
                ModelState.Clear();
                return View(); // Return the view after successful creation
            }
            else
            {
                ViewBag.ErrorMessage = "Error occurred while creating the farmer profile.";
                return View(farmer);
            }
        }

        public IActionResult DisplayFarmerProfile()
        {
            List<Farmer> farmers = new List<Farmer>();

            using (SqlConnection connection = Connections.GetConnection())
            {
                string cmdSelect = "SELECT FarmerID, EmpID, Name, Email FROM AgriFarmers";
                SqlCommand command = new SqlCommand(cmdSelect, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Farmer farmer = new Farmer
                    {
                        FarmerID = reader.GetString(0),
                        EmpID = reader.GetString(1),
                        Name = reader.GetString(2),
                        Email = reader.GetString(3)
                    };
                    farmers.Add(farmer);
                }
                connection.Close();
            }

            return View(farmers);
        }

        public IActionResult DisplayProducts(string farmerID = null, string category = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            List<Product> products = new List<Product>();

            using (SqlConnection connection = Connections.GetConnection())
            {
                StringBuilder cmdSelect = new StringBuilder("SELECT ProductID, FarmerID, Name, Category, ProductionDate FROM AgriProducts");

                // Check if filtering parameters are provided
                bool hasFilters = !string.IsNullOrEmpty(farmerID) || !string.IsNullOrEmpty(category) || startDate.HasValue || endDate.HasValue;
                if (hasFilters)
                {
                    cmdSelect.Append(" WHERE");
                    List<string> filters = new List<string>();

                    if (!string.IsNullOrEmpty(farmerID))
                    {
                        filters.Add(" FarmerID = @FarmerID");
                    }
                    if (!string.IsNullOrEmpty(category))
                    {
                        filters.Add(" Category = @Category");
                    }
                    if (startDate.HasValue)
                    {
                        filters.Add(" ProductionDate >= @StartDate");
                    }
                    if (endDate.HasValue)
                    {
                        filters.Add(" ProductionDate <= @EndDate");
                    }

                    cmdSelect.Append(string.Join(" AND", filters));
                }

                SqlCommand command = new SqlCommand(cmdSelect.ToString(), connection);

                if (!string.IsNullOrEmpty(farmerID))
                {
                    command.Parameters.AddWithValue("@FarmerID", farmerID);
                }
                if (!string.IsNullOrEmpty(category))
                {
                    command.Parameters.AddWithValue("@Category", category);
                }
                if (startDate.HasValue)
                {
                    command.Parameters.AddWithValue("@StartDate", startDate);
                }
                if (endDate.HasValue)
                {
                    command.Parameters.AddWithValue("@EndDate", endDate);
                }

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Product product = new Product
                    {
                        ProductID = reader.GetString(0),
                        FarmerID = reader.GetString(1),
                        Name = reader.GetString(2),
                        Category = reader.GetString(3),
                        ProductDate = reader.GetDateTime(4)
                    };
                    products.Add(product);
                }
                connection.Close();
            }

            var filterViewModel = new ProductFilter
            {
                FarmerID = farmerID,
                Category = category,
                StartDate = startDate,
                EndDate = endDate,
                Products = products
            };

            return View(filterViewModel);
        }
    }
}
