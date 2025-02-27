using Application.Interfaces;
using Domain.Entities;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class PurchaseOrderFileService : IFileConversionService
    {
        public async Task<List<PurchaseOrderHeader>> ConvertExcelToJsonAsync(IFormFile file)
        {
            var purchaseOrderHeaders = new List<PurchaseOrderHeader>();
            var purchaseOrderDict = new Dictionary<string, PurchaseOrderHeader>(); // To track headers by PO number

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true // Assuming first row contains headers
                        }
                    });

                    var dataTable = result.Tables[0];

                    if (dataTable.Rows.Count == 0)
                        throw new Exception("Excel file is empty!");

                    foreach (DataRow row in dataTable.Rows)
                    {
                        string purchaseOrderNumber = row["PurchaseOrder"]?.ToString()?.Trim();
                        if (string.IsNullOrEmpty(purchaseOrderNumber))
                            continue; // Skip rows with no PO number

                        // If we haven't seen this PurchaseOrder before, create a new header
                        if (!purchaseOrderDict.TryGetValue(purchaseOrderNumber, out var purchaseOrderHeader))
                        {
                            purchaseOrderHeader = new PurchaseOrderHeader
                            {
                                OUInstance = 2,
                                PurchaseOrder = purchaseOrderNumber,
                                NumberingType = row["NumberingType"].ToString(),
                                PODate = DateTime.TryParse(row["PODate"]?.ToString(), out var poDate) ? poDate : default,
                                POPriority = row["POPriority"]?.ToString()?.Trim(),
                                PartType = row["PartType"]?.ToString()?.Trim(),
                                Remarks1 = row["Remarks1"]?.ToString()?.Trim(),
                                Supplier = row["Supplier"]?.ToString()?.Trim(),
                                POCurrency = row["POCurrency"]?.ToString()?.Trim(),
                                AddressID = int.TryParse(row["AddressID"]?.ToString(), out var addressId) ? addressId : 0,
                                PoStatus = "D",
                                AmendNo = 0,
                                Role = "role",
                                User = "user",
                                Language = 1,
                                PODetailinfo = new List<PurchaseOrderDetail>()
                            };

                            purchaseOrderDict[purchaseOrderNumber] = purchaseOrderHeader;
                        }

                        // Create and add the purchase order detail
                        var purchaseOrderDetail = new PurchaseOrderDetail
                        {
                            OUInstance = 2,
                            Part = row["Part"]?.ToString()?.Trim(),
                            OrderQty = int.TryParse(row["OrderQty"]?.ToString(), out var orderQty) ? orderQty : 0,
                            PurchaseUOM = row["PurchaseUOM"]?.ToString()?.Trim(),
                            Cost = decimal.TryParse(row["Cost"]?.ToString(), out var cost) ? cost : 0,
                            CostPer = decimal.TryParse(row["CostPer"]?.ToString(), out var costPer) ? costPer : 0,
                            Condition = row["Condition"]?.ToString()?.Trim(),
                            CertificateType = row["CertificateType"]?.ToString()?.Trim(),
                            Warehouse = row["Warehouse"]?.ToString()?.Trim(),
                            Remarks2 = row["Remarks2"]?.ToString()?.Trim(),
                            Role = "role",
                            User = "user",
                            Language = 1
                        };

                        // Debug: Print the Part value before fixing any issues
                        Console.WriteLine($"Raw Part Value: {purchaseOrderDetail.Part}");

                        // Remove unwanted backslashes from Part
                        if (!string.IsNullOrEmpty(purchaseOrderDetail.Part))
                        {
                            purchaseOrderDetail.Part = purchaseOrderDetail.Part.Replace("\\", "");
                        }

                        purchaseOrderHeader.PODetailinfo.Add(purchaseOrderDetail);
                    }

                    // Add all headers to the final list
                    purchaseOrderHeaders = purchaseOrderDict.Values.ToList();
                }
            }

            // Serialize the purchaseOrderHeaders to JSON with ReferenceLoopHandling.Ignore
            var json = JsonConvert.SerializeObject(purchaseOrderHeaders, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            // Deserialize the JSON back to the list (if needed)
            var deserializedHeaders = JsonConvert.DeserializeObject<List<PurchaseOrderHeader>>(json);

            return deserializedHeaders;
        }
    }
}
