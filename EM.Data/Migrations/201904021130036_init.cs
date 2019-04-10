namespace EM.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        ParticipantId = c.String(nullable: false, maxLength: 128),
                        EventId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Evenements", t => t.EventId)
                .ForeignKey("dbo.Users", t => t.ParticipantId, cascadeDelete: true)
                .Index(t => t.ParticipantId)
                .Index(t => t.EventId);
            
            CreateTable(
                "dbo.Evenements",
                c => new
                    {
                        EventId = c.Int(nullable: false, identity: true),
                        Picture = c.String(),
                        theme = c.String(),
                        location = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.EventId)
                .ForeignKey("dbo.Users", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.Tasks",
                c => new
                    {
                        idTasks = c.Int(nullable: false, identity: true),
                        EventId = c.Int(nullable: false),
                        Id = c.String(maxLength: 128),
                        type = c.String(),
                    })
                .PrimaryKey(t => t.idTasks)
                .ForeignKey("dbo.Evenements", t => t.EventId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.Id)
                .Index(t => t.EventId)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(),
                        TenantId = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        Evenement_EventId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .ForeignKey("dbo.Evenements", t => t.Evenement_EventId)
                .Index(t => t.TenantId)
                .Index(t => t.Evenement_EventId);
            
            CreateTable(
                "dbo.IdentityUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.IdentityUserLogins",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(),
                        ProviderKey = c.String(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.IdentityUserRoles",
                c => new
                    {
                        RoleId = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                        IdentityRole_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.RoleId, t.UserId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.IdentityRoles", t => t.IdentityRole_Id)
                .Index(t => t.UserId)
                .Index(t => t.IdentityRole_Id);
            
            CreateTable(
                "dbo.Tenants",
                c => new
                    {
                        TenantId = c.Int(nullable: false, identity: true),
                        TenantName = c.String(nullable: false, maxLength: 256),
                        Default = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TenantId)
                .Index(t => t.TenantName, unique: true, name: "TenantNameIndex");
            
            CreateTable(
                "dbo.Replies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(nullable: false),
                        CreatedOn = c.DateTime(),
                        ParticipantId = c.Int(nullable: false),
                        CommentId = c.Int(nullable: false),
                        Participant_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Comments", t => t.CommentId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.Participant_Id)
                .Index(t => t.CommentId)
                .Index(t => t.Participant_Id);
            
            CreateTable(
                "dbo.IdentityRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Description = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IdentityUserRoles", "IdentityRole_Id", "dbo.IdentityRoles");
            DropForeignKey("dbo.Users", "Evenement_EventId", "dbo.Evenements");
            DropForeignKey("dbo.Tasks", "Id", "dbo.Users");
            DropForeignKey("dbo.Replies", "Participant_Id", "dbo.Users");
            DropForeignKey("dbo.Replies", "CommentId", "dbo.Comments");
            DropForeignKey("dbo.Comments", "ParticipantId", "dbo.Users");
            DropForeignKey("dbo.Users", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.Evenements", "ApplicationUser_Id", "dbo.Users");
            DropForeignKey("dbo.IdentityUserRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.IdentityUserLogins", "User_Id", "dbo.Users");
            DropForeignKey("dbo.IdentityUserClaims", "UserId", "dbo.Users");
            DropForeignKey("dbo.Tasks", "EventId", "dbo.Evenements");
            DropForeignKey("dbo.Comments", "EventId", "dbo.Evenements");
            DropIndex("dbo.Replies", new[] { "Participant_Id" });
            DropIndex("dbo.Replies", new[] { "CommentId" });
            DropIndex("dbo.Tenants", "TenantNameIndex");
            DropIndex("dbo.IdentityUserRoles", new[] { "IdentityRole_Id" });
            DropIndex("dbo.IdentityUserRoles", new[] { "UserId" });
            DropIndex("dbo.IdentityUserLogins", new[] { "User_Id" });
            DropIndex("dbo.IdentityUserClaims", new[] { "UserId" });
            DropIndex("dbo.Users", new[] { "Evenement_EventId" });
            DropIndex("dbo.Users", new[] { "TenantId" });
            DropIndex("dbo.Tasks", new[] { "Id" });
            DropIndex("dbo.Tasks", new[] { "EventId" });
            DropIndex("dbo.Evenements", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Comments", new[] { "EventId" });
            DropIndex("dbo.Comments", new[] { "ParticipantId" });
            DropTable("dbo.IdentityRoles");
            DropTable("dbo.Replies");
            DropTable("dbo.Tenants");
            DropTable("dbo.IdentityUserRoles");
            DropTable("dbo.IdentityUserLogins");
            DropTable("dbo.IdentityUserClaims");
            DropTable("dbo.Users");
            DropTable("dbo.Tasks");
            DropTable("dbo.Evenements");
            DropTable("dbo.Comments");
        }
    }
}
