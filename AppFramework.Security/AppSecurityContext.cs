using AppFramework.Security.Menus;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;

namespace AppFramework.Security
{
    public class AppSecurityContext : IdentityDbContext<AppUser, AppRole, long, AppUserLogin, AppUserRole, AppUserClaim>
    {
        public static AppSecurityContext Create()
        {
            return new AppSecurityContext();
        }
        public AppSecurityContext() : base("LearningContext")
        {

        }

        public DbSet<AppResource> Resources { get; set; }
        public DbSet<AppAction> Actions { get; set; }
        public DbSet<AppPermission> Permissions { get; set; }
        public DbSet<AppUserRole> UserRoles { get; set; }
        public DbSet<AppRolePermission> RolesPermissions { get; set; }
        public DbSet<AppMenu> Menus { get; set; }
        public DbSet<AppMenuItem> MenuItems{ get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.HasDefaultSchema("security");
            //nombres de propiedades en minúscula como pide postgresql
            //modelBuilder.Properties().Configure(p => p.HasColumnName(p.ClrPropertyInfo.Name.ToLower()));
            //nombre de tablas en minúscula como lo pide postgresql
            //modelBuilder.Types().Configure(c => c.ToTable(GetTableName(c.ClrType)));
            //Precisión por defecto de decimales a menos que se especifique otro
            modelBuilder.Properties<decimal>().Configure(config => config.HasPrecision(10, 2));

            //Todos los id son claves primarias
            modelBuilder.Entity<AppUser>().ToTable("AppUsers");
            modelBuilder.Entity<AppUser>().Property(p => p.Id).HasColumnName("Id");
            modelBuilder.Entity<AppUser>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<AppUser>().Property(x => x.Version).IsConcurrencyToken();

            modelBuilder.Entity<AppRole>().ToTable("AppRoles").Property(p => p.Id).HasColumnName("Id");
            modelBuilder.Entity<AppRole>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<AppUserClaim>().ToTable("AppUserClaims").Property(p => p.Id).HasColumnName("Id");
            modelBuilder.Entity<AppUserRole>().ToTable("AppUserRoles");
            modelBuilder.Entity<AppUserLogin>().ToTable("AppUserLogins");

            modelBuilder.Entity<AppRolePermission>().ToTable("AppRolePermissions").HasKey(x => new { x.RoleId, x.PermissionId });

            //Resource and Action
            modelBuilder.Entity<AppResource>().ToTable("AppResources").HasKey(x => x.Key);
            modelBuilder.Entity<AppAction>().ToTable("AppActions").HasKey(x => x.Key);

            modelBuilder.Entity<AppPermission>().ToTable("AppPermissions").HasKey(p => p.Id);
            modelBuilder.Entity<AppPermission>().Property(x => x.Id).HasDatabaseGeneratedOption(databaseGeneratedOption: DatabaseGeneratedOption.Identity);

            //Action-Resource combination in Permission
            modelBuilder.Entity<AppPermission>().HasRequired(x => x.Action).WithMany().HasForeignKey(x => x.ActionKey);
            modelBuilder.Entity<AppPermission>().HasRequired(x => x.Resource).WithMany().HasForeignKey(x => x.ResourceKey);
            //Data Driven Menus
            var menu = modelBuilder.Entity<AppMenu>();
            menu.ToTable("AppMenus").HasKey(x => x.Key);
            menu.Property(x => x.Key).HasMaxLength(25);
            menu.HasMany(x => x.Items).WithRequired(x => x.AppMenu).HasForeignKey(x => x.AppMenuKey);

            var menuItem = modelBuilder.Entity<AppMenuItem>();
            menuItem.ToTable("AppMenuItems").HasKey(x=>x.Id);
            menuItem.Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            menuItem.HasMany(x => x.Children).WithOptional(x => x.Parent).HasForeignKey(x => x.ParentId);
            menuItem.HasOptional(x => x.Permission).WithMany().HasForeignKey(x => x.PermissionId);
        }

        public bool Exists<T>(T entity) where T : class
        {
            return this.Set<T>().Local.Any(e => e == entity);
        }
    }
}
