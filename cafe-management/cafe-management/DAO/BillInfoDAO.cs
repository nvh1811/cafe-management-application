using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace cafe_management.DAO
{
    public class BillInfoDAO
    {
        private static BillInfoDAO? instance;
        public static BillInfoDAO Instance
        {
            get { if (instance == null) instance = new BillInfoDAO(); return instance; }
            private set { instance = value; }
        }
        private BillInfoDAO() { }
        public bool AddBillInfo(int billID, int foodID, int count)
        {
            string query = @"
        INSERT INTO dbo.BillInfo(idbill, idfood, count)
        VALUES ( @billID , @foodID , @count )";

            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { billID, foodID, count });

            return result > 0;
        }
        public bool DeleteBillInfoData()
        {
            string query = "DELETE FROM BillInfo;\r\nDBCC CHECKIDENT ('BillInfo', RESEED, 0);\r\n";
            return DataProvider.Instance.ExecuteNonQuery(query) > 0;
        }
    }
}
