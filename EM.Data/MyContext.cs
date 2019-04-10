using EM.Domain.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using EM.Data.Configurations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Data
{
    public class MyContext : IdentityDbContext<ApplicationUser>
    {
        public MyContext():base("EM_DB")
        {

        }

        public static MyContext Create()
        {
            return new MyContext();
        }

        static MyContext()
        {
            Database.SetInitializer<MyContext>(null);
        }

        // dbset
        public DbSet<Evenement> Evenements { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<Participant> Participants { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Configurations.Add(new IdentityUserLoginConfiguration());
            modelBuilder.Configurations.Add(new IdentityUserRoleConfiguration());
            var tenant = modelBuilder.Entity<Tenant>();
            tenant.Property(u => u.TenantName)
                .IsRequired()
                .HasMaxLength(256)
            .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("TenantNameIndex") { IsUnique = true, Order = 1 }));
            //modelBuilder.Conventions.Add();
        }


    }
}
