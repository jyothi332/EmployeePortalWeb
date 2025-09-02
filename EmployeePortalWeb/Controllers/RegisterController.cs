//using EmployeeModelPackage;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using EmployeePortalWeb.Models; // Make sure this includes BankModel
//// Add other necessary using statements

//namespace EmployeePortalWeb.Controllers
//{
//    public class RegisterController : Controller
//    {
//        private readonly string rootPath;
//        private readonly ApiService apiService;

//        public RegisterController(IWebHostEnvironment webHost, IHttpContextAccessor httpContextAccessor)
//        {
//            rootPath = webHost.ContentRootPath;
//            apiService = new ApiService();
//        }

//        [HttpGet]
//        public IActionResult Login()
//        {
//            return View();
//        }

//        [HttpGet]
//        public IActionResult BankRegister()
//        {
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> BankRegister(BankModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                TempData["ErrorMessage"] = "Invalid data. Please check your input.";
//                return View(model);
//            }

//            try
//            {
//                var employees = await apiService.GetEmployeeList();

//                // ✅ Check for existing user by username or email
//                var existing = employees.FirstOrDefault(e =>
//                    e.UserName.Equals(model.UserName, StringComparison.OrdinalIgnoreCase) ||
//                    e.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase));

//                if (existing != null)
//                {
//                    TempData["ErrorMessage"] = "User already exists with the same username or email.";
//                    return View(model);
//                }

//                // ✅ Save the new employee
//                bool isSaved = await apiService.SaveEmployee(model);

//                if (isSaved)
//                {
//                    TempData["SuccessMessage"] = "Registration successful!";
//                    return RedirectToAction("Login");
//                }
//                else
//                {
//                    TempData["ErrorMessage"] = "Failed to register user.";
//                    return View(model);
//                }
//            }
//            catch (Exception ex)
//            {
//                TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
//                return View(model);
//            }
//        }
//    }
//}
