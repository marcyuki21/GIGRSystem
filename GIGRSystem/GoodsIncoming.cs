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
    public partial class GoodsIncoming : Form
    {

        Class1 nc = new Class1();
        public static string gimodel;

        public MySqlConnection mycon;
        public MySqlCommand mycom;
        public MySqlDataReader myread;
        public MySqlDataAdapter myadapt;
        public string connectionFurukawa = "Server=192.168.4.136;Port=3309;Database=furukawa;User ID=root;Password=123;";
        public string connectionFormal = "Server=192.168.4.136;Port=3309;Database=formal;User ID=root;Password=123;";
        public string connection137 = "Server=192.168.4.137;Port=3306;Database=mmios;User ID=root;Password=pass1234;";

        public GoodsIncoming()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        private void GoodsIncoming_Load(object sender, EventArgs e)
        {
            loadmodel();
            loadhpnumber();
        }

        public void loadmodel()
        {
            string query = "select distinct model from mmiosmodel";
            try
            {
                mycon = new MySqlConnection(connection137);
                mycon.Open();
                mycom = new MySqlCommand(query, mycon);
                myread = mycom.ExecuteReader();
                while (myread.Read())
                {
                    cbModel.Items.Add(myread.GetValue(0).ToString());


                }
                mycon.Close();
                mycon.Dispose();
            }
            catch (Exception)
            {
                mycon.Close();
                mycon.Dispose();
                throw;
            }
        }
        public void loadhpnumber()
        {
            string query = "select distinct(hpnumber) from mmiosmodel where model='" + cbModel.Text + "' ";
            try
            {
                mycon = new MySqlConnection(connection137);
                mycon.Open();
                mycom = new MySqlCommand(query, mycon);
                myread = mycom.ExecuteReader();
                while (myread.Read())
                {
                    cbHPnumber.Items.Add(myread.GetValue(0).ToString());

                }
                mycon.Close();
                mycon.Dispose();
            }
            catch (Exception)
            {
                mycon.Close();
                mycon.Dispose();
                throw;
            }
        }



        private void cbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            string query = "select model from mmiosmodel where model ='" + cbModel.Text + "' ";
            gimodel = nc.getdata(query);
            loadhpnumber();

        }


        private void cbHPnumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            string queryhp = "select hpcode from mmiosmodel where model ='" + gimodel + "' and hpnumber = '" + cbHPnumber.Text + "' ";
            txthpcode.Text = nc.getdata(queryhp);
        }
    }
}
