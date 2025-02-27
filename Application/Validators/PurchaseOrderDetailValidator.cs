using FluentValidation;
using Domain.Entities;

public class PurchaseOrderDetailValidator : AbstractValidator<PurchaseOrderDetail>
{
    public PurchaseOrderDetailValidator()
    {
        RuleFor(x => x.Part)
            .NotEmpty().WithMessage("Unable to process Row #. Please Enter Part.")
            .Must(BeAValidPart).WithMessage("Part # is invalid.")
            .Must(BeActivePart).WithMessage("Part # is not in Active Status.")
            .Must(BePartPlanningActive).WithMessage("Part Planning Status is not Active.")
            .Must(HavePurchaseInformation).WithMessage("Purchase information does not exist for the Part.");

        RuleFor(x => x.OrderQty)
            .GreaterThan(0).WithMessage("Unable to process Row #. Order Quantity must be greater than 0.");

        RuleFor(x => x.PurchaseUOM)
            .NotEmpty().WithMessage("Unable to process Row #. Please provide Purchase UOM.")
            .Must(BeAValidUOM).WithMessage("Purchase UOM is invalid.")
            .Must(UOMExistsForPart).WithMessage("Purchase UOM does not exist for the Part.");

        RuleFor(x => x.Cost)
            .GreaterThan(0).WithMessage("Unable to process Row #. Cost must be greater than 0.")
            .Must(BePositiveNumeric).WithMessage("Cost should be a positive numeric value.");

        RuleFor(x => x.CostPer)
            .GreaterThan(0).WithMessage("Unable to process Row #. Cost Per must be greater than 0.")
            .Must(BePositiveNumeric).WithMessage("Cost Per should be a positive numeric value.");

        RuleFor(x => x.Condition)
            .NotEmpty().WithMessage("Unable to process Row #. Please Enter Condition.")
            .Must(BeAValidCondition).WithMessage("Condition is invalid.");

        RuleFor(x => x.Warehouse)
            .NotEmpty().WithMessage("Unable to process Row #. Please Enter Warehouse.");

        RuleFor(x => x.PurchaseUOM)
            .Must(ValidatePartUOM).WithMessage("Unable to process Row #. Parts of Expense Type \"Revenue\" cannot be ordered under PO of Expense Type \"Capital\".");

        RuleFor(x => x.CertificateType)
            .Must(IsValidCertificateType).WithMessage("Certificate Type is invalid.");

        RuleFor(x => x.PurchaseUOM)
            .Must(BeValidPurchaseUOM).WithMessage("Purchase UOM of the Part is not in Active Status");

        RuleFor(x => new { x.PurchaseUOM })
            .Must(HaveUOMConversionFactor).WithMessage("UOM Conversion factor doesn't exists between PurchaseUOM");

        RuleFor(x => x.Part)
            .Must(NotAllowFractions).WithMessage("Fractions are not allowed for the Part");

        RuleFor(x => x.Warehouse)
            .Must(BeValidWarehouse).WithMessage("Warehouse is not valid");

        RuleFor(x => x.Warehouse)
            .Must(BeActiveWarehouseStatus).WithMessage("Warehouse Status is not Active");

        RuleFor(x => new { x.Part, x.Warehouse })
            .Must(BeAllowedPartTypeInWarehouse).WithMessage("Part Type is not allowed in the warehouse");

        RuleFor(x => new { x.Part, x.Warehouse })
            .Must(BeAllowedTransactionTypeInWarehouse).WithMessage("Transaction Type is not allowed in the warehouse");

        RuleFor(x => x.Part)
            .Must(HaveActivePlanningInfo).WithMessage("Part Planning information for the part is not defined or not in Active status");

        RuleFor(x => x.Part)
            .Must(HavePlanningInfo).WithMessage("Part Planning information for the part is not defined");

        RuleFor(x => x.Part)
            .Must(NotAllowGRMovementForNonStockablePart).WithMessage("GR Movement cannot be provided for non-stockable part");

        RuleFor(x => x.Part)
            .Must(NotAllowNonControlledParts).WithMessage("Please do not provide none controlled Parts");

        RuleFor(x => x.Part)
            .Must(PartTypeIsExpendable).WithMessage("Part Type is selected as 'Expendable'. Please provide Parts with Expendable Part Type");

        RuleFor(x => x.Part)
            .Must(PartTypeIsNotConsumable).WithMessage("Part Type is selected as 'Expendable'. Please do not provide Consumable Parts");

        RuleFor(x => new { x.Part, x.Condition, x.Warehouse, x.CertificateType })
            .Must(BeUniquePartConditionWarehouseCertificateTypeCombination)
            .WithMessage("Unable to process row #. Please Enter Unique combination of Part # , Condition, Warehouse and Certificate Type");
    }

