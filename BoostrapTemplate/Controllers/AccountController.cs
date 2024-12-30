using BoostrapTemplate.ViewModels.Login;
using BoostrapTemplate.ViewModels.Signup;
using DATA.Interfaces;
using DATA.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace BoostrapTemplate.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccountController : Controller
    {
        private readonly IUser _user;
        private readonly IWebHostEnvironment _environment;

        public AccountController(IUser user, IWebHostEnvironment environment)
        {
            _user = user;
            _environment = environment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult LoginPage()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LoginPage(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = _user.getbyName(model.Username);
                    if (data != null)
                    {
                        bool b = (model.Username == data.UserName && model.Password == DecryptPassword(data.Password));
                        if (b)
                        {
                            bool isAuthentication = false;
                            ClaimsIdentity identity = null;
                            if (data.UserName == "Admin")
                            {
                                identity = new ClaimsIdentity(new[] {
                                    new Claim(ClaimTypes.Name, data.UserName),//For Claim Identity
                                    new Claim(ClaimTypes.Role,"Admin")//For Role Based Authentication
                                }, CookieAuthenticationDefaults.AuthenticationScheme);

                                HttpContext.Session.SetString("Username", data.UserName);
                                isAuthentication = true;

                            }
                            else
                            {
                                identity = new ClaimsIdentity(new[] {
                                    new Claim(ClaimTypes.Name, data.UserName),//For Claim Identity
                                    new Claim(ClaimTypes.Role,"User")//For Role Based Authentication
                                }, CookieAuthenticationDefaults.AuthenticationScheme);

                                HttpContext.Session.SetString("Username", data.UserName);
                                isAuthentication = true;

                            }

                            if (isAuthentication)
                            {
                                var principle = new ClaimsPrincipal(identity);
                                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principle);

                                //string str = HttpContext.Session.GetString("Username");
                                return RedirectToAction("Welcome", "DataVisulization");
                            }

                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Enter Valid credentials");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Record not found,contact to admin.");
                    }

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.ToString());
                }


            }
            else
            {
                ModelState.AddModelError("", "Enter Creadentials");
            }
            return View();
        }
        [AllowAnonymous]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var allCookies = Request.Cookies.Keys;
            foreach (var cookie in allCookies)
            {
                Response.Cookies.Delete(cookie);
            }
            return RedirectToAction("LoginPage");
        }

        [HttpGet]
        public IActionResult Signup()
        {
            ViewData["Title"] = "User List";
            var data = _user.Getall();

            var final = data.Select(x => new Signup_Create_ViewModel
            {
                Id = x.Id,
                UserName = x.UserName,
                Email = x.Email,
                Mobile = x.Mobile,
                Password = x.Password,
                Image = x.Image
            });

            var model = new Signup_List_ViewModel()
            {
                LstSignup = final
            };

            return View(model);

        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Title"] = "User Add";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Signup_Create_ViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = uploadImg(model.ImagePath);
                DateTime newDateTime = Convert.ToDateTime(model.CreatedDate).Add(model.Createdtime);
                var obj = new Users()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Mobile = model.Mobile,
                    Password = Encrypted(model.Password),
                    Image = uniqueFileName,
                    Isactive = model.Isactive,
                    CreatedDate = newDateTime
                };
                _user.Add(obj);
                TempData["message"] = "Record Save successfully";
                return RedirectToAction("Signup");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Enter Valid detials");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult edit(int id)
        {
            var data = _user.getbyid(id);

           
            if (data != null)
            {
                var month = Convert.ToDateTime(data.CreatedDate).Month.ToString().Length == 1 ? "0" + Convert.ToDateTime(data.CreatedDate).Month.ToString() : Convert.ToDateTime(data.CreatedDate).Month.ToString();
                var day = Convert.ToDateTime(data.CreatedDate).Day.ToString().Length == 1 ? "0" + Convert.ToDateTime(data.CreatedDate).Day.ToString() : Convert.ToDateTime(data.CreatedDate).Day.ToString();
                var year = Convert.ToDateTime(data.CreatedDate).Year;
                try
                {
                    var model = new Signup_Edit_ViewModel()
                    {
                        Id = data.Id,
                        UserName = data.UserName,
                        Email = data.Email,
                        Mobile = data.Mobile,
                        Image = data.Image,
                        Isactive = data.Isactive,
                        CreatedDate =year+"-"+ month+"-"+day,
                        Createdtime =Convert.ToDateTime(data.CreatedDate).TimeOfDay
                    };
                    return View(model);
                }
                catch (Exception ex)
                {
                    TempData["message"] = "Something went wrong, " + ex.InnerException.Message;
                    return RedirectToAction("Signup", "account");
                }   
            }
            return RedirectToAction("Signup");
        }
        [HttpPost]
        public IActionResult edit(Signup_Edit_ViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string uniquefileName = "";
                    var data = _user.getbyid(model.Id);
                    DateTime? newDateTime = Convert.ToDateTime(model.CreatedDate).Add(model.Createdtime);
                    if (model.ImagePath != null)
                    {
                        if (data.Image != null)
                        {
                            string filepath = Path.Combine(_environment.WebRootPath, "images/", data.Image);
                            if (System.IO.File.Exists(filepath))
                            {
                                System.IO.File.Delete(filepath);
                            }
                        }
                        uniquefileName = uploadImg(model.ImagePath);
                    }

                    data.UserName = model.UserName;
                    data.Email = model.Email;
                    data.Mobile = model.Mobile;
                    data.Password = Encrypted(model.Password);
                    if (model.ImagePath != null)
                    {
                        data.Image = uniquefileName;
                    }
                    data.Isactive = model.Isactive;
                    data.CreatedDate = newDateTime;
                    _user.Edit(data);
                    TempData["message"] = "Record Update successfully";
                    return RedirectToAction("Signup");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "Something went wrong, "+ ex.InnerException.Message;
                return RedirectToAction("Signup", "account");              
                //ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View(model);

        }
        public IActionResult delete(int id)
        {
            if (id != 0)
            {
                var data = _user.getbyid(id);
                if (data.Image != null)
                {
                    string filepath = Path.Combine(_environment.WebRootPath, "images/", data.Image);
                    if (System.IO.File.Exists(filepath))
                    {
                        System.IO.File.Delete(filepath);
                    }

                    _user.Delete(data);
                    TempData["message"] = "Record Deleted successfully";
                    return RedirectToAction("Signup");
                }
            }
            return NotFound();
        }
        private string uploadImg(IFormFile ImagePath)
        {
            string uniuqName = string.Empty;
            if (ImagePath != null)
            {
                string uploadfolder = Path.Combine(_environment.WebRootPath, "images/");
                uniuqName = Guid.NewGuid().ToString() + "_" + ImagePath.FileName;
                string filepath = Path.Combine(uploadfolder, uniuqName);
                using (var filestream = new FileStream(filepath, FileMode.Create))
                {
                    ImagePath.CopyTo(filestream);
                }
            }
            return uniuqName;
        }
        private static string Encrypted(string Pwd)
        {
            if (string.IsNullOrEmpty(Pwd))
            {
                return null;
            }
            else
            {
                byte[] storePass = ASCIIEncoding.ASCII.GetBytes(Pwd);
                string EncryptPass = Convert.ToBase64String(storePass);
                return EncryptPass;
            }
        }
        private static string DecryptPassword(string pwd)
        {
            if (string.IsNullOrEmpty(pwd))
            {
                return null;
            }
            else
            {
                byte[] StorePass = Convert.FromBase64String(pwd);
                string DecryptPass = ASCIIEncoding.ASCII.GetString(StorePass);
                return DecryptPass;
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
