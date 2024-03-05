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
using System.IO.Ports;

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
        public static string receiver;

        private SerialPort _serialPort = null;

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
            loadnames();

            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
            {
                comboBox2.Items.Add(port);
            }
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
        public string loadseries(string model)
        {
            string srs = string.Empty;
            string query = "select series from mmiosseries where model = '" + model + "' and pdate = '" + DateTime.Now.ToString("yyyy-MM-dd") + "'";
            try
            {
                mycon = new MySqlConnection(connection137);
                mycon.Open();
                mycom = new MySqlCommand(query, mycon);
                myread = mycom.ExecuteReader();
                while (myread.Read())
                {
                    srs = myread.GetValue(0).ToString();

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
            return srs;
        }

        public void loadnames()
        {
            string query = "select fullname from mmioscredentials";
            try
            {
                mycon = new MySqlConnection(connection137);
                mycon.Open();
                mycom = new MySqlCommand(query, mycon);
                myread = mycom.ExecuteReader();
                while (myread.Read())
                {
                    cbIPQC.Items.Add(myread.GetValue(0).ToString());

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
            string series = loadseries(cbModel.Text);
            if (series == "")
            {
                series = "0";
            }
            int seriessum = int.Parse(series) + 1;
            gimodel = nc.getdata(query);
            loadhpnumber();
            lblboxseries.Text = "GI-" + cbModel.Text + "-" + DateTime.Now.ToString("yyyyMMdd") + "-" + seriessum.ToString("d5");

        }


        private void cbHPnumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            string queryhp = "select hpcode from mmiosmodel where model ='" + gimodel + "' and hpnumber = '" + cbHPnumber.Text + "' ";
            txthpcode.Text = nc.getdata(queryhp);
        }

        private void btnsave_Click(object sender, EventArgs e)
        {
            if (cbModel.Text == "" || cbHPnumber.Text == "" || cbIPQC.Text == "")
            {
                MessageBox.Show("Please complete the details needed!", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (txthpcode.Text == "")
            {
                MessageBox.Show("Please complete the details needed!", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string query = "insert into mmioshistory(model,hpnumber,hpcode,qty,receivedby,ipqc,hstype,hsdate)values('" + cbModel.Text + "','" + cbHPnumber.Text + "','" + txthpcode.Text + "','" + txtqty.Text + "','" + txtreceived.Text + "','" + cbIPQC.Text + "','" + lblHSType.Text + "','" + lblDate.Text + "')";
                nc.iData(query);
                string queryprint = "insert into mmiosoprint(model,hpnumber,hpcode,qty,receivedby,ipqc,hstype,hsdate)values('" + cbModel.Text + "','" + cbHPnumber.Text + "','" + txthpcode.Text + "','" + txtqty.Text + "','" + txtreceived.Text + "','" + cbIPQC.Text + "','" + lblHSType.Text + "','" + lblDate.Text + "')";
                nc.iData(queryprint);
                MessageBox.Show("Printed Successfully", "Notification!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblboxseries.Text = string.Empty;
                cbModel.Text = "";
                cbHPnumber.Text = "";
                txthpcode.Text = "";
                txtqty.Text = "";
                cbIPQC.Text = "";

            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string WeighingScalePort = comboBox2.Text;
                string PortToOpen;
                if (WeighingScalePort != "")
                {
                    if (WeighingScalePort != comboBox2.Text)
                    {
                        PortToOpen = WeighingScalePort;
                    }
                    else
                    {
                        PortToOpen = comboBox2.Text;
                    }

                    if (_serialPort != null)
                    {
                        _serialPort.Close();

                    }
                    _serialPort = new SerialPort(PortToOpen, 9600, Parity.None, 8);
                    _serialPort.Open();
                }
            }
            catch (Exception ex)
            {
            }
            timer1.Enabled = true;
        }
        public void WriteTextSafe(string text)
        {
            if (text == "")
            {


            }
            else
            {
                try
                {
                    if (lblweigh.InvokeRequired)
                    {

                        Action safeWrite = delegate { WriteTextSafe($"{text}"); };
                        lblweigh.Invoke(safeWrite);
                        if (lblweigh.Text != "")
                        {
                            lblweigh.Text = text;
                            string[] DataPortArray = text.Split();

                            string weightData = DataPortArray[4].Substring(0, 4);

                            this.lblweigh.Text = weightData == "0.00" ? "0" : Convert.ToDouble(weightData).ToString("0.00");
                        }
                    }
                    else
                    {
                        lblweigh.Text = text;
                        string[] DataPortArray = text.Split();

                        string weightData = DataPortArray[4].Substring(0, 4);

                        this.lblweigh.Text = weightData == "0.00" ? "0" : Convert.ToDouble(weightData).ToString("0.00");
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }

        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_serialPort != null)
                {
                    string DataPortRead = _serialPort.ReadExisting();

                    WriteTextSafe(DataPortRead);
                }

            }
            catch (Exception ex)
            {

            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            lblDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:ss:mm");
            txtreceived.Text = receiver;
        }
    }
}
