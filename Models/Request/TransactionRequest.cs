﻿using System.Text.Json.Serialization;

namespace WebApplication1.Models.Request
{
    public class TransactionRequest
    {
        public string PartnerKey { get; set; }
        public string PartnerRefNo { get; set; }
        public string PartnerPassword { get; set; }
        public long TotalAmount { get; set; }
        public List<ItemDetail> Items { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Sig { get; set; }
    }

    public class ItemDetail
    {
        public string PartnerItemRef { get; set; }
        public string Name { get; set; }
        public int Qty { get; set; }
        public long UnitPrice { get; set; }
    }
}
