using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Application.Interfaces;
using Domain.Entities;
using System.Diagnostics;
using Infrastructure.Persistence;
using Infrastructure.Data;

[ApiController]
[Route("api/purchase-orders")]
public class PurchaseOrderController : ControllerBase
{
    private readonly IFileConversionService _fileConversionService;
    private readonly PurchaseOrderValidationService _validationService;
    private readonly IMediator _mediator;
    private readonly AppDbContext _context;
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;

    public PurchaseOrderController(
        IFileConversionService fileConversionService,
        PurchaseOrderValidationService validationService,
        IMediator mediator,
        AppDbContext context,
        IPurchaseOrderRepository purchaseOrderRepository) 
    {
        _fileConversionService = fileConversionService;
        _validationService = validationService;
        _mediator = mediator;
        _context = context;
        _purchaseOrderRepository = purchaseOrderRepository;
    }

    [HttpPost("upload-excel-linq")]
    public async Task<IActionResult> UploadExcelFile(IFormFile file)
    {
        // Convert Excel to JSON
        var convertedData = await _fileConversionService.ConvertExcelToJsonAsync(file);

        if (convertedData == null || !convertedData.Any())
        {
            return BadRequest(new { Message = "No data found or invalid file." });
        }

        Console.WriteLine($"JSON Conversion Successful. Records Found: {convertedData.Count}"); 

        // Validate Data
        var validationResults = await _validationService.ValidatePurchaseOrderAsync(convertedData);
        var errors = validationResults.Where(result => result.HeaderErrors.Any() || result.DetailErrors.Any()).ToList();

        if (errors.Any())
        {
            return BadRequest(new { Message = "Validation failed", Errors = errors });
        }

        // Prepare Data for Bulk Insert using LINQ
        var stopwatch = Stopwatch.StartNew();

        var purchaseOrderHeaders = convertedData;

        Console.WriteLine($"Before insert: {purchaseOrderHeaders.Count} records in memory.");

        // Bulk Insert using EF Core & LINQ
        var purchaseOrderEntities = purchaseOrderHeaders.Select(header => new PurchaseOrderHeader
        {
            OUInstance = header.OUInstance,
            PurchaseOrder = header.PurchaseOrder,
            NumberingType = header.NumberingType,
            PODate = header.PODate,
            POPriority = header.POPriority,
            PartType = header.PartType,
            Remarks1 = header.Remarks1,
            Supplier = header.Supplier,
            POCurrency = header.POCurrency,
            AddressID = header.AddressID,
            PoStatus = header.PoStatus,
            AmendNo = header.AmendNo,
            Role = header.Role,
            User = header.User,
            Language = header.Language,
        }).ToList();

        _context.PurchaseOrderHeaders.AddRange(purchaseOrderEntities);

        // Add PurchaseOrderDetails
        foreach (var header in purchaseOrderHeaders)
        {
            var purchaseOrderDetails = header.PODetailinfo.Select(detail => new PurchaseOrderDetail
            {
                OUInstance = header.OUInstance,
                Part = detail.Part,
                OrderQty = detail.OrderQty,
                PurchaseUOM = detail.PurchaseUOM,
                Cost = detail.Cost,
                CostPer = detail.CostPer,
                Condition = detail.Condition,
                CertificateType = detail.CertificateType,
                Warehouse = detail.Warehouse,
                Remarks2 = detail.Remarks2,
                Role = detail.Role,
                User = detail.User,
                Language = detail.Language,
            }).ToList();

            _context.PurchaseOrderDetails.AddRange(purchaseOrderDetails);
        }

        await _context.SaveChangesAsync();

        stopwatch.Stop();
        Console.WriteLine($"After insert: {purchaseOrderHeaders.Count} records in memory.");
        Console.WriteLine($"Bulk Insert Time: {stopwatch.ElapsedMilliseconds}ms");

        return Ok(new { Message = "Bulk insert (linq/EFCore) successful", RecordsInserted = purchaseOrderHeaders.Count });
    }

    [HttpPost("upload-excel-tvp")]
    public async Task<IActionResult> UploadExcelFileUsingTVP(IFormFile file)
    {
        var convertedData = await _fileConversionService.ConvertExcelToJsonAsync(file);
        if (convertedData == null || !convertedData.Any())
        {
            return BadRequest(new { Message = "No data found or invalid file." });
        }
        Console.WriteLine($"JSON Conversion Successful. Records Found: {convertedData.Count}");

        var validationResults = await _validationService.ValidatePurchaseOrderAsync(convertedData);
        var errors = validationResults.Where(result => result.HeaderErrors.Any() || result.DetailErrors.Any()).ToList();
        if (errors.Any())
        {
            return BadRequest(new { Message = "Validation failed", Errors = errors });
        }

        foreach (var header in convertedData)
        {
            header.OUInstance = 2;
            header.NumberingType = "PO";
            foreach (var detail in header.PODetailinfo)
            {
                detail.OUInstance = header.OUInstance;
            }
        }

        var headers = convertedData;
        var details = headers.SelectMany(h => h.PODetailinfo).ToList();

        var command = new BulkInsertPurchaseOrderTVPCommand(headers, details);
        bool result = await _mediator.Send(command);

        return Ok(new { Message = "Bulk insert (TVP) successful", RecordsInserted = headers.Count });
    }

