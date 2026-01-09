using cafe_management.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
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
            string query = "SELECT f.id, f.name, f.price, c.name AS CategoryName\r\n" +
                "FROM Food f\r\n" +
                "JOIN FoodCategory c ON f.idcategory = c.id\r\n";
            var data = DataProvider.Instance.ExcuteQuery(query);
            foreach (DataRow row in data.Rows)
            {
                list.Add(new FoodItem(row));
            }
            return list;
        }
        public string FormatVND(decimal price)
        {
            return price.ToString("#,###", new System.Globalization.CultureInfo("vi-VN")) + "đ";
        }
        public FoodItem? GetFoodByID(int id)
        {
            string query = "SELECT * FROM Food WHERE id = @id";
            SqlParameter[] parameters =
            {
                new SqlParameter ("id", SqlDbType.Int) { Value = id },
            };
            DataTable dt = DataProvider.Instance.ExcuteQuery(query, parameters);

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
            SqlParameter[] parameter =
            {
                new SqlParameter("@nameFood", System.Data.SqlDbType.NVarChar) { Value = nameFood },
                new SqlParameter("@category", System.Data.SqlDbType.Int) { Value = category },
                new SqlParameter("@price", System.Data.SqlDbType.Float) { Value = price }
            };
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
            SqlParameter[] parameter =
            {
                new SqlParameter("@id", System.Data.SqlDbType.Int) { Value = id }
            };
            return DataProvider.Instance.ExecuteNonQuery(query, parameter) > 0;
        }
        public bool UpdateFood(int id, string nameFood, int category, float price)
        {
            string query = "UPDATE dbo.Food SET name = @nameFood , idcategory = @category , price = @price WHERE id = @id ";
            SqlParameter[] parameter =
            {
                new SqlParameter("@id", System.Data.SqlDbType.Int) { Value = id },
                new SqlParameter("@nameFood", System.Data.SqlDbType.NVarChar) { Value = nameFood },
                new SqlParameter("@category", System.Data.SqlDbType.Int) { Value = category },
                new SqlParameter("@price", System.Data.SqlDbType.Float) { Value = price }
            };
            return DataProvider.Instance.ExecuteNonQuery(query, parameter) > 0;
        }
    }
}
