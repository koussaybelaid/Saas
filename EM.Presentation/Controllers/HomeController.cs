using EM.Data;
using EM.Presentation.Areas.Tenant;
using EM.Presentation.Areas.Tenant.Models;
using EM.Presentation.Models;
using EM.Service.UserService;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
namespace EM.Presentation.Controllers
{
    public class HomeController : AppController
    {
        IUserService UserService;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;


        public HomeController():base()
        {
            UserService = new UserService();
        }
        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager):base()
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
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

        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        // GET: Backend
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Home()
        {
            return View();
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            bool user = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (user)
            {
                ApplicationUser current_user = UserManager.FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                string role = UserManager.GetRoles(current_user.Id).FirstOrDefault();
                switch (role)
                {
                    case RoleNames.ROLE_ADMINISTRATOR: return RedirectToAction("TenantValidation","Tenant", new { area = "Tenant" });
                    case RoleNames.ROLE_President: return RedirectToAction("Home");
                    case RoleNames.ROLE_Organizer: return RedirectToAction("Home");
                    case RoleNames.ROLE_Participant: return RedirectToAction("Home");
                    default: return RedirectToAction("Home");
                }
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(Models.LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser user = UserManager.FindByEmail(model.Email);
            if (user != null)
            {
                if (UserManager.IsInRole(user.Id, RoleNames.ROLE_Participant))
                {
                    await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
                    return RedirectToAction("Home", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
                }
            }
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            bool user = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (user)
            {
                ApplicationUser current_user = UserManager.FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                string role = UserManager.GetRoles(current_user.Id).FirstOrDefault();
                switch (role)
                {
                    case RoleNames.ROLE_ADMINISTRATOR: return RedirectToAction("TenantValidation", "Tenant", new { area = "Tenant" });
                    case RoleNames.ROLE_President: return RedirectToAction("Home");
                    case RoleNames.ROLE_Organizer: return RedirectToAction("Home");
                    case RoleNames.ROLE_Participant: return RedirectToAction("Home");
                    default: return RedirectToAction("Home");
                }
            }
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(Models.RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Data.ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await UserManager.AddClaimAsync(user.Id, new Claim("Active", "actived"));
                    await UserManager.AddToRoleAsync(user.Id, RoleNames.ROLE_Participant);
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    TempData["user"] = user;
                    return RedirectToAction("Registered");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        public ActionResult Registered()
        {

            ApplicationUser user = TempData["user"] as ApplicationUser;
            if (user != null)
            {
                ViewData["Email"] = user.Email;
                return View();
            }
            else
            {
                return RedirectToAction("Register");
            }
        }

        /* ********************************************************************************************************************/
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
            return RedirectToAction("Home");
        }

        /* ************************************************************************************************************************/
    }
}                                                                                                                                