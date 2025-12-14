using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace cafe_management.DAO
{
    public class BillDAO
    {
        private static BillDAO? instance;
        public static BillDAO Instance
        {
            get { if (instance == null) instance = new BillDAO(); return instance; }
            private set { instance = value; }
        }
        private BillDAO() { }
        public bool CreateBill(int tableID, int status = 0)
        {
            string query = "INSERT INTO dbo.Bill(idtable, status) VALUES ( @tableID , @status )";

            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { tableID, status });

            return result > 0;
        }
        public int GetIdBillByTableID(int tableID, int status = 0)
        {
            string query = "select * from Bill where status = @status and idtable = @tableID ";
            object[] para = new object[] { status, tableID };
            object result = DataProvider.Instance.ExecuteScalar(query, para);
            if (result == null)
            {
                return -1; // Hoặc xử lý theo cách khác nếu không có giá trị
            }
            return (int)result;
        }
        public DateTime GetTimeCheckin(int tableID) 
        {
            string query = "select datecheckin from Bill where idtable =" + tableID;
            object result = DataProvider.Instance.ExecuteScalar(query);
            if (result == null)
            {
                return DateTime.MinValue; // Hoặc xử lý theo cách khác nếu không có giá trị
            }
            return (DateTime)result;
        }
        public void ChangeStatusBill(int idTable, int status)
        {
            string query = "UPDATE dbo.Bill SET status = @status WHERE idtable = @idTable ";
            DataProvider.Instance.ExecuteNonQuery(query, new object[] { status, idTable });
        }
        public int GetStatusBillByIdTable(int idtable)
        {
            string query = "SELECT TOP 1 status FROM Bill WHERE idtable = @idtable ORDER BY id DESC";
            object[] parameter = new object[] { idtable };
            object result = DataProvider.Instance.ExecuteScalar(query, parameter);

            if (result == null )
            {
                return -1;
            }

            return (int)result;
        }
        public bool DeleteBillData()
        {
            string query = "DELETE FROM Bill;\r\nDBCC CHECKIDENT ('Bill', RESEED, 0);\r\n";
            return DataProvider.Instance.ExecuteNonQuery(query) > 0;
        }
    }
}
