using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cafe_management.DAO
{
    public class DataProvider
    {
        private static DataProvider? instance; // ctrl + R + E
        private string connectionSTR = @"Data Source=DESKTOP-NF928OQ\SQLEXPRESS01;Initial Catalog=Quanlycoffe;Integrated Security=True;TrustServerCertificate=True"; 
        //private string connectionSTR = @"Data Source=localhost;Initial Catalog=Quanlycoffe;Integrated Security=True;TrustServerCertificate=True"; // chuoi xac dinh connect vs phan nao
        private DataProvider() { }
        public static DataProvider Instance
        {
            get { if (instance == null) instance = new DataProvider(); return DataProvider.instance; }
            private set { DataProvider.instance = value; }
        }
        /// <summary>
        /// Trả về 1 bảng dữ liệu
        /// Dùng cho các câu lệnh select
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        
        public DataTable ExcuteQuery(string query, SqlParameter[]? parameters = null)
        {
            DataTable data = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionSTR))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection); // truy van
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                SqlDataAdapter adapter = new SqlDataAdapter(command); // chung gian lay du lieu thuc hien cau truy van
                adapter.Fill(data);
                connection.Close();
            }
            return data;
        }
        /// <summary>
        /// Trả về số dòng bị ảnh hưởng
        /// Dùng cho insert, update, delete
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        
        public int ExecuteNonQuery(string query, SqlParameter[]? parameter = null)
        {
            int data = 0;
            using (SqlConnection connection = new SqlConnection(connectionSTR))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection); // truy van
                if (parameter != null)
                {
                    command.Parameters.AddRange(parameter);
                }
                data = command.ExecuteNonQuery();
                connection.Close();
            }
            return data;
        }
        /// <summary>
        /// Trả về giá trị của ô đầu tiên trong bảng dữ liệu
        /// dùng cho các hàm như count, sum, max, min
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public object ExecuteScalar(string query, SqlParameter[]? parameter = null)
        {
            object data = 0;
            using (SqlConnection connection = new SqlConnection(connectionSTR))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection); // truy van
                if (parameter != null)
                {
                    command.Parameters.AddRange(parameter);
                }
                data = command.ExecuteScalar();
                connection.Close();
            }
            return data;
        }
        
    }
}
