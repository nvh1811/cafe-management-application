using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cafe_management.DTO
{
    public class OrderItem
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public decimal TotalPrice => Price * Quantity;

        public OrderItem() { }

        public OrderItem(DataRow row)
        {
            ItemId = (int)row["id"];
            ItemName = Convert.ToString(row["name"])!;
            Price = Convert.ToDecimal(row["price"]);
            Quantity = (int)row["count"];
        }
    }
}
