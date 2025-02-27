using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class PurchaseOrderDetail
    {
        public int ID { get; set; }
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


        [NotMapped]
        [JsonIgnore]
        public PurchaseOrderHeader PurchaseOrderHeader { get; set; }
    }
}
