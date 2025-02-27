using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;

public class BulkInsertPurchaseOrderTVPHandler : IRequestHandler<BulkInsertPurchaseOrderTVPCommand, bool>
{
    private readonly IPurchaseOrderRepository _repository;

    public BulkInsertPurchaseOrderTVPHandler(IPurchaseOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(BulkInsertPurchaseOrderTVPCommand request, CancellationToken cancellationToken)
    {
        await _repository.BulkInsertUsingTVPAsync(request.Headers, request.Details);
        return true;
    }
}
