using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

public interface IAppDbContext
{
    DbSet<PurchaseOrderHeader> PurchaseOrderHeaders { get; }
    DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; }

    DatabaseFacade Database { get; }  

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
