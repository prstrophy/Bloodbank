using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BloodBank.Models;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Collections.Generic;

namespace BloodBank.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        BloodBankDbContext _context = new BloodBankDbContext();
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            Request.Cookies.Clear();

            var c = new HttpCookie("UserID");

            c.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(c);

            var d = new HttpCookie("Email");

            d.Expires = DateTime.Now.AddDays(-1);

            var e = new HttpCookie("UserType");

            e.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(d);

            return View();
        }

        //MD% generator
        public static string MD5Generator(string password)
        {
            var md5 = MD5.Create();
            var result = md5.ComputeHash(Encoding.ASCII.GetBytes(password));
            var hash = md5.ComputeHash(result);

            var sb = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                string haspass = MD5Generator(model.Password).ToString();
                var user = _context.users.FirstOrDefault(x => x.Email == model.Email && x.Password == haspass);
                if (user != null)
                {
                    if (user.EmailConfirmed == false)
                    {
                        ViewData["NotConfirm"] = "You must confirm your Email before Login.";
                        return View(model);
                    }
                    Response.Cookies["UserID"].Value = user.Id.ToString();
                    //Response.Cookies["LoggedInUserID"].Expires = DateTime.Now.AddDays(1);
                    Response.Cookies["Email"].Value = user.Email;
                    //Response.Cookies["Email"].Expires = DateTime.Now.AddDays(1);
                    Response.Cookies["UserType"].Value = user.UserType;


                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
            }
            ViewData["Message"] = "Invalid UserName OR Password.";
            return View(model);
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.BloodGroupId = new SelectList(_context.BloodGroup.ToList(), "Id", "Description");
            ViewBag.DistrictId = new SelectList(_context.Districts.ToList(), "Id", "Description");
            ViewBag.CityId = new SelectList(_context.City.ToList(), "Id", "Description");

            List<SelectListItem> userType = new List<SelectListItem>();
            userType.Add(new SelectListItem
            {
                Text = "Donor",
                Value = "Donor",
                Selected = true
            });
            userType.Add(new SelectListItem
            {
                Text = "Recipient",
                Value = "Recipient",

            });

            ViewBag.UserType = userType;
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            ViewBag.BloodGroupId = new SelectList(_context.BloodGroup.ToList(), "Id", "Description");
            ViewBag.DistrictId = new SelectList(_context.Districts.ToList(), "Id", "Description");
            ViewBag.CityId = new SelectList(_context.City.ToList(), "Id", "Description");


            MailMessage mail = new MailMessage();
            mail.To.Add(model.Email);
            mail.From = new MailAddress("stropher@gmail.com", "BloodBank.com.np", Encoding.UTF8);
            mail.Subject = "ACCOUNT ACTIVATION";
            mail.SubjectEncoding = Encoding.UTF8;

            var code = Guid.NewGuid().ToString();
            var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account",
             new { email = model.Email, code = code, }, protocol: Request.Url.Scheme);
            string Body = "Hello" + " " + model.FirstName + " " +
               "Please confirm your account by clicking this link:<a href=\"" + callbackUrl + "\">link</a>";
            mail.BodyEncoding = Encoding.UTF8;
            mail.Priority = MailPriority.High;
            mail.Body = Body;
            mail.IsBodyHtml = true;
            ViewData["RegMess"] = "Account Confirmation link has been sent to your Email.";

            var pass = MD5Generator(model.Password);
            Users user = new Users()
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmed = false,
                Password = pass,
                EmailConfirmationCode = code,
                DistrictId = model.DistrictId,
                CityId = model.CityId,
                Address = model.Address,
                Gender = model.Gender,
                Age = model.Age,
                Number = model.Number,
                BloodGroupId = model.BloodGroupId,
                UserType = model.UserType
            };
            _context.users.Add(user);
            await _context.SaveChangesAsync();
            using (SmtpClient smtp = new SmtpClient())
            {
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("aryan.silwal724@gmail.com", "$Baithakkotha!;)");// Enter seders User name and password
                smtp.EnableSsl = true;
                smtp.Send(mail);
                mail.Dispose();
            }
            ModelState.Clear();
            return RedirectToAction("Login", "Account");


        }

        //
        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ConfirmEmail(string email, string code)
        {
            if (email == null || code == null)
            {
                return View("Error");
            }
            var result = _context.users.FirstOrDefault(x => x.Email == email && x.EmailConfirmationCode == code);
            if (result.EmailConfirmed == true)
            {
                return View("Error");
            }
            else if (result.EmailConfirmationCode == code && result.Email == email)
            {
                result.EmailConfirmed = true;
                _context.Entry(result).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
            }
            return View("ConfirmEmail");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await UserManager.FindByNameAsync(model.Email);
        //        if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
        //        {
        //            // Don't reveal that the user does not exist or is not confirmed
        //            return View("ForgotPasswordConfirmation");
        //        }

        //        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
        //        // Send an email with this link
        //        // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        //        // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
        //        // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
        //        // return RedirectToAction("ForgotPasswordConfirmation", "Account");
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (ModelState.IsValid)
                {
                    var user = _context.users.FirstOrDefault(x => x.Email == model.Email);
                    if (user == null || user.EmailConfirmed == false)
                    {
                        ViewData["resPassMess"] = "Please check your email to reset your password.";
                        ModelState.Clear();

                        return View("ForgotPassword");
                    }
                    MailMessage mail = new MailMessage();
                    mail.To.Add(model.Email);
                    mail.From = new MailAddress("stropher@gmail.com", "BloodBank.com.np", Encoding.UTF8);
                    mail.Subject = "Password Reset";
                    mail.SubjectEncoding = Encoding.UTF8;
                    var code = Guid.NewGuid().ToString();
                    var callbackUrl = Url.Action(nameof(ResetPassword), "Account",
                     new { email = model.Email, code = code, }, protocol: Request.Url.Scheme);
                    string Body = "Hello" + " " + model.Email + " " +
                       " Click to Reset Password :<a href=\"" + callbackUrl + "\">link</a>";
                    mail.BodyEncoding = Encoding.UTF8;
                    mail.Priority = MailPriority.High;
                    mail.Body = Body;
                    mail.IsBodyHtml = true;
                    ViewData["resPassMess"] = "Please check your email to reset your password.";
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("aryan.silwal724@gmail.com", "$Baithakkotha!;)");// Enter seders User name and password
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                        mail.Dispose();
                    }
                    ModelState.Clear();
                    return RedirectToAction("Login", "Account");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var currentUser = _context.users.FirstOrDefault(x => x.Email == model.Email);
            if (currentUser == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            else
            {
                currentUser.Password = MD5Generator(model.Password);
                _context.Entry(currentUser).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Request.Cookies.Clear();

            //var c = new HttpCookie("UserID");

            //c.Expires = DateTime.Now.AddDays(-1);
            //Response.Cookies.Add(c);

            //var d = new HttpCookie("Email");

            //d.Expires = DateTime.Now.AddDays(-1);
            //Response.Cookies.Add(d);
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}