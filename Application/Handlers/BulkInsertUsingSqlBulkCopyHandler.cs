using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

public class BulkInsertUsingSqlBulkCopyHandler : IRequestHandler<BulkInsertUsingSqlBulkCopyCommand, bool>
{
    private readonly IPurchaseOrderRepository _repository;

    public BulkInsertUsingSqlBulkCopyHandler(IPurchaseOrderRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<bool> Handle(BulkInsertUsingSqlBulkCopyCommand request, CancellationToken cancellationToken)
    {
        await _repository.BulkInsertUsingSqlBulkCopy(request.Headers, request.Details);
        return true;
    }
}
