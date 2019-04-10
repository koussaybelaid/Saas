namespace EM.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class a1 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Users", newName: "IdentityUsers");
            DropForeignKey("dbo.IdentityUserRoles", "IdentityRole_Id", "dbo.IdentityRoles");
            DropIndex("dbo.IdentityUserRoles", new[] { "IdentityRole_Id" });
            DropColumn("dbo.IdentityUserRoles", "RoleId");
            RenameColumn(table: "dbo.IdentityUserClaims", name: "UserId", newName: "User_Id");
            RenameColumn(table: "dbo.IdentityUserRoles", name: "IdentityRole_Id", newName: "RoleId");
            RenameIndex(table: "dbo.IdentityUserClaims", name: "IX_UserId", newName: "IX_User_Id");
            DropPrimaryKey("dbo.IdentityUserRoles");
            AlterColumn("dbo.IdentityUserRoles", "RoleId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.IdentityUserRoles", new[] { "RoleId", "UserId" });
            CreateIndex("dbo.IdentityUserRoles", "RoleId");
            AddForeignKey("dbo.IdentityUserRoles", "RoleId", "dbo.IdentityRoles", "Id", cascadeDelete: true);
            DropColumn("dbo.IdentityUsers", "Email");
            DropColumn("dbo.IdentityUsers", "EmailConfirmed");
            DropColumn("dbo.IdentityUsers", "PhoneNumber");
            DropColumn("dbo.IdentityUsers", "PhoneNumberConfirmed");
            DropColumn("dbo.IdentityUsers", "TwoFactorEnabled");
            DropColumn("dbo.IdentityUsers", "LockoutEndDateUtc");
            DropColumn("dbo.IdentityUsers", "LockoutEnabled");
            DropColumn("dbo.IdentityUsers", "AccessFailedCount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.IdentityUsers", "AccessFailedCount", c => c.Int(nullable: false));
            AddColumn("dbo.IdentityUsers", "LockoutEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.IdentityUsers", "LockoutEndDateUtc", c => c.DateTime());
            AddColumn("dbo.IdentityUsers", "TwoFactorEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.IdentityUsers", "PhoneNumberConfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.IdentityUsers", "PhoneNumber", c => c.String());
            AddColumn("dbo.IdentityUsers", "EmailConfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.IdentityUsers", "Email", c => c.String());
            DropForeignKey("dbo.IdentityUserRoles", "RoleId", "dbo.IdentityRoles");
            DropIndex("dbo.IdentityUserRoles", new[] { "RoleId" });
            DropPrimaryKey("dbo.IdentityUserRoles");
            AlterColumn("dbo.IdentityUserRoles", "RoleId", c => c.String(maxLength: 128));
            AddPrimaryKey("dbo.IdentityUserRoles", new[] { "RoleId", "UserId" });
            RenameIndex(table: "dbo.IdentityUserClaims", name: "IX_User_Id", newName: "IX_UserId");
            RenameColumn(table: "dbo.IdentityUserRoles", name: "RoleId", newName: "IdentityRole_Id");
            RenameColumn(table: "dbo.IdentityUserClaims", name: "User_Id", newName: "UserId");
            AddColumn("dbo.IdentityUserRoles", "RoleId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.IdentityUserRoles", "IdentityRole_Id");
            AddForeignKey("dbo.IdentityUserRoles", "IdentityRole_Id", "dbo.IdentityRoles", "Id");
            RenameTable(name: "dbo.IdentityUsers", newName: "Users");
        }
    }
}
