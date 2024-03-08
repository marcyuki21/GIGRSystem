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
using MySql.Data.MySqlClient;

namespace GIGRSystem
{
    public partial class Issuance : Form
    {
        public static string issuer;
        Class1 cn = new Class1();

        public MySqlConnection mycon;
        public MySqlCommand mycom;
        public MySqlDataReader myread;
        public MySqlDataAdapter myadapt;
        public string connectionFurukawa = "Server=192.168.4.136;Port=3309;Database=furukawa;User ID=root;Password=123;";
        public string connectionFormal = "Server=192.168.4.136;Port=3309;Database=formal;User ID=root;Password=123;";
        public string connection137 = "Server=192.168.4.137;Port=3306;Database=mmios;User ID=root;Password=pass1234;";
        public static string receiver;
        public static int seriessum;
        public static bool timelimit = false;

        public Issuance()
        {
            InitializeComponent();
        }

        private void Issuance_Load(object sender, EventArgs e)
        {
            string loadtable = "select * from mmiosissuancebending order by id desc";
            loaddata(loadtable);
            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            txtIssuedby.Text = issuer;
            txtpissuedby.Text = issuer;
        }

        private void textBox8_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                string model = "select model from mmioshistory where boxseries = '" + txtGIBox.Text + "' order by id desc limit 1";
                string hpnumber = "select hpnumber from mmioshistory where boxseries = '" + txtGIBox.Text + "' order by id desc limit 1";
                string hpcode = "select hpcode from mmioshistory where boxseries = '" + txtGIBox.Text + "' order by id desc limit 1";
                string giqty = "select qty from mmioshistory where boxseries = '" + txtGIBox.Text + "' order by id desc limit 1";
                string type = "select hstype from mmioshistory where boxseries = '" + txtGIBox.Text + "' order by id desc limit 1";
                string remaining = "select remaining from mmioshistory where boxseries = '" + txtGIBox.Text + "' order by id desc limit 1";
                string boxexist = "select boxseries from mmioshistory where boxseries = '" + txtGIBox.Text + "' order by id desc limit 1";
                string id = "select id from mmioshistory where boxseries = '" + txtGIBox.Text + "' order by id desc limit 1";
                string lotno = "select slot_no from mmioshistory where boxseries = '" + txtGIBox.Text + "' order by id desc limit 1";
                string receivedby = "select receivedby from mmioshistory where boxseries = '" + txtGIBox.Text + "' order by id desc limit 1";
                string ipqc = "select ipqc from mmioshistory where boxseries = '" + txtGIBox.Text + "' order by id desc limit 1";
                string rdate = "select hsdate from mmioshistory where boxseries = '" + txtGIBox.Text + "' order by id desc limit 1";
                string weigh = "select weigh from mmioshistory where boxseries = '" + txtGIBox.Text + "' order by id desc limit 1";
                if (cn.dataExist(boxexist) == true)
                {
                    lblGImodel.Text = cn.getdata(model);
                    lblGInumber.Text = cn.getdata(hpnumber);
                    lblGIHPcode.Text = cn.getdata(hpcode);
                    lblGiquantity.Text = cn.getdata(giqty);
                    lblremaining.Text = cn.getdata(remaining);
                    lblIDbox.Text = cn.getdata(id);
                    lbllotno.Text = cn.getdata(lotno);
                    lblreceived.Text = cn.getdata(receivedby);
                    lblipqc.Text = cn.getdata(ipqc);
                    lblhsDate.Text = cn.getdata(rdate);
                    lblGIweigh.Text = cn.getdata(weigh);

                    if (lblremaining.Text == "")
                    {
                        lblremaining.Text = lblGiquantity.Text;
                    }
                    lblGIHStype.Text = cn.getdata(type);

                }
                else
                {
                    MessageBox.Show(txtGIBox.Text + " NOT EXIST!", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtGIBox.Text = "";
                    txtGIBox.Focus();
                }
            }
        }

