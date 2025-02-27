using System;
using System.Collections.Generic;
using Domain.Entities;
using MediatR;

public class BulkInsertUsingSqlBulkCopyCommand : IRequest<bool>
{
    public List<PurchaseOrderHeader> Headers { get; }
    public List<PurchaseOrderDetail> Details { get; }

    public BulkInsertUsingSqlBulkCopyCommand(List<PurchaseOrderHeader> headers, List<PurchaseOrderDetail> details)
    {
        Headers = headers ?? throw new ArgumentNullException(nameof(headers));
        Details = details ?? throw new ArgumentNullException(nameof(details));
    }
}
