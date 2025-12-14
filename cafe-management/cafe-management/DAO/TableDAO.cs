using cafe_management.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
namespace cafe_management.DAO
{
    public class TableDAO
    {
        private static TableDAO? instance;
        public static TableDAO Instance
        {
            get { if (instance == null) instance = new TableDAO(); return instance; }
            private set { instance = value; }
        }
        private TableDAO() { }

        public bool InsertTable(string name, string status)
        {
            // Cột name và status chấp nhận null/default, nhưng ta chèn giá trị rõ ràng.
            // Đảm bảo tên tham số (@name, @status) được sử dụng.
            string query = "INSERT INTO dbo.TableFood (name, status) VALUES ( @name , @status )";

            // Mảng tham số phải chứa các giá trị C# tương ứng với @name, @status.
            object[] parameter = new object[] { name, status };

            // Sử dụng ExecuteNonQuery
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameter);

            return result > 0;
        }
        // Trong TableFoodDAO.cs
        public List<TableInfo> GetListTable()
        {
            List<TableInfo> list = new List<TableInfo>();
            string query = "SELECT id, name, status FROM dbo.TableFood";
            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow row in data.Rows)
            {
                string status = Convert.ToString(row["status"])!;
                Color color;

                // Gán màu dựa trên trạng thái (Tùy chỉnh)
                if (status.Contains("khách"))   // chỉ cần chứa chữ "khách"
                    color = Colors.PaleVioletRed;
                else
                    color = Colors.Green;

                list.Add(new TableInfo
                {
                    TableId = (int)row["id"],
                    TableName = Convert.ToString(row["name"])!,
                    Status = status,
                    StatusColor = color // Gán Brush đã tính toán
                });
            }
            return list;
        }
        public int GetTableCount()
        {
            string query = "SELECT COUNT(*) FROM dbo.TableFood";
            object result = DataProvider.Instance.ExecuteScalar(query);
            return Convert.ToInt32(result);
        }   
        public bool DeleteTable(int id)
        {
            string query = "";
            if (GetTableCount() <= 1)
            {
                query = "DELETE FROM TableFood;\r\nDBCC CHECKIDENT ('TableFood', RESEED, 0);\r\n";
            }
            else query = "DELETE FROM dbo.TableFood WHERE id = @id";
            object[] parameter = new object[] { id };
            return DataProvider.Instance.ExecuteNonQuery(query, parameter) > 0;
        }
        public int GetMaxTableID()
        {
            string query = "SELECT MAX(id) FROM dbo.TableFood";
            object result = DataProvider.Instance.ExecuteScalar(query);
            return result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }
        public int GetMinTableID() 
        { 
            string query = "SELECT MIN(id) FROM dbo.TableFood";
            object result = DataProvider.Instance.ExecuteScalar(query);
            return result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }
        public bool UpdateTableStatus(int id, string status)
        {
            string query = "UPDATE dbo.TableFood SET status = @status WHERE id = @id ";
            object[] para = new object[] { status, id };
            return DataProvider.Instance.ExecuteNonQuery(query, para) > 0;
        }
        public List<OrderItem> GetListFoodOrderByIdTable(int idtable)
        {
            List<OrderItem> list = new List<OrderItem>();
            string query = "SELECT f.id, f.name, f.price, od.count" +
                "\r\nFROM dbo.Food f" +
                "\r\nJOIN dbo.BillInfo od ON f.id = od.idfood" +
                "\r\nJOIN dbo.Bill o ON od.idbill = o.id" +
                "\r\nWHERE o.idtable = @idtable AND o.status = 0"; // Chỉ lấy các đơn hàng chưa thanh toán
            object[] parameter = new object[] { idtable };
            DataTable data = DataProvider.Instance.ExecuteQuery(query, parameter);
            foreach (DataRow row in data.Rows)
            {
                list.Add(new OrderItem
                {
                    ItemId = (int)row["id"],
                    ItemName = Convert.ToString(row["name"])!,
                    Price = Convert.ToDecimal(row["price"]),
                    Quantity = (int)row["count"]
                });                
            }
            return list;
        }
    }
}
