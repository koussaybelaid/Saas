using EM.Data;
using EM.Domain.Entities;
using EM.Presentation.Areas.Tenant;
using EM.Service.UserService;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EM.Presentation.Controllers
{
    public class HowToController : AppController
    {

        /***** Please pay attention ******/
        /* ----------------------------------------- Controller Part --------------------------------------
         1- Always Use AppController when creating a new controller because it's containing the TenantId to make the isolation of data
         2- All the relationships between entites and user or tenant should be  specified with Tenant in EM.Domain.Entities or EM.Data.ApplicationUser
         3- To get the current user "ApplicationUser current_user = UserManager.FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());" 
         4- To get the current tenant because every user is assigned to a tenant "Current_Tenant.TenantId.ToString();"  
         5- To get the current user role "UserManager.GetRoles(current_user.Id).FirstOrDefault();" 
         6- All role are in the class Tenant View Models in Area.Tenant there is 4 roles {Administrator,President,Organizer and Participant}
         7- To check if a user is in role "UserManager.IsInRole(user.Id, RoleNames.{ Here you specify the role } !! Just click on ctrl +space !!)
         8- Every Tenant Create Should specify Email,Password and TenantName(Unique), his account should be validated by the administrator
         9- Administrator Credentials { Email = admin@acloudguru.com, Password= Azerty@12, Tenant = Admin }
         10- When logging as Administrator your will redirected to a dashboard in order to validate tenants (only president is validated)
         11- While creating a tenant (Register as a Tenant), only a user is created with the role president and assigned to the tenant entered in the form
         12- For the (Register), a user is created with the role participant and he is assigned to any tenant,
             so the variable Current_Tenant is null
         
            --------------------------------------Views Part-----------------------------------------------------
         1- All the views should inherit from the _CustomLayout.cshtml
         2- For Backend Tenant Side, all the views should inhered from _Tenant.cshtml in the namespace Area.Tenant.Shared._Tenant
         3- Regarding the Backend Tenant Side, there is sections required  :
         for example: In the UserProfile.cshtml with inheret from _Tenant I put My Profile as a Title
         @section page {
                My Profile
         }
         So Always Start with
          @section page {
                Here put Your Title View
         }

         In order to add your left nav bar in the Backed Tenant Side, go Area.Tenant.Views.Shared._Tenant and add this under MyProfile div
         for example:
         <li class="nav-item @RenderSection("active_myprofile",required: false)">
                        <a class="nav-link" href="@Url.Action("YourAction","Tenant")">
                            <i class="material-icons">person</i>
                            <p>Your Link title</p>
                        </a>
         </li>

        To add scripts to your views follow this example, add this part at the end of your views
        for exmaple:

        @section scripts {
                -------------- This is and example -----------------------------------
        <script src="@Url.Content("~/Content/Backend/Dashboard/js/core/jquery.min.js")"></script>
        }
        PS: Add scripts in the Content folder before, for Backend Tenant Side put them in the Content/Backend/Login folder and for the frontend
        put them in Content/Backend, use adding "existing item" by right click.

        Take this controller as a reference.
        Regarding Templates, I sent you two template, one for the backend and the other for the front, open them in the browser and pick up
        the html component that you would add to your views

        Go below to see the Index Action how I user some variables and how they are displayed in the view
               
        */
        /*************************************************/

        //This is the UserService you can get all the users here, check the Repository Patterns Methods
        IUserService UserService;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private Tenant _tenant;

        public HowToController()
        {
            UserService = new UserService();
        }
        public HowToController(ApplicationUserManager userManager, ApplicationRoleManager roleManager):base()
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public Tenant Current_Tenant
        {
            get
            {
                return _tenant ?? base.current_tenant;
            }
            private set
            {
                _tenant = value;
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

        /********* Actions Starts Heres *******/

        [AllowAnonymous]
        public ActionResult Index()
        {
            ApplicationUser current_user = UserManager.FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            ViewData["Tenant"] = Current_Tenant.TenantId.ToString();
            ViewData["Logged_User"] = current_user.Email;
            ViewData["UserRole"] = UserManager.GetRoles(current_user.Id).FirstOrDefault();
            return View();
        }
    }
}