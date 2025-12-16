using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static cafe_management.UI.HomeLayout;

namespace cafe_management.DTO
{
    public class Bill
    {
        public string BillId { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public int TableId { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; }

        public int TotalBill { get; set; } = 0;
        public Bill() { }

        public Bill(DataRow row)
        {
            BillId = row["id"].ToString()!;
            TableId = Convert.ToInt32(row["idtable"]);
            TableName = row.Table.Columns.Contains("TableName")
                        ? row["TableName"].ToString()!
                        : "";
            StartTime = Convert.ToDateTime(row["datecheckin"]);
            EndTime = row["datecheckout"] != DBNull.Value ? Convert.ToDateTime(row["datecheckout"]) : DateTime.MinValue;
            TotalBill = Convert.ToInt32(row["totalprice"]);
        }
    }
}
