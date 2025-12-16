using cafe_management.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
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
            SqlParameter[] parameter = new SqlParameter[]
            {
                new SqlParameter("@name", System.Data.SqlDbType.NVarChar) { Value = name },
                new SqlParameter("@status", System.Data.SqlDbType.NVarChar) { Value = status }
            };

            // Sử dụng ExecuteNonQuery
            int result = DataProvider.Instance.ExecuteNonQuery(query, parameter);

            return result > 0;
        }
        // Trong TableFoodDAO.cs
        public List<TableInfo> GetListTable()
        {
            List<TableInfo> list = new List<TableInfo>();
            string query = "SELECT id, name, status FROM dbo.TableFood";
            DataTable data = DataProvider.Instance.ExcuteQuery(query);

            foreach (DataRow row in data.Rows)
            {
                list.Add(new TableInfo(row));
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
            SqlParameter[] parameter = new SqlParameter[] { new SqlParameter("@id", System.Data.SqlDbType.Int) { Value = id } };
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
            SqlParameter[] para = new SqlParameter[]
            {
                new SqlParameter("@status", System.Data.SqlDbType.NVarChar) { Value = status },
                new SqlParameter("@id", System.Data.SqlDbType.Int) { Value = id }
            };
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
            SqlParameter[] parameter = new SqlParameter[]
            {
                new SqlParameter("@idtable", System.Data.SqlDbType.Int) { Value = idtable }
            };
            DataTable data = DataProvider.Instance.ExcuteQuery(query, parameter);
            foreach (DataRow row in data.Rows)
            {
                list.Add(new OrderItem(row));
            }
            return list;
        }
        public string GetTableNameById(int id)
        {
            string query = "SELECT name FROM dbo.TableFood WHERE id = @id";
            object[] parameter = new object[] { id };
            object result = DataProvider.Instance.ExecuteScalar(query, parameter);
            return Convert.ToString(result) ?? string.Empty;
        }
    }
}
