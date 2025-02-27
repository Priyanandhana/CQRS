using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public DbSet<PurchaseOrderHeader> PurchaseOrderHeaders { get; set; }
        public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PurchaseOrderHeader>()
                .ToTable("PurchaseOrderHeader") 
                .HasKey(p => p.ID); 

            modelBuilder.Entity<PurchaseOrderHeader>()
                .Property(p => p.ID)
                .UseIdentityColumn(); 

            modelBuilder.Entity<PurchaseOrderDetail>()
                .ToTable("PurchaseOrderDetails") 
                .HasKey(p => p.ID); 

            modelBuilder.Entity<PurchaseOrderDetail>()
                .HasOne<PurchaseOrderHeader>(d => d.PurchaseOrderHeader) 
                .WithMany(h => h.PODetailinfo) 
                .HasForeignKey(d => d.OUInstance) 
                .OnDelete(DeleteBehavior.NoAction); 
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
