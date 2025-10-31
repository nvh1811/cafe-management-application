namespace quan_li_quan_cafe
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
        }

        private void txbusername_TextChanged(object sender, EventArgs e)
        {

        }

        private void btLogin_Click(object sender, EventArgs e)
        {
            TableManager t = new TableManager();
            this.Hide();
            t.ShowDialog();
            this.Show();
        }

        private void btExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void login_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có thực sự muốn thoát?", "Thông báo", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
            {
                e.Cancel = true;
            }
        }
    }
}