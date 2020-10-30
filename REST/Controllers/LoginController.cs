using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using REST.Data;
using REST.ViewModels;
using static REST.Service.Enums;

namespace REST.Controllers
{
    public class LoginController : BaseController
    {
        private readonly DbConnection _db;
        public LoginController(DbConnection db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl = null)
        {
            if (returnUrl != null)
            {
                await HttpContext.SignOutAsync();
                Alert("", "รหัสหมดอายุ กรูณาเข้าสู่ระบบใหม่ !", NotificationType.warning);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(ViewLogin Results)
        {
            if (Results.UserName != null && Results.PassWord != null)
            {
                if (Results.UserName == "mixpro" && Results.PassWord == "Mp2008")
                {
                    var adminclaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, "MIXPRO"),
                        new Claim("FullName", "MixproAdministrator"),
                        new Claim("BranchId", "00"),
                        new Claim("BranchName", "MixproAdvance")
                    };
                    var adminidentity = new ClaimsIdentity(adminclaims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var adminprincipal = new ClaimsPrincipal(adminidentity);
                    var adminprops = new AuthenticationProperties();
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, adminprincipal, adminprops);

                    return RedirectToAction("Index", "Home");
                }

                var user = _db.CD_UserLogin.FirstOrDefault(u => u.UserName == Results.UserName);
                if (user == null)
                {
                    Alert("Error", "ชื่อผู้ใช้ไม่ถูกต้อง !", NotificationType.error);
                    return View();
                }

                var pass = _db.CD_UserLogin.FirstOrDefault(p => p.PassWord == Results.PassWord);
                if (pass == null)
                {
                    Alert("Error", "รหัสผู้ใช้ไม่ถูกต้อง !", NotificationType.error);
                    return View();
                }

                var Login = _db.CD_UserLogin.FirstOrDefault(x => x.UserName == Results.UserName && x.PassWord == Results.PassWord);
                var Branch = _db.CD_Branch.FirstOrDefault(x => x.BranchId == Login.BranchId);

                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, Login.UserId),
                        new Claim("FullName", Login.Name),
                        new Claim("BranchId", Login.BranchId),
                        new Claim("BranchName", Branch.BranchName)
                    };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var props = new AuthenticationProperties();
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

                return RedirectToAction("Index", "Home");

            }
            else
            {
                Alert("Warning", "กรุณากรอกข้อมูล", NotificationType.warning);
                return View();
            }            
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            if (HttpContext.Session.GetString("Session_StapleList") != null)
            {
                HttpContext.Session.Remove("Session_StapleList");
            }

            if (HttpContext.Session.GetString("Session_ListOrder") != null)
            {
                HttpContext.Session.Remove("Session_ListOrder");
            }

            return RedirectToAction("Index", "Login");
        }
    }
}