    private bool BeAValidPart(string part)
    {
        
        return true; 
    }

    private bool BeActivePart(string part)
    {
        return true; 
    }

    private bool BePartPlanningActive(string part)
    {
        return true; 
    }

    private bool HavePurchaseInformation(string part)
    {
        return true; 
    }

    private bool BePositiveNumeric(decimal value)
    {
        return value > 0;
    }

    private bool BeAValidUOM(string uom)
    {
       
        return !string.IsNullOrEmpty(uom);
    }

    private bool IsValidCertificateType(string certType)
    {
        
        var validTypes = new List<string> { "TypeA", "TypeB", "TypeC" };
        return validTypes.Contains(certType);
    }

    private bool UOMExistsForPart(string uom)
    {
        
        return true; 
    }

    private bool BeValidExpenseType(string expenseType)
    {
        return true; 
    }

    private bool ValidatePartUOM(string expenseType)
    {
        // Logic comes here
        return true; // write actual logic here ( derive ) 
    }

    private bool BeAValidCondition(string condition)
    {
        var validConditions = new[] { "New", "Used", "Refurbished" }; 
        return !string.IsNullOrEmpty(condition) && Array.Exists(validConditions, cond => cond == condition);
    }

    private bool BeValidPurchaseUOM(string purchaseUOM)
    { 
        return true;
    }

    private bool BeValidStockUOM(string stockUOM)
    {
        return true;
    }

    private bool HaveUOMConversionFactor(dynamic uom)
    {
        return true;
    }

    private bool NotAllowFractions(string part)
    {
        return true;
    }

    private bool BeValidWarehouse(string warehouse)
    {
        return true;
    }

    private bool BeActiveWarehouseStatus(string warehouse)
    {
        // Validate if the warehouse is active
        return true;
    }

    private bool BeAllowedPartTypeInWarehouse(dynamic warehousePart)
    {
        // Check if the part type is allowed in the warehouse
        return true;
    }

    private bool BeAllowedTransactionTypeInWarehouse(dynamic warehousePart)
    {
        // Validate if the transaction type is allowed in the warehouse
        return true;
    }

    private bool HaveActivePlanningInfo(string part)
    {
        // Validate if planning info for the part is active
        return true;
    }

    private bool HavePlanningInfo(string part)
    {
        // Validate if planning info for the part is defined
        return true;
    }

    private bool NotAllowGRMovementForNonStockablePart(string part)
    {
        // Check if GR Movement can be provided for non-stockable part
        return true;
    }

    private bool NotAllowNonControlledParts(string part)
    {
        // Check if non-controlled parts are allowed
        return true;
    }

    private bool PartTypeIsExpendable(string part)
    {
        // Check if the part type is expendable
        return true;
    }

    private bool PartTypeIsNotConsumable(string part)
    {
        // Check if the part type is not consumable
        return true;
    }

    private bool BeUniquePartConditionWarehouseCertificateTypeCombination(dynamic partCombination)
    {
        // Validate if the combination of Part, Condition, Warehouse, and Certificate Type is unique
        return true;
    }


}

