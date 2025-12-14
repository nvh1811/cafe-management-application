using cafe_management.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cafe_management.DAO
{
    public class FoodDAO
    {
        private static FoodDAO? instance;
        public static FoodDAO Instance
        {
            get { if (instance == null) instance = new FoodDAO(); return instance; }
            private set { instance = value; }
        }
        private FoodDAO() { }
        public List<FoodItem> GetListFood()
        {
            List<FoodItem> list = new List<FoodItem>();
            string query = "SELECT id, name, idcategory, price FROM dbo.Food";
            var data = DataProvider.Instance.ExecuteQuery(query);
            foreach (System.Data.DataRow row in data.Rows)
            {
                string categoryName = FoodCategoryDAO.Instance.GetNameCategoryById((int)row["idcategory"]);
                list.Add(new FoodItem
                {
                    Id = (int)row["id"],
                    Name = Convert.ToString(row["name"])!,
                    Category = categoryName,
                    Price = Convert.ToDecimal(row["price"])
                });
            }
            return list;
        }
        public string FormatVND(decimal price)
        {
            return price.ToString("#,###", new System.Globalization.CultureInfo("vi-VN")) + "đ";
        }
        public List<FoodItem> GetFoodByCategory(string category)
        {
            return GetListFood().Where(food => food.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        public List<FoodItem> SearchFoodByName(string name)
        {
            return GetListFood().Where(food => food.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }
        public FoodItem? GetFoodByID(int id)
        {
            string query = "SELECT * FROM Food WHERE id = " + id;
            DataTable dt = DataProvider.Instance.ExecuteQuery(query);

            if (dt.Rows.Count > 0)
                return new FoodItem
                {
                    Id = id,
                    Name = Convert.ToString(dt.Rows[0]["name"])!,
                    Category = Convert.ToString(dt.Rows[0]["idcategory"])!,
                    Price = Convert.ToDecimal(dt.Rows[0]["price"])
                };

            return null;
        }
        public bool InsertFood(string nameFood, int category, float price)
        {
            string query = "INSERT INTO Food (name, idcategory, price) VALUES ( @nameFood , @category , @price )";
            object[] parameter = new object[] { nameFood, category, price };
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameter);
            return result > 0;
        }
        public int GetFoodCount()
        {
            string query = "SELECT COUNT(*) FROM dbo.Food";
            object result = DataProvider.Instance.ExecuteScalar(query);
            return Convert.ToInt32(result);
        }
        public bool DeleteFood(int id)
        {
            string query = "";
            if (GetFoodCount() <= 1)
            {
                query = "DELETE FROM Food;\r\nDBCC CHECKIDENT ('Food', RESEED, 0);\r\n";
            }
            else query = "DELETE FROM dbo.Food WHERE id = @id";
            BillInfoDAO.Instance.DeleteBillInfoData();
            BillDAO.Instance.DeleteBillData();
            object[] parameter = new object[] { id };
            return DataProvider.Instance.ExecuteNonQuery(query, parameter) > 0;
        }
        public bool UpdateFood(int id, string nameFood, int category, float price)
        {
            string query = "UPDATE dbo.Food SET name = @nameFood , idcategory = @category , price = @price WHERE id = @id ";
            object[] parameter = new object[] { nameFood, category, price, id };
            return DataProvider.Instance.ExecuteNonQuery(query, parameter) > 0;
        }
    }
}
