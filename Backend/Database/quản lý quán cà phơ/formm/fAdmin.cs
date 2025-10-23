using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace formm
{
    public partial class fAdmin : Form
    {
        public fAdmin()
        {
            InitializeComponent();

            LoadAccountList();

        }
        void LoadAccountList()
        {
            string connectionSTR = "Data Source=DESKTOP-NF928OQ.\\SQLEXPRESS01;Initial Catalog=Quanlycoffe;Integrated Security=True"; // chuoi xac dinh connect vs phan nao
            SqlConnection connection = new SqlConnection(connectionSTR); // ket noi vs sever
            string query = "SELECT * FROM dbo.Account";
            connection.Open();
            SqlCommand command = new SqlCommand(query, connection); // truy van
            DataTable data = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(command); // chung gian lay giu lieu thuc hien cau truy van
            adapter.Fill(data);
            connection.Close();
            dtgvAccount.DataSource = data;

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dtpkToDateBill_ValueChanged(object sender, EventArgs e)
        {

        }

        private void tpFood_Click(object sender, EventArgs e)
        {

        }

        private void btAddFood_Click(object sender, EventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel19_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void fAdmin_Load(object sender, EventArgs e)
        {

        }
    }
}
