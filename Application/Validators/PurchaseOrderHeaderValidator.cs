using FluentValidation;
using Domain.Entities;
using System;

public class PurchaseOrderHeaderValidator : AbstractValidator<PurchaseOrderHeader>
{
    public PurchaseOrderHeaderValidator()
    {
        RuleFor(x => x.PurchaseOrder)
            .NotEmpty().WithMessage("Unable to process Row #. Please Enter Purchase Order.")
            .MaximumLength(11).WithMessage("Unable to process Row #. Purchase Order # length exceeds. Please enter length ≤ 11.");

        RuleFor(x => x.PODate)
            .NotEmpty().WithMessage("Unable to process Row #. Please Enter PO Date.")
            .Must(BeAValidDate).WithMessage("Invalid PO Date format.")
            .Must(BeLessThanOrEqualToCurrentDate).WithMessage("PO Date cannot be later than current date.");

        RuleFor(x => x.POPriority)
            .NotEmpty().WithMessage("Unable to process Row #. Please Enter PO Priority.")
            .Must(BeValidPOPriority).WithMessage("PO Priority is invalid.");

        RuleFor(x => x.PartType)
            .NotEmpty().WithMessage("Unable to process Row #. Please provide Part Type.")
            .Must(BeAValidPartType).WithMessage("Part Type is invalid.");

        RuleFor(x => x.Supplier)
            .NotEmpty().WithMessage("Unable to process Row #. Please Enter Supplier.")
            .Must(BeAValidSupplier).WithMessage("Supplier # is invalid.")
            .Must(BeActiveSupplier).WithMessage("Supplier # is not in Active Status.")
            .Must(NotBeOnHold).WithMessage("Unable to process. Operational Status for the Supplier # is set as 'Hold PO'. Parts / Services cannot be procured from the supplier.");

        RuleFor(x => x.POCurrency)
            .NotEmpty().WithMessage("Unable to process Row #. Please provide PO Currency.")
            .Must(BeAValidCurrency).WithMessage("PO Currency is invalid.")
            .Must(MatchSupplierCurrency).WithMessage("PO currency is different from the Supplier currency.");

        RuleFor(x => x.NumberingType)
            .NotEmpty().WithMessage("Unable to process Row #. Please Enter Numbering Type.")
            .Must(BeValidNumberingType).WithMessage("Numbering Type is invalid.");

        RuleFor(x => x.AddressID)
            .Must(BeValidAddressId).WithMessage("Unable to process Row #. Address Id is not valid for the Supplier.");

    }

    private bool BeAValidDate(DateTime date) => date != default;

    private bool BeLessThanOrEqualToCurrentDate(DateTime date) => date <= DateTime.Now;

    private bool BeValidPOPriority(string poPriority)
    {
        var validPriorities = new[] { "Low", "Medium", "High" };
        return !string.IsNullOrEmpty(poPriority) && Array.Exists(validPriorities, p => p == poPriority);
    }

    private bool BeAValidPartType(string partType)
    {
        var validPartTypes = new[] { "All", "New" }; // type1, type2, type3
        return !string.IsNullOrEmpty(partType) && Array.Exists(validPartTypes, type => type == partType);
    }

    private bool BeAValidCurrency(string currency)
    {
        var validCurrencies = new[] { "USD", "EUR", "GBP" };
        return !string.IsNullOrEmpty(currency) && Array.Exists(validCurrencies, curr => curr == currency);
    }

    private bool BeActiveSupplier(string supplier) => true; // Implement actual check

    private bool BeAValidSupplier(string supplier) => true; 

    private bool MatchSupplierCurrency(string currency) => true; 

    private bool BeValidAddressId(int addressId) => true; 

    private bool BeValidCertificateType(string certificateType) => !string.IsNullOrEmpty(certificateType);

    private bool BeValidNumberingType(string numberingType)
    {
        var validNumberingTypes = new[] { "Auto", "Manual" , "PO" };
        return !string.IsNullOrEmpty(numberingType) && Array.Exists(validNumberingTypes, type => type == numberingType);
    }

    private bool NotBeOnHold(string supplier) => true; 
}
