using System.Data;
using System.IO;
using System.Collections.Generic;
using ExcelDataReader;
using Newtonsoft.Json;
using Domain.Entities; 
public class ExcelService
{
    public List<PurchaseOrderHeader> ReadExcelFile(Stream fileStream)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        using var reader = ExcelReaderFactory.CreateReader(fileStream);
        var result = reader.AsDataSet();

        var table = result.Tables[0]; // Assuming data is in the first sheet

        List<PurchaseOrderHeader> orders = new List<PurchaseOrderHeader>();

        for (int i = 1; i < table.Rows.Count; i++) // Skipping header row
        {
            var row = table.Rows[i];
            var order = new PurchaseOrderHeader
            {
                OUInstance = Convert.ToInt32(row[0]),
                PurchaseOrder = row[1].ToString(),
                NumberingType = row[2].ToString(),
                PODate = Convert.ToDateTime(row[3]),
                POPriority = row[4].ToString(),
                PartType = row[5].ToString(),
                Remarks1 = row[6].ToString(),
                Supplier = row[7].ToString(),
                POCurrency = row[8].ToString(),
                AddressID = Convert.ToInt32(row[9]),
                PODetailinfo = new List<PurchaseOrderDetail>
                {
                    new PurchaseOrderDetail
                    {
                        Part = row[10].ToString(),
                        OrderQty = Convert.ToInt32(row[11]),
                        PurchaseUOM = row[12].ToString(),
                        Cost = Convert.ToDecimal(row[13]),
                        CostPer = Convert.ToInt32(row[14]),
                        Condition = row[15].ToString(),
                        CertificateType = row[16].ToString(),
                        Warehouse = row[17].ToString(),
                        Remarks2 = row[18].ToString()
                    }
                }
            };
            orders.Add(order);
        }

        return orders;
    }

    public string ConvertToJson(List<PurchaseOrderHeader> orders)
    {
        return JsonConvert.SerializeObject(orders, Formatting.Indented);
    }
}
