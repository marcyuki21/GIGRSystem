using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GIGRSystem
{

    public partial class GRForm : Form
    {
        private SerialPort _serialPort = null;

        public MySqlConnection mycon;
        public MySqlCommand mycom;
        public MySqlDataReader myread;
        public MySqlDataAdapter myadapt;
        public string connectionFurukawa = "Server=192.168.4.136;Port=3309;Database=furukawa;User ID=root;Password=123;";
        public string connectionFormal = "Server=192.168.4.136;Port=3309;Database=formal;User ID=root;Password=123;";
        public string connection137 = "Server=192.168.4.137;Port=3306;Database=mmios;User ID=root;Password=pass1234;";
        public static string user;
        Class1 nc = new Class1();
        public GRForm()
        {
            InitializeComponent();
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
        public void WriteTextSafe(string text)
        {
            if (text == "")
            {


            }
            else
            {
                try
                {
                    if (lblWeigh.InvokeRequired)
                    {

                        Action safeWrite = delegate { WriteTextSafe($"{text}"); };
                        lblWeigh.Invoke(safeWrite);
                        if (lblWeigh.Text != "")
                        {
                            lblWeigh.Text = text;
                            string[] DataPortArray = text.Split();

                            string weightData = DataPortArray[4].Substring(0, 4);

                            this.lblWeigh.Text = weightData == "0.00" ? "0" : Convert.ToDouble(weightData).ToString("0.00");
                        }
                    }
                    else
                    {
                        lblWeigh.Text = text;
                        string[] DataPortArray = text.Split();

                        string weightData = DataPortArray[4].Substring(0, 4);

                        this.lblWeigh.Text = weightData == "0.00" ? "0" : Convert.ToDouble(weightData).ToString("0.00");
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }

        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string WeighingScalePort = comboBox1.Text;
                string PortToOpen;
                if (WeighingScalePort != "")
                {
                    if (WeighingScalePort != comboBox1.Text)
                    {
                        PortToOpen = WeighingScalePort;
                    }
                    else
                    {
                        PortToOpen = comboBox1.Text;
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

        private void test_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }
            string query = "select * from mmiosreceived where convert(pdate,datetime) between '" + DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00' and '" + DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59' order by id desc";
            loadgrid(query);

        }

        public void loadgrid(string query)
        {
            DataTable dt = new DataTable();
            dt = nc.filldata(query).Tables[0];
            dataGridView1.DataSource = dt;

        }

        private void txtPtc_KeyPress(object sender, KeyPressEventArgs e)

        {
            if (e.KeyChar == (char)13)
            {
                string query = "select icboxseries from mmioshistory where icboxseries = '" + txtPtc.Text + "'";
                if (nc.dataExist(query) == true)
                {
                    string quantity = "select icqty from mmioshistory where icboxseries = '" + txtPtc.Text + "'";
                    lblPtcQty.Text = nc.getdata(quantity);
                    string weigh = "select issuedweigh from mmioshistory where icboxseries = '" + txtPtc.Text + "'";
                    lblptcweigh.Text = nc.getdata(weigh);
                }
                else
                {
                    MessageBox.Show("Boxseries not exist!", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPtc.Text = "";
                    txtPtc.Focus();
                }
            }
        }

        private void txtGboxid_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                string g = txtGboxid.Text.Substring(0, 1);

                if (g == "G")
                {


                    string query = "select boxnumber from bendinghistory where boxnumber = '" + txtGboxid.Text + "'";
                    if (nc.hsdataExist(query) == true)
                    {
                        string quantity = "select quantity from bendinghistory where boxnumber = '" + txtGboxid.Text + "'";
                        lblgQuantity.Text = nc.hsgetdata(quantity);
                        string weigh = "select weigh from bendinghistory where boxnumber = '" + txtGboxid.Text + "'";
                        lblGweigh.Text = nc.hsgetdata(weigh);
                        string lotno = "select lotno from bendinghistory where boxnumber = '" + txtGboxid.Text + "'";
                        lblGlotno.Text = nc.hsgetdata(lotno);
                        string hpsize = "select size from bendinghistory where boxnumber = '" + txtGboxid.Text + "'";
                        lblGHpsize.Text = nc.hsgetdata(hpsize);
                        string hpnumber = "select hpnumber from bendinghistory where boxnumber = '" + txtGboxid.Text + "'";
                        lblGhpnumber.Text = nc.hsgetdata(hpnumber);
                        string hpcode = "select hpcode from bendinghistory where boxnumber = '" + txtGboxid.Text + "'";
                        lblGhpcode.Text = nc.hsgetdata(hpcode);
                        string gtype = "select hstype from bendinghistory where boxnumber = '" + txtGboxid.Text + "'";
                        lblGhstype.Text = nc.hsgetdata(gtype);
                        string gmodel = "select modelname from bendinghistory where boxnumber = '" + txtGboxid.Text + "'";
                        lblGmodel.Text = nc.hsgetdata(gmodel);
                    }
                    else
                    {
                        MessageBox.Show("Boxseries not exist!", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtGboxid.Text = "";
                        txtGboxid.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Boxseries is not for scan here!", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            lblusername.Text = user;
        }

        private void txtNGbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                string NG = txtNGbox.Text.Substring(0, 2);
                if (NG == "NG")
                {


                    string query = "select boxnumber from bendinghistory where boxnumber = '" + txtNGbox.Text + "'";
                    if (nc.hsdataExist(query) == true)
                    {
                        string quantity = "select quantity from bendinghistory where boxnumber = '" + txtNGbox.Text + "'";
                        lblNGQuantity.Text = nc.hsgetdata(quantity);
                        string weigh = "select weigh from bendinghistory where boxnumber = '" + txtNGbox.Text + "'";
                        lblNGweigh.Text = nc.hsgetdata(weigh);
                        string lotno = "select lotno from bendinghistory where boxnumber = '" + txtNGbox.Text + "'";
                        lblNGLotno.Text = nc.hsgetdata(lotno);
                        string hpsize = "select size from bendinghistory where boxnumber = '" + txtNGbox.Text + "'";
                        lblNGHPsize.Text = nc.hsgetdata(hpsize);
                        string hpnumber = "select hpnumber from bendinghistory where boxnumber = '" + txtNGbox.Text + "'";
                        lblNGHpnumber.Text = nc.hsgetdata(hpnumber);
                        string hpcode = "select hpcode from bendinghistory where boxnumber = '" + txtNGbox.Text + "'";
                        lblNGHPcode.Text = nc.hsgetdata(hpcode);
                        string hstype = "select hstype from bendinghistory where boxnumber = '" + txtNGbox.Text + "'";
                        lblNGhstype.Text = nc.hsgetdata(hstype);
                        string gmodel = "select modelname from bendinghistory where boxnumber = '" + txtGboxid.Text + "'";
                        lblNGmodel.Text = nc.hsgetdata(gmodel);
                    }
                    else
                    {
                        MessageBox.Show("Boxseries not exist!", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtGboxid.Text = "";
                        txtGboxid.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Boxseries is not for good to scan!", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtGboxid.Text == "")
            {
                MessageBox.Show("Empty good box id!", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtGboxid.Focus();
            }
            else if (lblGweigh.Text == lblWeigh.Text)
            {
                string idata = "insert into mmiosreceived(pdate,boxseries,weigh,qty,lotno,hpnumber,hpsize,hpcode,hstype,model)values('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + txtGboxid.Text + "','" + lblWeigh.Text + "','" + lblgQuantity.Text + "','" + lblGlotno.Text + "','" + lblGhpnumber.Text + "','" + lblGHpsize.Text + "','" + lblGhpcode.Text + "','" + lblGhstype.Text + "','" + lblGmodel.Text + "')";
                nc.iData(idata);
                txtGboxid.Text = "";
                lblGhpcode.Text = "";
                lblGhpnumber.Text = "";
                lblGHpsize.Text = "";
                lblGhstype.Text = "";
                lblGlotno.Text = "";
                lblgQuantity.Text = "";
                lblGweigh.Text = "";
                MessageBox.Show("Box saved!", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(txtGboxid.Text + " weigh is not match in previous weigh", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtGboxid.Focus();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (txtNGbox.Text == "")
            {
                MessageBox.Show("Empty good box id!", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtNGbox.Focus();
            }
            else if (lblNGweigh.Text == lblWeigh.Text)
            {
                string idata = "insert into mmiosreceived(pdate,boxseries,weigh,qty,lotno,hpnumber,hpsize,hpcode,hstype,model)values('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + txtNGbox.Text + "','" + lblWeigh.Text + "','" + lblNGQuantity.Text + "','" + lblNGLotno.Text + "','" + lblNGHpnumber.Text + "','" + lblNGHPsize.Text + "','" + lblNGHPcode.Text + "','" + lblNGhstype.Text + "','" + lblNGmodel.Text + "')";
                nc.iData(idata);
                txtNGbox.Text = "";
                lblNGHPcode.Text = "";
                lblNGHpnumber.Text = "";
                lblNGHPsize.Text = "";
                lblNGhstype.Text = "";
                lblNGLotno.Text = "";
                lblNGQuantity.Text = "";
                lblNGweigh.Text = "";
                MessageBox.Show("Box saved!", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(txtNGbox.Text + " weigh is not match in previous weigh", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtGboxid.Focus();
            }
        }

        private void timer2_Tick_1(object sender, EventArgs e)
        {

        }
    }
}

