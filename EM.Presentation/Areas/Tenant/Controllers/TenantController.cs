using EM.Data;
using EM.Presentation.Areas.Tenant.Helpers;
using EM.Presentation.Areas.Tenant.Models;
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

namespace EM.Presentation.Areas.Tenant.Controllers
{
    public class TenantController : AppController
    {
        IUserService UserService;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;


        public TenantController():base()
        {
            UserService = new UserService();
        }
        public TenantController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager):base()
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
        [TenantAuthorize(Roles = "President,Organizer")]
        public ActionResult Index()
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
                    case RoleNames.ROLE_ADMINISTRATOR: return RedirectToAction("TenantValidation");
                    case RoleNames.ROLE_President: return RedirectToAction("Index");
                    case RoleNames.ROLE_Organizer: return RedirectToAction("TenantValidation");
                    case RoleNames.ROLE_Participant: return RedirectToAction("Home","Home");
                    default: return RedirectToAction("Home", "Home");
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
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            ApplicationUser user = await UserManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var claim = user.Claims.FirstOrDefault(c => c.ClaimType.Equals("Active"));
                if (claim.ClaimValue.Equals("pending"))
                {
                    ModelState.AddModelError("", "Your account is pending validation.");
                    return View(model);
                }
                if (!user.Tenant.TenantName.ToUpper().Equals(model.Tenant.ToUpper()))
                    {
                        ModelState.AddModelError("", "Your account does not exit for this Tenant.");
                        return View(model);
                    }
                if (UserManager.IsInRole(user.Id, RoleNames.ROLE_ADMINISTRATOR))
                {
                    await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
                    return RedirectToAction("TenantValidation", "Tenant");
                }
                else if (!UserManager.IsInRole(user.Id, RoleNames.ROLE_President) && !UserManager.IsInRole(user.Id, RoleNames.ROLE_Organizer)){                   
                    ModelState.AddModelError("", "Your account is not authorized.");
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
                    case RoleNames.ROLE_ADMINISTRATOR: return RedirectToAction("TenantValidation");
                    case RoleNames.ROLE_President: return RedirectToAction("Index");
                    case RoleNames.ROLE_Organizer: return RedirectToAction("Index");
                    case RoleNames.ROLE_Participant: return RedirectToAction("Home", "Home");
                    default: return RedirectToAction("Home", "Home");
                }
            }
            return View();
        }
        

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var tenant = new EM.Domain.Entities.Tenant() { TenantName = model.Tenant };
                    var user = new Data.ApplicationUser { UserName = model.Email, Email = model.Email, Tenant = tenant };
                    var result = await UserManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await UserManager.AddClaimAsync(user.Id, new Claim("Active", "pending"));
                        await UserManager.AddToRoleAsync(user.Id, RoleNames.ROLE_President);
                        //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                        TempData["user"] = user;
                        return RedirectToAction("Registered", "Tenant");
                    }
                    AddErrors(result);
                }
                catch (Exception Ex)
                {
                    ModelState.AddModelError("", "This Tenant is already exiting.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        
        public ActionResult Registered()
        {
             
            ApplicationUser user = TempData["user"] as ApplicationUser;
            if (user != null)
            {
                TenantViewModel user_tenant = new TenantViewModel (){ Email = user.Email, Tenant = user.Tenant.TenantName };    
                return View(user_tenant);
            }
            else
            {
                return RedirectToAction("Register","Tenant");
            }
        }

        [TenantAuthorize(Roles = "Administrator")]
        public ActionResult TenantValidation()
        {
            IEnumerable<ApplicationUser> users = UserService.GetMany(u => u.Claims.All( c => c.ClaimValue.Equals("pending")),null,u => u.Tenant);
            IList<TenantValidationViewModel> tenants = new List<TenantValidationViewModel>();
            foreach (ApplicationUser s in users)
            {
                TenantValidationViewModel tenant = new TenantValidationViewModel();
                tenant.Id = s.Id;
                tenant.Email = s.Email;
                var claim = s.Claims.FirstOrDefault(c => c.ClaimType.Equals("Active"));
                tenant.Status = claim.ClaimValue;
                tenant.Organization = s.Tenant.TenantName;
                tenants.Add(tenant);
            }

            return View(tenants);
        }
        [TenantAuthorize(Roles = "Administrator")]
        public ActionResult TenantActivation(string Id)
        {
            ApplicationUser user = UserService.GetById(Id) as ApplicationUser;
            var claim = user.Claims.FirstOrDefault(c => c.ClaimType.Equals("Active"));
            user.Claims.Remove(claim);
            UserManager.AddClaim(user.Id, new System.Security.Claims.Claim("Active", "actived"));
            UserService.Update(user);
            UserService.Commit();
            return RedirectToAction("TenantValidation");
        }

        [TenantAuthorize(Roles = "Administrator")]
        public ActionResult TenantReject(string Id)
        {
            ApplicationUser user = UserService.GetById(Id) as ApplicationUser;
            UserService.Delete(user);
            UserService.Commit();
            return RedirectToAction("TenantValidation");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Home", "Home", new { area = "" });
        }

        [TenantAuthorize(Roles = "President,Organizer")]
        public ActionResult UserProfile()
        {
            ViewData["TenantName"] = current_tenant.TenantName;
            //ViewData["bool"] = "false";
            return View();
        }

        [HttpPost]
        [TenantAuthorize(Roles = "President,Organizer")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserProfile(ChangePasswordTenantViewModel model)
        {
            /*ApplicationUser user_profile = await UserManager.FindByIdAsync(user.UserId);
            user_profile.UserName = user.UserName;
            UserManager.Update(user_profile);
            return RedirectToAction("UserProfile", "Tenant");*/

            ViewData["TenantName"] = current_tenant.TenantName;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                TempData["password"] = "true";
                return RedirectToAction("UserProfile");
            }

            ModelState.AddModelError("", "Your current password is wrong.");
            return View(model);

        }

        [HttpPost]
        [TenantAuthorize(Roles = "President,Organizer")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrganizer(RegisterOrganizerViewModel model)
        {
            if (ModelState.IsValid)
            {
                    var user = new Data.ApplicationUser { UserName = model.Email, Email = model.Email, Tenant = current_tenant };
                    var result = await UserManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await UserManager.AddClaimAsync(user.Id, new Claim("Active", "actived"));
                        await UserManager.AddToRoleAsync(user.Id, RoleNames.ROLE_Organizer);
                        TempData["organizer"] = "true";
                        return RedirectToAction("UserProfile");
                }
                    AddErrors(result);
                
            }
            // If we got this far, something failed, redisplay form
            return PartialView(model);
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
            return RedirectToAction("Index", "Tenant");
        }

        /* ************************************************************************************************************************/
    }
}
