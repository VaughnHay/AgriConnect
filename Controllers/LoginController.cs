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
            string userID = EmployeeIDSession.GetEmployeeID;
            string userType = EmployeeIDSession.GetUserType;

            if (userType == "Employee")
            {
                Employee loggedEmployee = Employee.GetEmployeeByID(userID);
                return View("~/Views/Employee/MainPageEmployee.cshtml", loggedEmployee);
            }
            else if (userType == "Farmer")
            {
                Farmer loggedFarmer = Farmer.GetFarmerByID(userID);
                return View("~/Views/Farmer/MainPageFarmer.cshtml", loggedFarmer);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ProcessLogin(UserLogin userLogin)
        {
            Employee loggedEmployee = Employee.GetEmployeeByID(userLogin.UserID);
            Farmer loggedFarmer = Farmer.GetFarmerByID(userLogin.UserID);

            if (loggedEmployee != null && userLogin.Password == loggedEmployee.Password)
            {
                EmployeeIDSession.AssignID(loggedEmployee.EmpID, "Employee");
                return RedirectToAction("MainPage");
            }
            else if (loggedFarmer != null && userLogin.Password == loggedFarmer.Password)
            {
                EmployeeIDSession.AssignID(loggedFarmer.FarmerID, "Farmer");
                return RedirectToAction("MainPage");
            }
            else
            {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Invalid ID or Password. Please try again.");
                return View("Index", userLogin);
            }
        }
    }
}
