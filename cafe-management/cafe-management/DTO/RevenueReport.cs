using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cafe_management.DTO
{
    public class RevenueReport
    {
        public string BillId { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public DateTime Time { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
    }
}
