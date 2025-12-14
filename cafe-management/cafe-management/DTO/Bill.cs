using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static cafe_management.UI.HomeLayout;

namespace cafe_management.DTO
{
    public class Bill
    {
        public string BillId { get; set; } = string.Empty;
        public int TableId { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public ObservableCollection<BillItem> Items { get; set; } = new ObservableCollection<BillItem>();

        public decimal TotalAmount => Items.Sum(item => item.Price * item.Quantity);
    }
}
