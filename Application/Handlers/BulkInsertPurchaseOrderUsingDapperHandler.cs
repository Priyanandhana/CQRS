using MediatR;
using System.Threading;
using System.Threading.Tasks;

public class BulkInsertPurchaseOrderUsingDapperHandler : IRequestHandler<BulkInsertUsingDapperCommand>
{
    private readonly IPurchaseOrderRepository _repository;

    public BulkInsertPurchaseOrderUsingDapperHandler(IPurchaseOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(BulkInsertUsingDapperCommand request, CancellationToken cancellationToken)
    {
        await _repository.BulkInsertUsingDapperAsync(request.Headers, request.Details);
    }
}
