using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class PurchaseOrderService
{
    private readonly IAppDbContext _context;

    public PurchaseOrderService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task BulkInsertPurchaseOrdersAsync(List<PurchaseOrderHeader> headers, List<PurchaseOrderDetail> details, CancellationToken cancellationToken = default)
    {
        // Start a database transaction to ensure both header and detail insertions are atomic
        using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
        {
            try
            {
                // 1. Insert PurchaseOrderHeaders first and save changes to the database
                _context.PurchaseOrderHeaders.AddRange(headers);
                await _context.SaveChangesAsync(cancellationToken); // Ensure headers are saved before details

                // 2. Insert PurchaseOrderDetails using the valid OrderIds from the headers
                foreach (var detail in details)
                {
                    // Ensure each detail has the correct OrderId that references a valid PurchaseOrderHeader
                    var header = headers.FirstOrDefault(h => h.OUInstance == detail.OUInstance);
                    if (header != null)
                    {
                        // If the corresponding header is found, add the detail
                        _context.PurchaseOrderDetails.Add(detail);
                    }
                    else
                    {
                        // If no corresponding PurchaseOrderHeader exists, throw an error
                        throw new InvalidOperationException($"PurchaseOrderHeader with Id {detail.OUInstance} not found.");
                    }
                }

                // 3. Save all PurchaseOrderDetails to the database
                await _context.SaveChangesAsync(cancellationToken);

                // 4. Commit the transaction if everything is successful
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // 5. Rollback the transaction if something goes wrong
                await transaction.RollbackAsync(cancellationToken);
                throw new InvalidOperationException("An error occurred while inserting purchase orders and details.", ex);
            }
        }
    }
}
