using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;

public static class DataTableHelper
{
    public static DataTable ToDataTable<T>(IEnumerable<T> data)
    {
        var table = new DataTable();

        // Exclude properties marked with [NotMapped] and those we explicitly don't want (e.g.,"PurchaseOrderHeader")
        var props = typeof(T).GetProperties()
            .Where(p => p.GetCustomAttributes(typeof(NotMappedAttribute), true).Length == 0
                        && !string.Equals(p.Name, "PurchaseOrderHeader", StringComparison.OrdinalIgnoreCase)
                        && (p.PropertyType.IsPrimitive ||
                            p.PropertyType.IsValueType ||
                            p.PropertyType == typeof(string) ||
                            p.PropertyType == typeof(DateTime) ||
                            p.PropertyType == typeof(decimal)))
            .ToArray();

        foreach (var prop in props)
        {
            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        }

        foreach (var item in data)
        {
            var row = table.NewRow();
            foreach (var prop in props)
            {
                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
            }
            table.Rows.Add(row);
        }

        return table;
    }
}
