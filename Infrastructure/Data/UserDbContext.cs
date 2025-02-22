using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using UsersAuthorization.Domain.Entities;

namespace UsersAuthorization.Infrastructure.Data
{
    public class UserDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public DbSet<ApplicationUser> Users { get; set; }

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().ToTable("users");
            modelBuilder.Entity<IdentityRole<int>>().ToTable("roles");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("user_roles");

            //ignore some tables
            modelBuilder.Ignore<IdentityUserClaim<int>>();
            modelBuilder.Ignore<IdentityUserLogin<int>>();
            modelBuilder.Ignore<IdentityUserToken<int>>();
            modelBuilder.Ignore<IdentityRoleClaim<int>>();

            modelBuilder.Entity<ApplicationUser>().Property(u => u.Id).HasColumnName("id").ValueGeneratedOnAdd().UseIdentityColumn();
            modelBuilder.Entity<ApplicationUser>().Property(u => u.UserName).HasColumnName("username");
            modelBuilder.Entity<ApplicationUser>().Property(u => u.Email).HasColumnName("email");
            modelBuilder.Entity<ApplicationUser>().Property(u => u.RegistrationDate).HasColumnName("registration_date");
            modelBuilder.Entity<ApplicationUser>().Property(u => u.Status).HasColumnName("status");
            modelBuilder.Entity<ApplicationUser>().Property(u => u.PasswordHash).HasColumnName("password_hash");
            modelBuilder.Entity<ApplicationUser>().Property(u => u.SecurityStamp).HasColumnName("security_stamp");
            modelBuilder.Entity<ApplicationUser>().Property(u => u.NormalizedUserName).HasColumnName("normalized_user_name");

            modelBuilder.Entity<IdentityRole<int>>().Property(u => u.Id).HasColumnName("id").ValueGeneratedOnAdd().UseIdentityColumn();
            
            modelBuilder.Entity<IdentityUserRole<int>>().Property(r => r.UserId).HasColumnName("id_user");
            modelBuilder.Entity<IdentityUserRole<int>>().Property(r => r.RoleId).HasColumnName("id_role");

            //ignore some fields
            modelBuilder.Entity<ApplicationUser>().Ignore(u => u.AccessFailedCount);
            modelBuilder.Entity<ApplicationUser>().Ignore(u => u.ConcurrencyStamp);
            modelBuilder.Entity<ApplicationUser>().Ignore(u => u.EmailConfirmed);
            modelBuilder.Entity<ApplicationUser>().Ignore(u => u.LockoutEnabled);
            modelBuilder.Entity<ApplicationUser>().Ignore(u => u.LockoutEnd);
            modelBuilder.Entity<ApplicationUser>().Ignore(u => u.NormalizedEmail);
            //modelBuilder.Entity<ApplicationUser>().Ignore(u => u.NormalizedUserName);
            modelBuilder.Entity<ApplicationUser>().Ignore(u => u.PhoneNumber);
            modelBuilder.Entity<ApplicationUser>().Ignore(u => u.PhoneNumberConfirmed);
            //modelBuilder.Entity<ApplicationUser>().Ignore(u => u.SecurityStamp);
            modelBuilder.Entity<ApplicationUser>().Ignore(u => u.TwoFactorEnabled);
            //modelBuilder.Entity<ApplicationUser>().Ignore(u => u.UserName);
            modelBuilder.Entity<ApplicationUser>().HasKey(u => u.Id);


            //add data
            modelBuilder.Entity<IdentityRole<int>>().HasData(new IdentityRole<int> { Id=1, Name = "super_admin", NormalizedName = "SUPER_ADMIN", ConcurrencyStamp = "example" });
            modelBuilder.Entity<IdentityRole<int>>().HasData(new IdentityRole<int> { Id =2,Name = "user", NormalizedName = "USER", ConcurrencyStamp = "example2" });
        }

    }
}
