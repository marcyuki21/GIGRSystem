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
    public partial class GoodsIncoming : Form
    {

        Class1 nc = new Class1();
        public static string gimodel;



        public GoodsIncoming()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        private void cbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            string query = "select ";
            gimodel = nc.getdata(query);
        }
    }
}
