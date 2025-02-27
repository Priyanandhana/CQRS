using System.Data;
using System.IO;
using System.Xml;
using ExcelDataReader;
using Newtonsoft.Json;

public class ExcelProcessingService
{
    public string ConvertExcelToJson(Stream excelStream)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        using (var reader = ExcelReaderFactory.CreateReader(excelStream))
        {
            var result = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }
            });

            DataTable dataTable = result.Tables[0]; // Assuming the first sheet is used
            var jsonResult = JsonConvert.SerializeObject(dataTable, Newtonsoft.Json.Formatting.Indented);
            return jsonResult;
        }
    }
}
