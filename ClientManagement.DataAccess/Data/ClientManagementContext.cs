using ClientManagement.Models.Models.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClientManagement.Models
{
    public partial class ClientManagementContext : IdentityDbContext<ApplicationUser>
    {
        public ClientManagementContext(DbContextOptions<ClientManagementContext> options)
            : base(options)
        {
        }

        public virtual  DbSet<Client> Clients { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(e => e.ClientName).HasMaxLength(20);

                entity.Property(e => e.Description).HasMaxLength(300);
            });

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUserLogin<string>>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
