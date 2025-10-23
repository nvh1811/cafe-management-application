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
    public partial class ftablemanager : Form
    {
        public ftablemanager()
        {
            InitializeComponent();
        }

        private void ftablemanager_Load(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void đăngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //this.Hide();
            this.Close();
        }

        private void flowLayoutPaneltablefood_Paint(object sender, PaintEventArgs e)
        {

        }

        private void thôngTinCáNhânToolStripMenuItem_Click(object sender, EventArgs e)
        {
            accountprofile f = new accountprofile();
            f.ShowDialog();
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAdmin f = new fAdmin();
            f.ShowDialog();

        }
    }
}
