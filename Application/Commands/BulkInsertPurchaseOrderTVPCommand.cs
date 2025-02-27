using Domain.Entities;
using MediatR;
using System.Collections.Generic;

public class BulkInsertPurchaseOrderTVPCommand : IRequest<bool>
{
    public List<PurchaseOrderHeader> Headers { get; set; }
    public List<PurchaseOrderDetail> Details { get; set; }

    public BulkInsertPurchaseOrderTVPCommand(List<PurchaseOrderHeader> headers, List<PurchaseOrderDetail> details)
    {
        Headers = headers;
        Details = details;
    }
}
