using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Persistence.Models;

namespace Persistence.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options)
        {
        }

        public DbSet<ConstructionSite> ConstructionSites { get; set; } = default!;
        public DbSet<Client> Clients { get; set; } = default!;
        public DbSet<Material> Material { get; set; } = default!;
        public DbSet<MaterialMovement> MaterialMovements { get; set; } = default!;
    }
}
