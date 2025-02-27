using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class PurchaseOrderHeader
    {
        [Key]
        public int ID { get; set; }  

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
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property for PurchaseOrderDetail
        public ICollection<PurchaseOrderDetail> PODetailinfo { get; set; } = new List<PurchaseOrderDetail>();
    }
}
