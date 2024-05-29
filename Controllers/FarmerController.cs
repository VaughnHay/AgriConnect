using AgriConnectLibrary;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Text;

namespace AgriConnect_ST10044023.Controllers
{
    public class FarmerController : Controller
    {
        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateProduct(IFormCollection col)
        {
            // Assign FarmerID to the product from the session
            string farmerID = EmployeeIDSession.GetEmployeeID;

            // Get data from form collection
            string productID = col["ProductID"];
            string name = col["Name"];
            string category = col["Category"];
            DateTime productionDate = Convert.ToDateTime(col["ProductDate"]);

            // Create Product object and assign FarmerID
            Product product = new Product
            {
                ProductID = productID,
                Name = name,
                Category = category,
                ProductDate = productionDate,
                FarmerID = farmerID
            };

            // Check if the form data is valid
            if (ModelState.IsValid)
            {
                using (SqlConnection connection = Connections.GetConnection())
                {
                    string cmdInsert = "INSERT INTO AgriProducts (ProductID, FarmerID, Name, Category, ProductionDate) VALUES (@ProductID, @FarmerID, @Name, @Category, @ProductionDate)";
                    SqlCommand command = new SqlCommand(cmdInsert, connection);
                    command.Parameters.AddWithValue("@ProductID", product.ProductID);
                    command.Parameters.AddWithValue("@FarmerID", product.FarmerID);
                    command.Parameters.AddWithValue("@Name", product.Name);
                    command.Parameters.AddWithValue("@Category", product.Category);
                    command.Parameters.AddWithValue("@ProductionDate", product.ProductDate);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                ViewBag.SuccessMessage = "Product created successfully.";
                ModelState.Clear();
                return View(); // Return the view after successful creation
            }
            else
            {
                ViewBag.ErrorMessage = "Error occurred while creating the product.";
                return View(product);
            }
        }

        public IActionResult DisplayEmployeeProfile()
        {
            List<Employee> emp = new List<Employee>();

            using (SqlConnection connection = Connections.GetConnection())
            {
                string cmdSelect = "SELECT EmpID, Name, Surname FROM AgriEmployees";
                SqlCommand command = new SqlCommand(cmdSelect, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Employee employee = new Employee
                    {
                        EmpID = reader.GetString(0),
                        Name = reader.GetString(1),
                        Surname = reader.GetString(2),
                    };
                    emp.Add(employee);
                }
                connection.Close();
            }

            return View(emp);
        }

        public IActionResult DisplayOwnProducts(string category = null, DateTime? startDate = null, DateTime? endDate = null)
        {

            List<Product> products = new List<Product>();

            // Get FarmerID from session
            string farmerID = EmployeeIDSession.GetEmployeeID;

            using (SqlConnection connection = Connections.GetConnection())
            {
                string cmdSelect = "SELECT ProductID, Name, Category, ProductionDate FROM AgriProducts WHERE FarmerID = @FarmerID";

                SqlCommand command = new SqlCommand(cmdSelect, connection);
                command.Parameters.AddWithValue("@FarmerID", farmerID);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Product product = new Product
                    {
                        ProductID = reader.GetString(0),
                        Name = reader.GetString(1),
                        Category = reader.GetString(2),
                        ProductDate = reader.GetDateTime(3)
                    };
                    products.Add(product);
                }
                connection.Close();
            }

            return View(products);
        }
    }
}
