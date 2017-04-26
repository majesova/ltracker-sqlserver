namespace AppFramework.Security.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AppActions",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Key);
            
            CreateTable(
                "dbo.AppMenuItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Order = c.Int(nullable: false),
                        Name = c.String(),
                        PathToResource = c.String(),
                        AppMenuKey = c.String(nullable: false, maxLength: 25),
                        ParentId = c.Int(),
                        PermissionId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AppMenus", t => t.AppMenuKey, cascadeDelete: true)
                .ForeignKey("dbo.AppMenuItems", t => t.ParentId)
                .ForeignKey("dbo.AppPermissions", t => t.PermissionId)
                .Index(t => t.AppMenuKey)
                .Index(t => t.ParentId)
                .Index(t => t.PermissionId);
            
            CreateTable(
                "dbo.AppMenus",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 25),
                        IsActive = c.Boolean(),
                        Name = c.String(),
                        RoleId = c.Long(),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.AppRoles", t => t.RoleId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AppRoles",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AppUserRoles",
                c => new
                    {
                        UserId = c.Long(nullable: false),
                        RoleId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AppRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AppUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AppPermissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ActionKey = c.String(nullable: false, maxLength: 128),
                        ResourceKey = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AppActions", t => t.ActionKey, cascadeDelete: true)
                .ForeignKey("dbo.AppResources", t => t.ResourceKey, cascadeDelete: true)
                .Index(t => t.ActionKey)
                .Index(t => t.ResourceKey);
            
            CreateTable(
                "dbo.AppResources",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Key);
            
            CreateTable(
                "dbo.AppRolePermissions",
                c => new
                    {
                        RoleId = c.Long(nullable: false),
                        PermissionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoleId, t.PermissionId })
                .ForeignKey("dbo.AppPermissions", t => t.PermissionId, cascadeDelete: true)
                .ForeignKey("dbo.AppRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.PermissionId);
            
            CreateTable(
                "dbo.AppUsers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Info = c.String(),
                        Version = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AppUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AppUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AppUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AppUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AppUserRoles", "UserId", "dbo.AppUsers");
            DropForeignKey("dbo.AppUserLogins", "UserId", "dbo.AppUsers");
            DropForeignKey("dbo.AppUserClaims", "UserId", "dbo.AppUsers");
            DropForeignKey("dbo.AppRolePermissions", "RoleId", "dbo.AppRoles");
            DropForeignKey("dbo.AppRolePermissions", "PermissionId", "dbo.AppPermissions");
            DropForeignKey("dbo.AppMenuItems", "PermissionId", "dbo.AppPermissions");
            DropForeignKey("dbo.AppPermissions", "ResourceKey", "dbo.AppResources");
            DropForeignKey("dbo.AppPermissions", "ActionKey", "dbo.AppActions");
            DropForeignKey("dbo.AppMenuItems", "ParentId", "dbo.AppMenuItems");
            DropForeignKey("dbo.AppMenus", "RoleId", "dbo.AppRoles");
            DropForeignKey("dbo.AppUserRoles", "RoleId", "dbo.AppRoles");
            DropForeignKey("dbo.AppMenuItems", "AppMenuKey", "dbo.AppMenus");
            DropIndex("dbo.AppUserLogins", new[] { "UserId" });
            DropIndex("dbo.AppUserClaims", new[] { "UserId" });
            DropIndex("dbo.AppUsers", "UserNameIndex");
            DropIndex("dbo.AppRolePermissions", new[] { "PermissionId" });
            DropIndex("dbo.AppRolePermissions", new[] { "RoleId" });
            DropIndex("dbo.AppPermissions", new[] { "ResourceKey" });
            DropIndex("dbo.AppPermissions", new[] { "ActionKey" });
            DropIndex("dbo.AppUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AppUserRoles", new[] { "UserId" });
            DropIndex("dbo.AppRoles", "RoleNameIndex");
            DropIndex("dbo.AppMenus", new[] { "RoleId" });
            DropIndex("dbo.AppMenuItems", new[] { "PermissionId" });
            DropIndex("dbo.AppMenuItems", new[] { "ParentId" });
            DropIndex("dbo.AppMenuItems", new[] { "AppMenuKey" });
            DropTable("dbo.AppUserLogins");
            DropTable("dbo.AppUserClaims");
            DropTable("dbo.AppUsers");
            DropTable("dbo.AppRolePermissions");
            DropTable("dbo.AppResources");
            DropTable("dbo.AppPermissions");
            DropTable("dbo.AppUserRoles");
            DropTable("dbo.AppRoles");
            DropTable("dbo.AppMenus");
            DropTable("dbo.AppMenuItems");
            DropTable("dbo.AppActions");
        }
    }
}
