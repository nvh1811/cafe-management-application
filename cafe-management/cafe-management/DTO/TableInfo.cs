using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace cafe_management.DTO
{
    // Class lưu thông tin bàn
    public class TableInfo
    {
        public int TableId { get; set; }
        public string TableName { get; set; } = string.Empty;
        public string Status { get; set; } = "Trống";
        public int Seats { get; set; } = 4;
        public Color StatusColor { get; set; }//= Colors.Green;
    }
}
