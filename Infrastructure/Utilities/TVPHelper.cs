using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities;

namespace Infrastructure.Helpers
{
    public static class TVPHelper
    {
        public static DataTable ToPurchaseOrderHeaderDataTable(List<PurchaseOrderHeader> headers)
        {
            var table = new DataTable();
            // Define exact columns in TVP for headers (15 columns)
            table.Columns.Add("OUInstance", typeof(int));
            table.Columns.Add("PurchaseOrder", typeof(string));
            table.Columns.Add("NumberingType", typeof(string));
            table.Columns.Add("PODate", typeof(DateTime));
            table.Columns.Add("POPriority", typeof(string));
            table.Columns.Add("PartType", typeof(string));
            table.Columns.Add("Remarks1", typeof(string));
            table.Columns.Add("Supplier", typeof(string));
            table.Columns.Add("POCurrency", typeof(string));
            table.Columns.Add("AddressID", typeof(int));
            table.Columns.Add("PoStatus", typeof(string));
            table.Columns.Add("AmendNo", typeof(int));
            table.Columns.Add("Role", typeof(string));
            table.Columns.Add("User", typeof(string));
            table.Columns.Add("Language", typeof(int));

            foreach (var header in headers)
            {
                var row = table.NewRow();
                row["OUInstance"] = header.OUInstance;
                row["PurchaseOrder"] = string.IsNullOrEmpty(header.PurchaseOrder) ? (object)DBNull.Value : header.PurchaseOrder;
                row["NumberingType"] = string.IsNullOrEmpty(header.NumberingType) ? (object)DBNull.Value : header.NumberingType;
                row["PODate"] = header.PODate;
                row["POPriority"] = string.IsNullOrEmpty(header.POPriority) ? (object)DBNull.Value : header.POPriority;
                row["PartType"] = string.IsNullOrEmpty(header.PartType) ? (object)DBNull.Value : header.PartType;
                row["Remarks1"] = string.IsNullOrEmpty(header.Remarks1) ? (object)DBNull.Value : header.Remarks1;
                row["Supplier"] = string.IsNullOrEmpty(header.Supplier) ? (object)DBNull.Value : header.Supplier;
                row["POCurrency"] = string.IsNullOrEmpty(header.POCurrency) ? (object)DBNull.Value : header.POCurrency;
                row["AddressID"] = header.AddressID;
                row["PoStatus"] = string.IsNullOrEmpty(header.PoStatus) ? (object)DBNull.Value : header.PoStatus;
                row["AmendNo"] = header.AmendNo;
                row["Role"] = string.IsNullOrEmpty(header.Role) ? (object)DBNull.Value : header.Role;
                row["User"] = string.IsNullOrEmpty(header.User) ? (object)DBNull.Value : header.User;
                row["Language"] = header.Language;
                table.Rows.Add(row);
            }
            return table;
        }

        public static DataTable ToPurchaseOrderDetailDataTable(List<PurchaseOrderDetail> details)
        {
            var table = new DataTable();
            // Define exact columns in TVP for details (13 columns)
            table.Columns.Add("OUInstance", typeof(int));
            table.Columns.Add("Part", typeof(string));
            table.Columns.Add("OrderQty", typeof(int));
            table.Columns.Add("PurchaseUOM", typeof(string));
            table.Columns.Add("Cost", typeof(decimal));
            table.Columns.Add("CostPer", typeof(decimal));
            table.Columns.Add("Condition", typeof(string));
            table.Columns.Add("CertificateType", typeof(string));
            table.Columns.Add("Warehouse", typeof(string));
            table.Columns.Add("Remarks2", typeof(string));
            table.Columns.Add("Role", typeof(string));
            table.Columns.Add("User", typeof(string));
            table.Columns.Add("Language", typeof(int));

            foreach (var detail in details)
            {
                var row = table.NewRow();
                row["OUInstance"] = detail.OUInstance;
                row["Part"] = string.IsNullOrEmpty(detail.Part) ? (object)DBNull.Value : detail.Part;
                row["OrderQty"] = detail.OrderQty;
                row["PurchaseUOM"] = string.IsNullOrEmpty(detail.PurchaseUOM) ? (object)DBNull.Value : detail.PurchaseUOM;
                row["Cost"] = detail.Cost;
                row["CostPer"] = detail.CostPer;
                row["Condition"] = string.IsNullOrEmpty(detail.Condition) ? (object)DBNull.Value : detail.Condition;
                row["CertificateType"] = string.IsNullOrEmpty(detail.CertificateType) ? (object)DBNull.Value : detail.CertificateType;
                row["Warehouse"] = string.IsNullOrEmpty(detail.Warehouse) ? (object)DBNull.Value : detail.Warehouse;
                row["Remarks2"] = string.IsNullOrEmpty(detail.Remarks2) ? (object)DBNull.Value : detail.Remarks2;
                row["Role"] = string.IsNullOrEmpty(detail.Role) ? (object)DBNull.Value : detail.Role;
                row["User"] = string.IsNullOrEmpty(detail.User) ? (object)DBNull.Value : detail.User;
                row["Language"] = detail.Language;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
