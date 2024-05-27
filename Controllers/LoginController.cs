using Microsoft.AspNetCore.Mvc;
using AgriConnectLibrary;

namespace AgriConnect_ST10044023.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult MainPage()
        {
            // Retrieve the student based on the stored EmpID in the session
            Employee loggedEmployee = Employee.GetEmployeeID(EmployeeIDSession.GetEmployeeID);

            // Return the MainPage view with the Employee data
            return View(loggedEmployee);
        }
        [HttpPost]
        public IActionResult ProcessLogin(Employee employee)
        {
            Employee loggedEmployee = Employee.GetEmployeeID(employee.empID);

            if (loggedEmployee != null && employee.empPassword == loggedEmployee.empPassword)
            {
                // Successful login, assign employee ID to session
                EmployeeIDSession.assignID(loggedEmployee.empID);
                return RedirectToAction("MainPage"); // Redirect to the main page after successful login
            }
            else
            {
                // Clear textboxes
                ModelState.Clear();

                // Add a model error for displaying an error message in the view
                ModelState.AddModelError(string.Empty, "Invalid EmpID or Password. Please try again.");

                // Return to the login page (Index) with the model containing any errors
                return View("Index", employee);
            }
        }
    }
}
