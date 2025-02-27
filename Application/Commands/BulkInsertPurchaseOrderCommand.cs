using Domain.Entities;
using MediatR;
using System.Collections.Generic;

public class BulkInsertPurchaseOrderCommand : IRequest<bool>
{
    public List<PurchaseOrderHeader> PurchaseOrderHeaders { get; set; }

    public BulkInsertPurchaseOrderCommand(List<PurchaseOrderHeader> purchaseOrderHeaders)
    {
        PurchaseOrderHeaders = purchaseOrderHeaders;
    }
}



public class BulkInsertResult
{
    public bool IsSuccess { get; set; }
    public int RecordsInserted { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public long ExecutionTime { get; set; } 
}

