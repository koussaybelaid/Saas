using EM.Data;
using EM.Presentation.Areas.Tenant.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EM.Presentation.Areas.Tenant.Helpers
{
    public class IdentityHelper
    {
        public static EM.Domain.Entities.Tenant GetTenantBasedOnUser(ApplicationUser user)
        {
            EM.Domain.Entities.Tenant tenant;
            tenant = user.Tenant;
            return tenant;
        }
        internal static void SeedIdentities(MyContext context)
	    {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(context));
                if (!roleManager.RoleExists(RoleNames.ROLE_ADMINISTRATOR))
                    {
                        var roleresult = roleManager.Create(new ApplicationRole(RoleNames.ROLE_ADMINISTRATOR));
                    }
                if (!roleManager.RoleExists(RoleNames.ROLE_President))
                    {
                        var roleresult = roleManager.Create(new ApplicationRole(RoleNames.ROLE_President));
                    }
                if (!roleManager.RoleExists(RoleNames.ROLE_Organizer))
                    {
                        var roleresult = roleManager.Create(new ApplicationRole(RoleNames.ROLE_Organizer));
                    }
                if (!roleManager.RoleExists(RoleNames.ROLE_Participant))
                    {
                        var roleresult = roleManager.Create(new ApplicationRole(RoleNames.ROLE_Participant));
                    }
            string userName = "admin@acloudguru.com";
            string password = "Azerty@12";

            ApplicationUser user = userManager.FindByName(userName);
                 if (user == null)
                  {
                EM.Domain.Entities.Tenant tenant = new EM.Domain.Entities.Tenant { TenantName = "Admin" };
                user = new ApplicationUser()
                {
                    UserName = userName,
                    Email = userName,
                    EmailConfirmed = true,
                    Tenant = tenant,
                        };
                
            IdentityResult userResult = userManager.Create(user, password);
                    if (userResult.Succeeded)
                    {
                        userManager.AddClaim(user.Id, new System.Security.Claims.Claim("Active","actived"));
                        var result = userManager.AddToRole(user.Id, RoleNames.ROLE_ADMINISTRATOR);
                    }
                  }
        }
    }
}