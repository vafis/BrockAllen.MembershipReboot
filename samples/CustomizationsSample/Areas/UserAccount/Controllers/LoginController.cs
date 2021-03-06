﻿using BrockAllen.MembershipReboot.Mvc.Areas.UserAccount.Models;
using System.Web.Mvc;

namespace BrockAllen.MembershipReboot.Mvc.Areas.UserAccount.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        UserAccountService userAccountService;
        AuthenticationService authSvc;

        public LoginController(
            UserAccountService userService, 
            AuthenticationService authSvc)
        {
            this.userAccountService = userService;
            this.authSvc = authSvc;
        }

        public ActionResult Index()
        {
            return View(new LoginInputModel());
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                BrockAllen.MembershipReboot.UserAccount account;
                if (userAccountService.AuthenticateWithUsernameOrEmail(model.Username, model.Password, out account))
                {
                    authSvc.SignIn(account);

                    if (userAccountService.IsPasswordExpired(account))
                    {
                        return RedirectToAction("Index", "ChangePassword");
                    }
                    else
                    {
                        if (Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Username or Password");
                }
            }

            return View(model);
        }
    }
}
