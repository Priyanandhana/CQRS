using Domain.Entities;

public class PurchaseOrderValidationService
{
    private readonly PurchaseOrderHeaderValidator _headerValidator;
    private readonly PurchaseOrderDetailValidator _detailValidator;

    public PurchaseOrderValidationService(
        PurchaseOrderHeaderValidator headerValidator,
        PurchaseOrderDetailValidator detailValidator)
    {
        _headerValidator = headerValidator;
        _detailValidator = detailValidator;
    }

    public async Task<List<ValidationResult>> ValidatePurchaseOrderAsync(List<PurchaseOrderHeader> purchaseOrders)
    {
        var validationTasks = purchaseOrders.Select(async po =>
        {
            var headerValidationResult = await _headerValidator.ValidateAsync(po);

            // Ensure PODetailinfo is not null before validation
            var detailValidationTasks = po.PODetailinfo?.Select(d => _detailValidator.ValidateAsync(d)) ?? Enumerable.Empty<Task<FluentValidation.Results.ValidationResult>>();
            var detailValidationResults = await Task.WhenAll(detailValidationTasks);

            return new ValidationResult
            {
                PurchaseOrder = po.PurchaseOrder,
                HeaderErrors = headerValidationResult.Errors.Select(e => e.ErrorMessage).ToList(),
                DetailErrors = detailValidationResults
                                .SelectMany((result, index) => result.Errors.Select(e => $"Detail {index + 1}: {e.ErrorMessage}"))
                                .ToList()
            };
        });

        var results = await Task.WhenAll(validationTasks);
        return results.ToList();
    }
}

public class ValidationResult
{
    public string PurchaseOrder { get; set; }
    public List<string> HeaderErrors { get; set; } = new();
    public List<string> DetailErrors { get; set; } = new();
}
