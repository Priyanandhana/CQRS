using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

public class BulkInsertPurchaseOrderHandler : IRequestHandler<BulkInsertPurchaseOrderCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<BulkInsertPurchaseOrderHandler> _logger;

    public BulkInsertPurchaseOrderHandler(IAppDbContext context, ILogger<BulkInsertPurchaseOrderHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(BulkInsertPurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            _logger.LogInformation($"Received Bulk Insert Request with {request.PurchaseOrderHeaders.Count} headers.");

            // Start transaction
            using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    // Add purchase order headers
                    await _context.PurchaseOrderHeaders.AddRangeAsync(request.PurchaseOrderHeaders, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);

                    // Now, for each purchase order header, assign the correct OUInstance to the details
                    foreach (var header in request.PurchaseOrderHeaders)
                    {
                        foreach (var detail in header.PODetailinfo)
                        {
                            // Assign the OUInstance to each detail
                            detail.OUInstance = header.OUInstance;
                        }
                    }

                    // Add purchase order details
                    await _context.PurchaseOrderDetails.AddRangeAsync(
                        request.PurchaseOrderHeaders.SelectMany(h => h.PODetailinfo),
                        cancellationToken
                    );

                    // Commit the transaction
                    await _context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    stopwatch.Stop();
                    _logger.LogInformation($"Bulk Insert Completed in {stopwatch.ElapsedMilliseconds} ms.");

                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError($"Error in BulkInsertPurchaseOrderHandler: {ex.Message}", ex);
                    throw new Exception($"Error while inserting purchase orders: {ex.Message}", ex);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unhandled error in BulkInsertPurchaseOrderHandler: {ex.Message}", ex);
            throw;
        }
    }
}