    [HttpPost("upload-excel-sqlbulkcopy")]
    public async Task<IActionResult> UploadExcelFileUsingSqlBulkCopy(IFormFile file)
    {
        var convertedData = await _fileConversionService.ConvertExcelToJsonAsync(file);
        if (convertedData == null || !convertedData.Any())
        {
            return BadRequest(new { Message = "No data found or invalid file." });
        }
        Console.WriteLine($"JSON Conversion Successful. Records Found: {convertedData.Count}");

        var validationResults = await _validationService.ValidatePurchaseOrderAsync(convertedData);
        var errors = validationResults.Where(result => result.HeaderErrors.Any() || result.DetailErrors.Any()).ToList();
        if (errors.Any())
        {
            return BadRequest(new { Message = "Validation failed", Errors = errors });
        }

        foreach (var header in convertedData)
        {
            header.OUInstance = 2;
            header.NumberingType = "PO";
            foreach (var detail in header.PODetailinfo)
            {
                detail.OUInstance = header.OUInstance;
            }
        }

        var headers = convertedData;
        var details = headers.SelectMany(h => h.PODetailinfo).ToList();

        var command = new BulkInsertUsingSqlBulkCopyCommand(headers, details);
        bool result = await _mediator.Send(command);

        return Ok(new { Message = "Bulk insert (SqlBulkCopy) successful", RecordsInserted = headers.Count });
    }

    [HttpPost("upload-excel-dapper")]
    public async Task<IActionResult> BulkInsertUsingDapper(IFormFile file)
    {
        // Step 1: Convert Excel to JSON
        var convertedData = await _fileConversionService.ConvertExcelToJsonAsync(file);
        if (convertedData == null || !convertedData.Any())
        {
            return BadRequest(new { Message = "No data found or invalid file." });
        }
        Console.WriteLine($"JSON Conversion Successful. Records Found: {convertedData.Count}");

        // Step 2: Validate Data
        var validationResults = await _validationService.ValidatePurchaseOrderAsync(convertedData);
        var errors = validationResults.Where(result => result.HeaderErrors.Any() || result.DetailErrors.Any()).ToList();
        if (errors.Any())
        {
            return BadRequest(new { Message = "Validation failed", Errors = errors });
        }

        //bulk insert
        foreach (var header in convertedData)
        {
            header.OUInstance = 2; 
            header.NumberingType = "PO";
            foreach (var detail in header.PODetailinfo)
            {
                detail.OUInstance = header.OUInstance;
            }
        }

        var headers = convertedData;
        var details = headers.SelectMany(h => h.PODetailinfo).ToList();

        await _purchaseOrderRepository.BulkInsertUsingDapperAsync(headers, details);

        return Ok(new { Message = "Bulk insert using Dapper successful", RecordsInserted = details.Count });
    }

    [HttpPost("upload-excel-dapper-plus")]
    public async Task<IActionResult> BulkInsertUsingDapperPlus(IFormFile file)
    {
        // Convert Excel to JSON
        var convertedData = await _fileConversionService.ConvertExcelToJsonAsync(file);
        if (convertedData == null || !convertedData.Any())
        {
            return BadRequest(new { Message = "No data found or invalid file." });
        }
        Console.WriteLine($"JSON Conversion Successful. Records Found: {convertedData.Count}");

        // Validate Data
        var validationResults = await _validationService.ValidatePurchaseOrderAsync(convertedData);
        var errors = validationResults.Where(result => result.HeaderErrors.Any() || result.DetailErrors.Any()).ToList();
        if (errors.Any())
        {
            return BadRequest(new { Message = "Validation failed", Errors = errors });
        }

        // Set Default Values for Headers & Details
        foreach (var header in convertedData)
        {
            header.OUInstance = 2;
            header.NumberingType = "PO";
            foreach (var detail in header.PODetailinfo)
            {
                detail.OUInstance = header.OUInstance;
            }
        }

        var headers = convertedData;
        var details = headers.SelectMany(h => h.PODetailinfo).ToList();

        // Perform Bulk Insert Using Dapper Plus
        await _purchaseOrderRepository.BulkInsertUsingDapperPlusAsync(headers, details);

        return Ok(new { Message = "Bulk insert using Dapper Plus successful", RecordsInserted = details.Count });
    }


}


