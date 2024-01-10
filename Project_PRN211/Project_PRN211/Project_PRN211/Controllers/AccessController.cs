using Microsoft.AspNetCore.Mvc;

using Project_PRN211.Models;

namespace Project_PRN211.Controllers
{
    public class AccessController : Controller
    {
        WishContext db = new WishContext();
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("User") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");   
            }
            
        }
        [HttpPost]
        public IActionResult Login(Account account)
        {
            if (HttpContext.Session.GetString("User") == null)
            {
                var user = db.Accounts.FirstOrDefault(x => x.User.Equals(account.User));

                if (user != null && BCrypt.Net.BCrypt.Verify(account.Pass, user.Pass))
                {
                    HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString());

                    if (user.IsAdmin == 1)
                    {
                        HttpContext.Session.SetString("User", user.User.ToString());
                        HttpContext.Session.SetInt32("UId", user.UId);
                        return RedirectToAction("DanhMucSanPham", "admin");
                    }
                    HttpContext.Session.SetInt32("UId", user.UId);
                    HttpContext.Session.SetString("User", user.User.ToString());
                    return RedirectToAction("Index", "Home");
                }
                TempData["Message"] = "Account or password is not correct!!";
                return View();
            }

            TempData["Message"] = "Account or password is not correct!!";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("UserId"); ;
            return RedirectToAction("Login", "Access");
        }
        [Route("DangKyTaiKhoan")]
        [HttpGet]
        public IActionResult DangKyTaiKhoan()
        {
            return View();
        }

        [Route("DangKyTaiKhoan")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangKyTaiKhoan(Account account, string User, string Repass)
        {
            if (ModelState.IsValid)
            {
                if (account.Pass != Repass)
                {
                    ModelState.AddModelError("ErrorResister", "Password and Repass do not match");
                    return View(account);
                }
                if (db.Accounts.Any(u => u.User == account.User))
                {
                    ModelState.AddModelError("ErrorResister", "This account already exists");
                    return View(account);
                }
                if(db.Accounts.Any(u=>u.Gmail == account.Gmail))
                {
                    ModelState.AddModelError("ErrorResister", "This Email already exists");
                    return View(account);
                }

                // Hash the password using BCrypt
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(account.Pass);

                // Assign the hashed password back to the account object
                account.Pass = passwordHash;

                // Save the account to the database
                db.Accounts.Add(account);
                db.SaveChanges();

                ModelState.AddModelError("SuccessResister", "SignUp Success");
                return RedirectToAction("Login", "Access");
            }
            return View(account);
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }
        /*[HttpPost]
        public ActionResult ForgotPassword(string EmailID) 
        {
            //verify Email ID
            //Generate Reset password link
            //send Email
            string message = "";
            bool status = false;
            using (WishContext dc = new WishContext())
            {
                var account = dc.Accounts.Where(a=>a.Gmail==EmailID).FirstOrDefault();
                if (account != null)
                {
                    //send email for reset password
                    string resetCode = Guid.NewGuid().ToString();   

                }else
                {
                    message = "Account not found";
                }
            }
            return View();
        }*/

    }
}
