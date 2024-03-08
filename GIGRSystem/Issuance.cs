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
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            txtIssuedby.Text = issuer;
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
            if (lblremaining.Text == "0")
            {
                MessageBox.Show("Insufficient to issue please check!" + Environment.NewLine + "Please fill the needed details!", "ALERT!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtGIBox.Focus();
            }
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
        }
    }
}
