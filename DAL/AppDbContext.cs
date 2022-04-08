using Microsoft.EntityFrameworkCore;
using HomesForAll.DAL.Entities;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HomesForAll.DAL.UserRoles;

namespace HomesForAll.DAL
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid, IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasMany(j => j.Properties)
                .WithOne(j => j.LandLord)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<User>()
                .HasMany(u => u.PropertyRequests)
                .WithOne(tr => tr.Tenant)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Property>()
               .HasMany(p => p.AcceptedTenants)
               .WithOne(t => t.AcceptedAtProperty)
               .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Property>()
                .HasMany(p => p.TenantRequests)
                .WithOne(u => u.Property)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(builder);

        }
        
        public override DbSet<User>? Users { get; set; }
        public DbSet<Property>? Properties { get; set; }
        public DbSet<TenantRequest>? TenantRequests { get; set;}


    }
}