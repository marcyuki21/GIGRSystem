using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Data;


namespace GIGRSystem
{
    public partial class Signup : Form
    {
        Class1 nc = new Class1();
        public Signup()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Login sf = new Login();
            sf.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string fnamequery = "Select fullname from mmioscredentials where fullname='" + txtfname.Text + "' ";
            string userquery = "Select username from mmioscredentials where username ='" + txtusername.Text + "'  ";

            if (nc.dataExist(fnamequery) == true)
            {
                MessageBox.Show(txtfname.Text + " FULLNAME ALREADY EXIST!! ", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (nc.dataExist(userquery) == true)
            {
                MessageBox.Show(txtusername.Text + " USERNAME ALREADY EXIST!! ", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (txtpassword.Text != txtconfpassword.Text)
            {
                MessageBox.Show("Password is not match! Please type a correct password !!", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (txtfname.Text == "" || txtusername.Text =="" || txtposition.Text =="" || txtpassword.Text =="" || txtconfpassword.Text == "")
            {
                MessageBox.Show("Kindly fill up all details!!", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string iquery = "insert into mmioscredentials(fullname,position,username,password)values('"+txtfname.Text+"','"+txtposition.Text+ "','" + txtusername.Text + "','" + txtpassword.Text + "')";
                nc.iData(iquery);
                MessageBox.Show("SUCCESSFULLY REGISTERED!!", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtfname.Text = "";
                txtusername.Text = "";
                txtposition.Text = "";
                txtpassword.Text ="";
                txtconfpassword.Text = "";


            }
        

        }
    }
}
