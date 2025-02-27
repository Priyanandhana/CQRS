using Domain.Entities;
using MediatR;
using System.Collections.Generic;

public class BulkInsertUsingDapperCommand : IRequest
{
    public List<PurchaseOrderHeader> Headers { get; }
    public List<PurchaseOrderDetail> Details { get; }

    public BulkInsertUsingDapperCommand(List<PurchaseOrderHeader> headers, List<PurchaseOrderDetail> details)
    {
        Headers = headers;
        Details = details;
    }
}
