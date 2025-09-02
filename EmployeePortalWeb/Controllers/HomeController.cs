

using Azure.Identity;
using EmployeeModelPackage;
using EmployeePortalWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using NPOI.XWPF.UserModel;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using Aes = System.Runtime.Intrinsics.Arm.Aes;
using System;
using System.IO;


namespace EmployeePortalWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly string rootPath;
        private readonly ApiService apiService;

        public HomeController(IWebHostEnvironment webHost, IHttpContextAccessor httpContextAccessor)
        {
            rootPath = webHost.ContentRootPath;
            apiService = new ApiService();
        }




        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }




        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var random = new Random();
            var verificationCode = random.Next(100000, 999999).ToString();
            HttpContext.Session.SetSessionData("VerificationCode", verificationCode);

            string body = $"Your OTP code is {verificationCode}";
            string subject = $"Re: Your OTP Details";

            //Send OTP To Email
            SendEmail(model.Email, subject, body);
            HttpContext.Session.SetSessionData("UserData", model);
            return RedirectToAction("VerifyOTP");
        }



        [HttpGet]
        public IActionResult VerifyOTP()
        {
            return View();
        }





        [HttpPost]
        public IActionResult VerifyOTP(string OTPCode)
        {
            var otp = HttpContext.Session.GetSessionData<string>("VerificationCode");

            if (otp == OTPCode)
            {
                var memberData = HttpContext.Session.GetSessionData<RegisterModel>("UserData");

                if (memberData == null)
                {

                    return RedirectToAction("Register");
                }

                var random = new Random();
                const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()_-+=<>?";

                string generatedPassword = new string(
                    Enumerable.Repeat(chars, 10)
                              .Select(s => s[random.Next(s.Length)])
                              .ToArray()
                );



                string hashedPassword = EncryptStringAES(generatedPassword, "authToken");

                memberData.Password = hashedPassword;


                var result = apiService.SaveEmployee(memberData);


                if (result!=null)
                {
                    string body = $"Your Password is {generatedPassword}\nDo not share it with anyone!";
                    string subject = "Re: Your OTP Details";

                    SendEmail(memberData.Email, subject, body);

                    return RedirectToAction("Login");
    }
                else
                {
                    return RedirectToAction("Register");
}
            }


            ModelState.AddModelError("", "Invalid OTP");
                   return View();
        }



        public string EncryptStringAES(string plainText, string key)
        {
            using (System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
                aes.IV = new byte[16]; 

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                return Convert.ToBase64String(cipherBytes);
            }
        }




  [HttpGet]
public IActionResult Login()
{
    return View();
}



[HttpPost]
public async Task<IActionResult> Login(RegisterModel loginDetails)
{


    var login = await apiService.GetRegistrationDetails(loginDetails);

    if (login != null)
    {
        return RedirectToAction("List", "Employee");
    }

    ModelState.AddModelError(string.Empty, "Invalid username or password");
    return View(loginDetails);
}







[HttpGet]
public IActionResult JsonResult()
{
    return View();
}




[HttpGet]
public JsonResult IsUserExist(string Username)
{
    try
    {
        var res = apiService.IsUserExists(Username);
        return new JsonResult(res.Result);
    }

    catch (Exception ex)
    {
        return new JsonResult(false);
    }
}



private async Task<bool> RegisterDetails(RegisterModel model)
{

    await Task.Delay(100);


    if (model.UserName != null && model.Password != null)
    {
        return true;
    }

    return false;
}





private void SendEmail(string toEmail, string subject, string body)
{
    var smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com")
    {
        Port = 587,
        Credentials = new System.Net.NetworkCredential("adminsupport@intellectinfo.com", "kkefbmmzfhdxzsqg"),
        EnableSsl = true,
    };

    var mailMessage = new MailMessage
    {
        From = new MailAddress("adminsupport@intellectinfo.com"),
        Subject = subject,
        Body = body,
        IsBodyHtml = false,
    };

    mailMessage.To.Add(toEmail);

    smtpClient.Send(mailMessage);
}



[HttpGet]
public async Task<IActionResult> List(EmployeeModel loginDetails)
{
    var details = await apiService.GetEmployeeList();
    return View(details);
}

    }
}








