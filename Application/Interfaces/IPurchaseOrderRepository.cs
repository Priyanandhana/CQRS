using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IPurchaseOrderRepository
{
    // LINQ method
    Task<int> BulkInsertlinqAsync(List<PurchaseOrderHeader> purchaseOrders);

    // TVP method
    Task BulkInsertUsingTVPAsync(List<PurchaseOrderHeader> headers, List<PurchaseOrderDetail> details);

    //SqlBulkCopy
    Task BulkInsertUsingSqlBulkCopy(List<PurchaseOrderHeader> headers, List<PurchaseOrderDetail> details);

    //Dapper
    Task BulkInsertUsingDapperAsync(List<PurchaseOrderHeader> headers, List<PurchaseOrderDetail> details);

    //Dapper Plus
    Task<bool> BulkInsertUsingDapperPlusAsync(List<PurchaseOrderHeader> headers, List<PurchaseOrderDetail> details);

}
