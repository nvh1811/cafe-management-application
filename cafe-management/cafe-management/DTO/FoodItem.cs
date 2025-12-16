using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cafe_management.DTO
{
    public class FoodItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        

        public FoodItem() { }
        public FoodItem(DataRow row)
        {
            Id = Convert.ToInt32(row["id"]);
            Name = row["name"].ToString()!;
            Category = row["CategoryName"].ToString()!;
            Price = Convert.ToDecimal(row["price"]);
        }
    }
}
