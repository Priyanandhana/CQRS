using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class PurchaseOrderDTO
    {
        public int OUInstance { get; set; }
        public string PurchaseOrder { get; set; }
        public string NumberingType { get; set; }
        public DateTime PODate { get; set; }
        public string POPriority { get; set; }
        public string PartType { get; set; }
        public string Remarks1 { get; set; }
        public string Supplier { get; set; }
        public string POCurrency { get; set; }
        public int AddressID { get; set; }
        public string PoStatus { get; set; }
        public int AmendNo { get; set; }
        public string Role { get; set; }
        public string User { get; set; }
        public int Language { get; set; }

        // List of PurchaseOrderDetails as part of the same DTO
        public List<PurchaseOrderDetailDTO> PurchaseOrderDetails { get; set; }
    }

    public class PurchaseOrderDetailDTO
    {
        public int Id { get; set; }  // Primary key
        public int OUInstance { get; set; }
        public string Part { get; set; }
        public int OrderQty { get; set; }
        public string PurchaseUOM { get; set; }
        public decimal Cost { get; set; }
        public decimal CostPer { get; set; }
        public string Condition { get; set; }
        public string CertificateType { get; set; }
        public string Warehouse { get; set; }
        public string Remarks2 { get; set; }
        public string Role { get; set; }
        public string User { get; set; }
        public int Language { get; set; }
    }
}

