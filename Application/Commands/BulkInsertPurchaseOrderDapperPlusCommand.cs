using MediatR;
using Domain.Entities;
using System.Collections.Generic;

public class BulkInsertPurchaseOrderDapperPlusCommand : IRequest<bool>
{
    public List<PurchaseOrderHeader> Headers { get; set; }
    public List<PurchaseOrderDetail> Details { get; set; }

    public BulkInsertPurchaseOrderDapperPlusCommand(List<PurchaseOrderHeader> headers, List<PurchaseOrderDetail> details)
    {
        Headers = headers;
        Details = details;
    }
}
