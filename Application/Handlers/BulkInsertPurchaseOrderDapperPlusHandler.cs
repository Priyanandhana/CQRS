using MediatR;
using Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;

public class BulkInsertPurchaseOrderDapperPlusHandler : IRequestHandler<BulkInsertPurchaseOrderDapperPlusCommand, bool>
{
    private readonly IPurchaseOrderRepository _repository;

    public BulkInsertPurchaseOrderDapperPlusHandler(IPurchaseOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(BulkInsertPurchaseOrderDapperPlusCommand request, CancellationToken cancellationToken)
    {
        return await _repository.BulkInsertUsingDapperPlusAsync(request.Headers, request.Details);
        return true;
    }
}
