using EM.Data;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Data.Entity;

namespace EM.Presentation.Models
{
    public class ApplicationUserStore<TUser> : UserStore<TUser> where TUser : ApplicationUser
    {
        public ApplicationUserStore(MyContext context) : base(context)
        {
        }
        public int? TenantId { get; set; }

        public override Task CreateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (this.TenantId.HasValue)
            {
                user.TenantId = this.TenantId;
            }            
            return base.CreateAsync(user);
        }

        public override Task<TUser> FindByEmailAsync(string Email)
        {
            if (this.TenantId.HasValue)
            {
                return this.GetUserAggregateAsync(u => u.Email.ToUpper() == Email.ToUpper() && u.TenantId == this.TenantId);
            }
            else
            {
                return this.GetUserAggregateAsync(u => u.Email.ToUpper() == Email.ToUpper());
            }
            
        }

        public override Task<TUser> FindByIdAsync(string Id)
        {
            if (this.TenantId.HasValue)
            {
                return this.GetUserAggregateAsync(u => u.Id == Id && u.TenantId == this.TenantId);
            }
            else
            {
                return this.GetUserAggregateAsync(u => u.Id == Id);
            }

            
        }

        protected override Task<TUser> GetUserAggregateAsync(Expression<Func<TUser, bool>> filter)
        {
            return Users.Include(u => u.Roles)
                .Include(u => u.Claims)
                .Include(u => u.Logins)
                .Include(u => u.Tenant)
                .FirstOrDefaultAsync(filter);
        }

        /*public override Task<TUser> FindByNameAsync(string Org)
        {
            return this.GetUserAggregateAsync(u => u.Tenant.Organization == Org && u.TenantId == this.TenantId);
        }*/

    }
}