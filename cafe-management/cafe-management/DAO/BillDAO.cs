using cafe_management.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
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

            SqlParameter[] parameters =
            {
                new SqlParameter("@tableID", SqlDbType.Int) { Value = tableID },
                new SqlParameter("@status", SqlDbType.Int) { Value = status }
            };  

            int result = DataProvider.Instance.ExecuteNonQuery(query, parameters);

            return result > 0;
        }
        public int GetIdBillByTableID(int tableID, int status = 0)
        {
            string query = "select * from Bill where status = @status and idtable = @tableID ";
            SqlParameter[] parameters = 
            {
                new SqlParameter("status", SqlDbType.Int) { Value = status },
                new SqlParameter("idtable", SqlDbType.Int) {Value = tableID}
            };
            object result = DataProvider.Instance.ExecuteScalar(query, parameters);
            if (result == null)
            {
                return -1; // Hoặc xử lý theo cách khác nếu không có giá trị
            }
            return (int)result;
        }
        public DateTime GetTimeCheckin(int tableID)
        {
            string query = "select datecheckin from Bill where idtable = @tableID";
            SqlParameter[] parameters = 
            { 
                new SqlParameter ("idtable", SqlDbType.DateTime) { Value = tableID },
            };
            object result = DataProvider.Instance.ExecuteScalar(query, parameters);
            if (result == null)
            {
                return DateTime.MinValue; // Hoặc xử lý theo cách khác nếu không có giá trị
            }
            return (DateTime)result;
        }
        public DateTime GetTimeCheckout(int tableID) 
        {
            string query = "select datecheckout from Bill where idtable = @tableID";
            SqlParameter[] parameters =
            {
                new SqlParameter ("idtable", SqlDbType.DateTime) { Value = tableID },
            };
            object result = DataProvider.Instance.ExecuteScalar(query, parameters);
            if (result == null)
            {
                return DateTime.MinValue; // Hoặc xử lý theo cách khác nếu không có giá trị
            }
            return (DateTime)result;
        }
        public void UpdateBill(int idTable, int status, DateTime checkoutTime)
        {
            string query = "UPDATE dbo.Bill SET datecheckout = @checkoutTime, status = @status WHERE idtable = @idTable ";
            SqlParameter[] parameters =
            {
                new SqlParameter("@checkoutTime", SqlDbType.DateTime) { Value = checkoutTime },
                new SqlParameter("@status", SqlDbType.Int) { Value = status },
                new SqlParameter("@idTable", SqlDbType.Int) { Value = idTable }
            };
            DataProvider.Instance.ExecuteNonQuery(query, parameters);
        }
        public int GetStatusBillByIdTable(int idtable)
        {
            string query = "SELECT TOP 1 status FROM Bill WHERE idtable = @idtable ORDER BY id DESC";
            SqlParameter[] parameters =
            {
                new SqlParameter ("idtable", SqlDbType.DateTime) { Value = idtable },
            };
            object result = DataProvider.Instance.ExecuteScalar(query, parameters);

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
        public List<Bill> GetListBillRange(DateTime start, DateTime end)
        {
            string query = @"
                            SELECT 
                                b.id,
                                b.datecheckin,
                                b.datecheckout,
                                b.idtable,
                                t.name AS TableName,
                                SUM(f.price * bi.count) AS TotalPrice
                            FROM Bill b
                            JOIN BillInfo bi   ON bi.idbill = b.id
                            JOIN Food f        ON bi.idfood = f.id
                            JOIN TableFood t   ON b.idtable = t.id
                            WHERE b.datecheckin >= @start
                            AND b.datecheckin <  @end
                            AND b.status = 1
                            GROUP BY 
                                b.id,
                                b.datecheckin,
                                b.datecheckout,
                                b.idtable,
                                t.name;
                                ";

            SqlParameter[] parameters =
                                       {
                                            new SqlParameter("@start", SqlDbType.DateTime) { Value = (SqlDateTime)start.Date },
                                            new SqlParameter("@end",   SqlDbType.DateTime) { Value = (SqlDateTime)end.Date.AddDays(1) }
                                        };
            
            var data = DataProvider.Instance.ExcuteQuery(query, parameters);

            List<Bill> list = new List<Bill>();
            foreach (DataRow row in data.Rows)
            {
                list.Add(new Bill(row));
            }

            return list;
        }
    }
}
