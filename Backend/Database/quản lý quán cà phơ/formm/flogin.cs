using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace formm
{
    public partial class flogin : Form
    {
        public flogin()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {
           

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //// Giả sử 2 textbox là txtUsername và txtPassword
            //string username = textBoxuser.Text.Trim();
            //string password = textboxpassword.Text.Trim();

            //// Tài khoản cố định
            //string correctUser = "tuyencoi123";
            //string correctPass = "password";

            //if (username == correctUser && password == correctPass)
            //{
            //    // Nếu đúng -> mở form chính
            //    MessageBox.Show("Đăng nhập thành công!");
                ftablemanager f = new ftablemanager();
                this.Hide(); // ẩn tab đăng nhập 
                f.ShowDialog(); // show
            //    this.Show(); // mở lại tab đăng nhập
            //}
            //else
            //{
            //    // Nếu sai -> báo lỗi
            //    MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void flogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn thoát chương trình?", "Thông Báo", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
            {
                e.Cancel = true;
            }
        }

        private void flogin_Load(object sender, EventArgs e)
        {

        }
    }
}
