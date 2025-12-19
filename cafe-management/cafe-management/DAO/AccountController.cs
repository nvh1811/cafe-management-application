using cafe_management.Helper;
using cafe_management.DAO;
using cafe_management.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
namespace cafe_management.DAO
{
    public class AccountController
    {
        private static AccountController? instance;

        public static AccountController Instance
        {
            get { if (instance == null) instance = new AccountController(); return instance; }
            private set { instance = value; }
        }
        private AccountController() { }
        public Account Login(string username, string password)
        {
            string query = "SELECT * FROM dbo.Account WHERE username = @username AND password = @password";

            SqlParameter[] parameters =
            {
                new SqlParameter("@username", SqlDbType.NVarChar) {Value = username },
                new SqlParameter("@password", SqlDbType.NVarChar) {Value = password }
            };

            DataTable result = DataProvider.Instance.ExcuteQuery(query, parameters);

            if (result.Rows.Count == 0)
                return null;

            DataRow row = result.Rows[0];

            Account acc =  new Account
            {
                Username = row["username"].ToString(),
                Role = row["role"].ToString()
            };
            UserSession.CurrentUser = acc; // ⭐ QUAN TRỌNG
            return acc;
        }

    }
}
