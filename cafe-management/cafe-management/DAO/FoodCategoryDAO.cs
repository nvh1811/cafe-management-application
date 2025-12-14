using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cafe_management.DAO
{
    public class FoodCategoryDAO
    {
        private static FoodCategoryDAO? instance;
        public static FoodCategoryDAO Instance
        {
            get { if (instance == null) instance = new FoodCategoryDAO(); return instance; }
            private set { instance = value; }
        }
        private FoodCategoryDAO() { }
        public List<string> GetListFoodCategory()
        {
            List<string> list = new List<string>();
            string query = "SELECT name FROM dbo.FoodCategory";
            var data = DataProvider.Instance.ExecuteQuery(query);
            foreach (System.Data.DataRow row in data.Rows)
            {
                list.Add(Convert.ToString(row["name"])!);
            }
            return list;
        }
        public int GetIdCategoryByName(string name)
        {
            string query = "SELECT id FROM dbo.FoodCategory WHERE name = N'" + name + "'";
            var data = DataProvider.Instance.ExecuteQuery(query);
            if (data.Rows.Count > 0)
            {
                return (int)data.Rows[0]["id"];
            }
            return -1; // Not found
        }
        public string GetNameCategoryById(int id)
        {
            string query = "SELECT name FROM dbo.FoodCategory WHERE id = " + id;
            var data = DataProvider.Instance.ExecuteQuery(query);
            if (data.Rows.Count > 0)
            {
                return Convert.ToString(data.Rows[0]["name"])!;
            }
            return string.Empty; // Not found
        }
    }
}
