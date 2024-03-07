using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GIGRSystem
{
    public partial class Loginoptions : Form
    {
        public Loginoptions()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Login sf = new Login();
            sf.Show();
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GoodsIncoming gi = new GoodsIncoming();
            gi.Show();
            
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            GRForm grf = new GRForm();
            grf.Show();
        }
    }
}
