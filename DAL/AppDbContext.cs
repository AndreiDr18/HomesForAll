using Microsoft.EntityFrameworkCore;
using HomesForAll.DAL.Entities;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HomesForAll.DAL
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasMany(j => j.Properties)
                .WithOne(j => j.LandLord);

            builder.Entity<Property>()
               .HasMany(p => p.AcceptedTenants)
               .WithOne(t => t.AcceptedAtProperty)
               .HasPrincipalKey(t => t.Id)
               .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Property>()
                .HasOne(p => p.LandLord)
                .WithMany(p => p.Properties)
                .HasPrincipalKey(t=> t.Id)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(builder);
        }
        
        public override DbSet<User>? Users { get; set; }
        public DbSet<Property>? Properties { get; set; }


    }
}