        private void lblGImodel_TextChanged(object sender, EventArgs e)
        {

            string series = loadseries(lblGImodel.Text);
            if (series == "")
            {
                series = "0";
            }
            seriessum = int.Parse(series) + 1;
            lblBoxseries.Text = "IC-" + lblGImodel.Text + "-" + DateTime.Now.ToString("yyyyMMdd") + "-" + seriessum.ToString("d5");
        }
        public string loadseries(string model)
        {
            string srs = string.Empty;
            string query = "select series from mmiosissueseries where model = '" + model + "' and pdate = '" + DateTime.Now.ToString("yyyy-MM-dd") + "' order by id desc limit 1";
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

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                  (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            string remaining = "select remaining from mmioshistory where boxseries = '" + txtGIBox.Text + "' order by id desc limit 1";
            lblremaining.Text = cn.getdata(remaining);
            if (lblremaining.Text == "")
            {
                lblremaining.Text = lblGiquantity.Text;
            }
            if (textBox2.Text == "")
            {
                textBox2.Text = "0";
                lblremaining.Text = cn.getdata(remaining);
                if (lblremaining.Text == "")
                {
                    lblremaining.Text = lblGiquantity.Text;
                }
            }
            if (int.Parse(textBox2.Text) > int.Parse(lblremaining.Text))
            {

                MessageBox.Show("OUT OF RANGE", "ALERT!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox2.Text = "0";
            }
            else
            {
                lblremaining.Text = (int.Parse(lblremaining.Text) - int.Parse(textBox2.Text)).ToString();
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {//|| lblWeigh1.Text == "00.00"

            if (txtGIBox.Text == "" || textBox2.Text == "" || txtLotno.Text == "" || txtIssuedto.Text == "")
            {
                MessageBox.Show("Insufficient details!" + Environment.NewLine + "Please fill the needed details!", "ALERT!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtGIBox.Focus();
            }
            //if (lblremaining.Text == "0")
            //{
            //    MessageBox.Show("Insufficient to issue please check!" + Environment.NewLine + "Please fill the needed details!", "ALERT!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    txtGIBox.Focus();
            //}
            else
            {
                string querysaved = "insert into mmiosissuancebending(dateissued,model,icboxseries,icquantity,icweigh,lot_no,hpcode,hpnumber,hstype,issuedby,issuedto)values('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + lblGImodel.Text + "','" + lblBoxseries.Text + "','" + textBox2.Text + "','" + lblWeigh1.Text + "','" + txtLotno.Text + "','" + lblGIHPcode.Text + "','" + lblGInumber.Text + "','" + lblGIHStype.Text + "','" + txtIssuedby.Text + "','" + txtIssuedto.Text + "')";
                cn.iData(querysaved);
                string querysavedprint = "insert into mmiosissueoprint(dateissued,model,icboxseries,icquantity,icweigh,lot_no,hpcode,hpnumber,hstype,issuedby,issuedto)values('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + lblGImodel.Text + "','" + lblBoxseries.Text + "','" + textBox2.Text + "','" + lblWeigh1.Text + "','" + txtLotno.Text + "','" + lblGIHPcode.Text + "','" + lblGInumber.Text + "','" + lblGIHStype.Text + "','" + txtIssuedby.Text + "','" + txtIssuedto.Text + "')";
                cn.iData(querysavedprint);
                //string updatetable = "update mmioshistory set remaining = '" + lblremaining.Text + "',icboxseries = '" + lblBoxseries.Text + "',icqty='" + textBox2.Text + "',issuedweigh = '" + lblWeigh1.Text + "',ftllot_no = '" + txtLotno.Text + "',dateissued = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',issuedby = '" + txtIssuedby.Text + "',issuedto = '" + txtIssuedto.Text + "' where id = '" + lblIDbox.Text + "'";
                string insertdata = "insert into mmioshistory(boxseries,qty,model,hpnumber,slot_no,hpcode,receivedby,ipqc,hstype,hsdate,weigh,remaining,icboxseries,icqty,issuedweigh,ftllot_no,dateissued,issuedby,issuedto)values('" + txtGIBox.Text + "','" + lblGiquantity.Text + "','" + lblGImodel.Text + "','" + lblGInumber.Text + "','" + lbllotno.Text + "','" + lblGIHPcode.Text + "','" + lblreceived.Text + "','" + lblipqc.Text + "','" + lblGIHStype.Text + "','" + lblhsDate.Text + "','" + lblGIweigh.Text + "','" + lblremaining.Text + "','" + lblBoxseries.Text + "','" + textBox2.Text + "','" + lblWeigh1.Text + "','" + txtLotno.Text + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + txtIssuedby.Text + "','" + txtIssuedto.Text + "')";
                cn.iData(insertdata);
                string queryprint = "insert into mmiosoprint(boxseries,model,hpnumber,hpcode,qty,receivedby,ipqc,hstype,hsdate,weigh)values('" + txtGIBox.Text + "','" + lblGImodel.Text + "','" + lblGInumber.Text + "','" + lblGIHPcode.Text + "','" + lblremaining.Text + "','" + lblreceived.Text + "','" + lblipqc.Text + "','" + lblGIHStype.Text + "','" + lblhsDate.Text + "','" + lblGIweigh.Text + "')";
                cn.iData(queryprint);
                MessageBox.Show("Printed and saved!" + Environment.NewLine + "Please check the details!", "ALERT!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                string loadtable = "select * from mmiosissuancebending order by id desc";
                string series = "insert into mmiosissueseries(model,series,pdate)values('" + lblGImodel.Text + "','" + seriessum + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "')";
                cn.iData(series);
                loaddata(loadtable);
                txtGIBox.Text = "";
                txtLotno.Text = "";
                textBox2.Text = "";
                lblGIHPcode.Text = "";
                lblGIHStype.Text = "";
                lblGImodel.Text = "";
                lblGInumber.Text = "";
                lblGiquantity.Text = "";
                lblGIweigh.Text = "";
                lblhsDate.Text = "";
                lblIDbox.Text = "";
                lblipqc.Text = "";
                lbllotno.Text = "";
                lblreceived.Text = "";
                lblremaining.Text = "";

            }
        }

        public void loaddata(string query)
        {
            DataTable dt = new DataTable();
            dt = cn.filldata(query).Tables[0];
            dataGridView1.DataSource = dt;
            dataGridView2.DataSource = dt;
        }

        private void txtpboxserial_KeyPress(object sender, KeyPressEventArgs e)
        {


            if (e.KeyChar == (char)13)
            {
                if (txtpboxserial.Text != "")
                {


                    string g = txtpboxserial.Text.Substring(0, 1);
                    if (g == "G")
                    {
                        string model = "select model from mmiosreceived where boxseries = '" + txtpboxserial.Text + "' order by id desc limit 1";
                        string hpnumber = "select hpnumber from mmiosreceived where boxseries = '" + txtpboxserial.Text + "' order by id desc limit 1";
                        string hpcode = "select hpcode from mmiosreceived where boxseries = '" + txtpboxserial.Text + "' order by id desc limit 1";
                        string giqty = "select qty from mmiosreceived where boxseries = '" + txtpboxserial.Text + "' order by id desc limit 1";
                        string type = "select hstype from mmiosreceived where boxseries = '" + txtpboxserial.Text + "' order by id desc limit 1";
                        string remaining = "select remaining from mmiosreceived where boxseries = '" + txtpboxserial.Text + "' order by id desc limit 1";
                        string boxexist = "select boxseries from mmiosreceived where boxseries = '" + txtpboxserial.Text + "' order by id desc limit 1";
                        string id = "select id from mmiosreceived where boxseries = '" + txtpboxserial.Text + "' order by id desc limit 1";
                        string lotno = "select lotno from mmiosreceived where boxseries = '" + txtpboxserial.Text + "' order by id desc limit 1";
                        string hpsize = "select hpsize from mmiosreceived where boxseries = '" + txtpboxserial.Text + "' order by id desc limit 1";
                        string pdate = "select pdate from mmiosreceived where boxseries = '" + txtpboxserial.Text + "' order by id desc limit 1";
                        string weigh = "select weigh from mmiosreceived where boxseries = '" + txtpboxserial.Text + "' order by id desc limit 1";
                        if (cn.dataExist(boxexist) == true)
                        {
                            lblpmodel.Text = cn.getdata(model);
                            lblphpnumber.Text = cn.getdata(hpnumber);
                            lblphpcode.Text = cn.getdata(hpcode);
                            lblpquantity.Text = cn.getdata(giqty);
                            lblPremaining.Text = cn.getdata(remaining);
                            lblpBoxid.Text = cn.getdata(id);
                            lblpLotno.Text = cn.getdata(lotno);
                            lblphpsize.Text = cn.getdata(hpsize);
                            lblpDate.Text = cn.getdata(pdate);
                            lblpWeigh.Text = cn.getdata(weigh);

                            if (lblPremaining.Text == "")
                            {
                                lblPremaining.Text = lblpquantity.Text;
                            }
                            lblptype.Text = cn.getdata(type);

                        }
                        else
                        {
                            MessageBox.Show(txtpboxserial.Text + " NOT EXIST!", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtpboxserial.Text = "";
                            txtpboxserial.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show(txtpboxserial.Text + " IS NOT GOOD TO ISSUE!", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtpboxserial.Text = "";
                        txtpboxserial.Focus();
                    }
                }
            }

        }
        private SerialPort _serialPort = null;
        private void button3_Click(object sender, EventArgs e)
        {
            if (txtpboxserial.Text == "" || txtpquantity.Text == "" || txtplotno.Text == "" || txtpissuedto.Text == "")
            {
                MessageBox.Show("Insufficient details!" + Environment.NewLine + "Please fill the needed details!", "ALERT!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtpboxserial.Focus();
            }
            //if (lblPremaining.Text == "0")
            //{
            //    MessageBox.Show("Insufficient to issue please check!" + Environment.NewLine + "Please fill the needed details!", "ALERT!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    txtpboxserial.Focus();
            //}
            else
            {
                string querysaved = "insert into mmiosissuancebending(dateissued,model,icboxseries,icquantity,icweigh,lot_no,hpcode,hpnumber,hstype,issuedby,issuedto)values('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + lblpmodel.Text + "','" + lblpBoxseries.Text + "','" + txtpquantity.Text + "','" + lblWeigh1.Text + "','" + txtplotno.Text + "','" + lblphpcode.Text + "','" + lblphpnumber.Text + "','" + lblptype.Text + "','" + txtpissuedby.Text + "','" + txtpissuedto.Text + "')";
                cn.iData(querysaved);
                string querysavedprint = "insert into mmiosissueoprintprod(dateissued,model,icboxseries,icquantity,icweigh,lot_no,hpcode,hpnumber,hstype,issuedby,issuedto)values('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + lblpmodel.Text + "','" + lblpBoxseries.Text + "','" + txtpquantity.Text + "','" + lblWeigh1.Text + "','" + txtplotno.Text + "','" + lblphpcode.Text + "','" + lblphpnumber.Text + "','" + lblptype.Text + "','" + txtpissuedby.Text + "','" + txtpissuedto.Text + "')";
                cn.iData(querysavedprint);
                string inspector = "select inspector from bendinghistory where boxnumber = '" + txtpboxserial.Text + "'";
                string pinspector = cn.hsgetdata(inspector);
                string bending = "select bending from bendinghistory where boxnumber = '" + txtpboxserial.Text + "'";
                string pbending = cn.hsgetdata(bending);
                string flat = "select flattening from bendinghistory where boxnumber = '" + txtpboxserial.Text + "'";
                string pflattening = cn.hsgetdata(flat);
                string delta = "select deltatest from bendinghistory where boxnumber = '" + txtpboxserial.Text + "'";
                string pdelta = cn.hsgetdata(delta);
                string gng = "select gng from bendinghistory where boxnumber = '" + txtpboxserial.Text + "'";
                string pgng = cn.hsgetdata(gng);
                string remarks = "select remarks from bendinghistory where boxnumber = '" + txtpboxserial.Text + "'";
                string premarks = cn.hsgetdata(remarks);
                string status = "select status from bendinghistory where boxnumber = '" + txtpboxserial.Text + "'";
                string pstatus = cn.hsgetdata(status);
                string classification = "select classification from bendinghistory where boxnumber = '" + txtpboxserial.Text + "'";
                string pclassification = cn.hsgetdata(classification);
                //string updatetable = "update mmioshistory set remaining = '" + lblremaining.Text + "',icboxseries = '" + lblBoxseries.Text + "',icqty='" + textBox2.Text + "',issuedweigh = '" + lblWeigh1.Text + "',ftllot_no = '" + txtLotno.Text + "',dateissued = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',issuedby = '" + txtIssuedby.Text + "',issuedto = '" + txtIssuedto.Text + "' where id = '" + lblIDbox.Text + "'";
                string insertdata = "insert into mmiosreceived(pdate,boxseries,model,hpnumber,hpcode,qty,hstype,lotno,weigh,hpsize,remaining,icboxseries,icqty,issuedweigh,ftllot_no,dateissued,issuedby,issuedto)values('" + lblpDate.Text + "','" + txtpboxserial.Text + "','" + lblpmodel.Text + "','" + lblphpnumber.Text + "','" + lblphpcode.Text + "','" + lblpquantity.Text + "','" + lblptype.Text + "','" + lblpLotno.Text + "','" + lblpWeigh.Text + "','" + lblphpsize.Text + "','" + lblPremaining.Text + "','" + lblpBoxseries.Text + "','" + txtpquantity.Text + "','" + lblweigh2.Text + "','" + txtplotno.Text + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + txtIssuedby.Text + "','" + txtIssuedto.Text + "')";
                cn.iData(insertdata);
                string query = "insert into bendingoprintware(modelname,boxnumber,partnumber,inspector,hpnumber,hpcode,size,bending,flattening,deltatest,gng,lotno,quantity,weigh,remarks,status,pdate,hstype,classification)values('" + lblpmodel.Text + "','" + txtpboxserial.Text + "','" + lblphpcode.Text + "','" + pinspector + "','" + lblptype.Text + "','" + lblGIHPcode.Text + "','" + lblphpsize.Text + "','" + pbending + "','" + pflattening + "','" + pdelta + "','" + pgng + "','" + lblpLotno.Text + "','" + lblPremaining.Text + "','" + lblpWeigh.Text + "','" + premarks + "','" + pstatus + "','" + DateTime.Now.ToString("MM-dd-yyyy") + "','" + lblptype.Text + "','" + pclassification + "')";
                cn.hsiData(query);
                MessageBox.Show("Printed and saved!" + Environment.NewLine + "Please check the details!", "ALERT!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                string loadtable = "select * from mmiosissuancebending order by id desc";
                string series = "insert into mmiosissueseries(model,series,pdate)values('" + lblpmodel.Text + "','" + seriessum + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "')";
                cn.iData(series);
                loaddata(loadtable);
                txtpboxserial.Text = "";
                txtplotno.Text = "";
                txtpquantity.Text = "";
                lblphpcode.Text = "";
                lblptype.Text = "";
                lblpmodel.Text = "";
                lblphpnumber.Text = "";
                lblpquantity.Text = "";
                lblpWeigh.Text = "";
                lblpDate.Text = "";
                lblpBoxid.Text = "";
                lblpLotno.Text = "";
                lblPremaining.Text = "";
                lblphpsize.Text = "";

            }
        }

        private void lblpmodel_TextChanged(object sender, EventArgs e)
        {
            string series = loadseries(lblpmodel.Text);
            if (series == "")
            {
                series = "0";
            }
            seriessum = int.Parse(series) + 1;
            lblpBoxseries.Text = "IC-" + lblpmodel.Text + "-" + DateTime.Now.ToString("yyyyMMdd") + "-" + seriessum.ToString("d5");
        }

        private void txtpquantity_TextChanged(object sender, EventArgs e)
        {
            string remaining = "select remaining from mmiosreceived where boxseries = '" + txtpboxserial.Text + "' order by id desc limit 1";
            lblPremaining.Text = cn.getdata(remaining);
            if (lblPremaining.Text == "")
            {
                lblPremaining.Text = lblpquantity.Text;
            }
            if (txtpquantity.Text == "")
            {
                txtpquantity.Text = "0";
                lblPremaining.Text = cn.getdata(remaining);
                if (lblPremaining.Text == "")
                {
                    lblPremaining.Text = lblpquantity.Text;
                }
            }
            if (int.Parse(txtpquantity.Text) > int.Parse(lblPremaining.Text))
            {

                MessageBox.Show("OUT OF RANGE", "ALERT!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtpquantity.Text = "0";
            }
            else
            {
                lblPremaining.Text = (int.Parse(lblPremaining.Text) - int.Parse(txtpquantity.Text)).ToString();
            }

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

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
            timer2.Enabled = true;
        }

        private void timer2_Tick(object sender, EventArgs e)
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
                    if (lblWeigh1.InvokeRequired)
                    {

                        Action safeWrite = delegate { WriteTextSafe($"{text}"); };
                        lblWeigh1.Invoke(safeWrite);
                        if (lblWeigh1.Text != "")
                        {
                            lblWeigh1.Text = text;
                            string[] DataPortArray = text.Split();

                            string weightData = DataPortArray[4].Substring(0, 4);

                            this.lblWeigh1.Text = weightData == "0.00" ? "0" : Convert.ToDouble(weightData).ToString("0.00");
                        }
                    }
                    else
                    {
                        lblWeigh1.Text = text;
                        string[] DataPortArray = text.Split();

                        string weightData = DataPortArray[4].Substring(0, 4);

                        this.lblWeigh1.Text = weightData == "0.00" ? "0" : Convert.ToDouble(weightData).ToString("0.00");
                    }
                }
                catch (Exception)
                {

                    throw;
                }



                try
                {
                    if (lblweigh2.InvokeRequired)
                    {

                        Action safeWrite = delegate { WriteTextSafe($"{text}"); };
                        lblweigh2.Invoke(safeWrite);
                        if (lblweigh2.Text != "")
                        {
                            lblweigh2.Text = text;
                            string[] DataPortArray = text.Split();

                            string weightData = DataPortArray[4].Substring(0, 4);

                            this.lblweigh2.Text = weightData == "0.00" ? "0" : Convert.ToDouble(weightData).ToString("0.00");
                        }
                    }
                    else
                    {
                        lblweigh2.Text = text;
                        string[] DataPortArray = text.Split();

                        string weightData = DataPortArray[4].Substring(0, 4);

                        this.lblweigh2.Text = weightData == "0.00" ? "0" : Convert.ToDouble(weightData).ToString("0.00");
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }

        }
    }
}
