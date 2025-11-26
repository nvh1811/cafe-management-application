using cafe_management.DAO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public bool login(string username, string password)
        {
            string query = "SELECT * FROM dbo.Account WHERE username = N'" + username + "' AND password = N'" + password + "'";
            DataTable result = DataProvider.Instance.ExecuteQuery(query);
            return result.Rows.Count > 0;
        }
    }
}
