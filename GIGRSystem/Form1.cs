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
    public partial class Login : Form
    {

        Class1 nc = new Class1();
        public Login()
        {
            InitializeComponent();
        }

      

        private void button1_Click(object sender, EventArgs e)
        {
            string existquery = "select username from mmioscredentials where username ='" + txtuser.Text + "' ";
            string passquery = "select password from mmioscredentials where username ='" + txtuser.Text + "' ";
            string queryname = "select fullname from mmioscredentials where username ='" + txtuser.Text + "' ";
            string name = nc.getdata(queryname);
            if (nc.dataExist(existquery) == true)
            {
                string password = nc.getdata(passquery);
                if (txtpass.Text == password)
                {
                    Loginoptions lo = new Loginoptions();
                    
                    lo.Show();
                    GoodsIncoming.receiver = name;
                    Issuance.issuer = name;


                }
                else
                {
                    MessageBox.Show("INCORRECT PASSWORD", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtpass.Focus();
                }
            }
            else
            {
                MessageBox.Show(txtuser.Text + " USERNAME DOESNT EXIST!! ", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtuser.Focus();
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            Signup sf = new Signup();
            sf.Show();
        }
    }
}
