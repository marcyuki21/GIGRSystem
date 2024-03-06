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
        public static int seriessum;
        public static bool timelimit = false;

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
            loaddgv();
            loaddgv2();

            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkOrange;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.WhiteSmoke;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.ReadOnly = true;
            dataGridView1.EnableHeadersVisualStyles = false;

            dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkOrange;
            dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.WhiteSmoke;
            dataGridView2.BackgroundColor = Color.White;
            dataGridView2.ReadOnly = true;
            dataGridView2.EnableHeadersVisualStyles = false;




            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
            {
                comboBox2.Items.Add(port);
            }


            dateTimePicker1.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            dateTimePicker2.CustomFormat = "yyyy-MM-dd  HH:mm:ss";

            comboBox1.Items.Add("boxseries");
            comboBox1.Items.Add("model");
            comboBox1.Items.Add("hpnumber");
            comboBox1.Items.Add("hpcode");
            comboBox1.Items.Add("receivedby");
            comboBox1.Items.Add("ipqc");





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
            string query = "select series from mmiosseries where model = '" + model + "' and pdate = '" + DateTime.Now.ToString("yyyy-MM-dd") + "' order by id desc limit 1";
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
            seriessum = int.Parse(series) + 1;
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

                string query = "insert into mmioshistory(boxseries,model,hpnumber,hpcode,qty,receivedby,ipqc,hstype,hsdate,weigh)values('" + lblboxseries.Text + "','" + cbModel.Text + "','" + cbHPnumber.Text + "','" + txthpcode.Text + "','" + txtqty.Text + "','" + txtreceived.Text + "','" + cbIPQC.Text + "','" + lblHSType.Text + "','" + lblDate.Text + "','" + lblweigh.Text + "')";
                nc.iData(query);
                string queryprint = "insert into mmiosoprint(boxseries,model,hpnumber,hpcode,qty,receivedby,ipqc,hstype,hsdate,weigh)values('" + lblboxseries.Text + "','" + cbModel.Text + "','" + cbHPnumber.Text + "','" + txthpcode.Text + "','" + txtqty.Text + "','" + txtreceived.Text + "','" + cbIPQC.Text + "','" + lblHSType.Text + "','" + lblDate.Text + "','" + lblweigh.Text + "')";
                nc.iData(queryprint);
                string queryseries = "insert into mmiosseries(model,series,pdate)values('" + cbModel.Text + "','" + seriessum + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "')";
                nc.iData(queryseries);

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

        private void button3_Click(object sender, EventArgs e)
        {
            string query = "insert into mmiosmodel(model,hpnumber,hpcode)values('" + txtMModel.Text + "','" + txtMHpnumber.Text + "','" + txtMhpcode.Text + "')";
            string checkquery = "select * from mmiosmodel where model='" + txtMModel.Text + "' and hpnumber='" + txtMHpnumber.Text + "' and hpcode='" + txtMhpcode.Text + "'  ";

            if (txtMModel.Text == "" || txtMHpnumber.Text == "" || txtMhpcode.Text == "")
            {
                MessageBox.Show("PLEASE FILL UP ALL DETAILS ", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (nc.dataExist(checkquery) == true)
            {
                MessageBox.Show("MODEL AND HPNUMBER ALREADY EXIST", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else
            {
                nc.iData(query);
                MessageBox.Show("Data Successfully Registered", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMModel.Text = "";
                txtMHpnumber.Text = "";
                txtMhpcode.Text = "";
                loaddgv();

            }
        }

        public void loaddgv()
        {
            string query = "Select * from mmiosmodel order by model and hpnumber asc ";
            DataTable dt;
            dt = nc.filldata(query).Tables[0];
            dataGridView1.DataSource = dt;
        }

        public void loaddgv2()
        {
            string query = "Select * from mmioshistory order by id desc limit 200";
            DataTable dt;
            dt = nc.filldata(query).Tables[0];
            dataGridView2.DataSource = dt;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string query = "update mmiosmodel set hpcode='" + txtMhpcode.Text + "' where model='" + txtMModel.Text + "' and hpnumber='" + txtMHpnumber.Text + "' ";
            nc.udata(query);
            MessageBox.Show("Data Successfully update", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtMModel.Text = "";
            txtMHpnumber.Text = "";
            txtMhpcode.Text = "";
            loaddgv();

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtMModel.Text = dataGridView1.CurrentRow.Cells["model"].Value.ToString();
            txtMHpnumber.Text = dataGridView1.CurrentRow.Cells["hpnumber"].Value.ToString();
            txtMhpcode.Text = dataGridView1.CurrentRow.Cells["hpcode"].Value.ToString();
            lblid.Text = dataGridView1.CurrentRow.Cells["id"].Value.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string query = "delete from mmiosmodel where id='" + lblid.Text + "' ";

            if (txtMModel.Text == "" || txtMHpnumber.Text == "" || txtMhpcode.Text == "")
            {
                MessageBox.Show("Kindly select a row from datagridview", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                nc.deldata(query);
                MessageBox.Show("Data Successfully Deleted", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMModel.Text = "";
                txtMHpnumber.Text = "";
                txtMhpcode.Text = "";
                lblid.Text = "";
                loaddgv();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //test
            if (timelimit == true && comboBox1.Text != "")
            {

                dataGridView1.Refresh();

                string query = "select * from mmioshistory where " + comboBox1.Text + " = '" + txtSearchValue.Text + "' and convert(hsdate,datetime) between '" + dateTimePicker1.Text + "' and '" + dateTimePicker2.Text + "' order by id desc";
                DataTable dt = new DataTable();
                dt = nc.filldata(query).Tables[0];
                dataGridView2.DataSource = dt;




            }
            else if (timelimit == true && comboBox1.Text == "")
            {
                string query = "select * from mmioshistory where convert(hsdate,datetime) between '" + dateTimePicker1.Text + "' and '" + dateTimePicker2.Text + "' order by id desc";
                DataTable dt = new DataTable();
                dt = nc.filldata(query).Tables[0];
                dataGridView2.DataSource = dt;
            }
            else
            {

                dataGridView1.Refresh();

                string query = "select * from mmioshistory where " + comboBox1.Text + " = '" + txtSearchValue.Text + "' order by id desc";
                DataTable dt = new DataTable();
                dt = nc.filldata(query).Tables[0];
                dataGridView2.DataSource = dt;



            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                timelimit = true;
            }
            else
            {
                timelimit = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("No data to export");
            }
            else
            {
                Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Open(@"C:\template\Template.xlsx");
                Microsoft.Office.Interop.Excel._Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];

                worksheet.Name = "Goods Incoming";

                // Write the headers
                for (int i = 0; i < dataGridView2.Columns.Count; i++)
                {
                    worksheet.Cells[1, i + 1] = dataGridView2.Columns[i].HeaderText;
                }

                // Prepare the data array
                int rowsCount = dataGridView2.Rows.Count;
                int columnsCount = dataGridView2.Columns.Count;
                object[,] data = new object[rowsCount, columnsCount];

                // Fill the data array
                for (int i = 0; i < rowsCount; i++)
                {
                    for (int j = 0; j < columnsCount; j++)
                    {
                        data[i, j] = dataGridView2.Rows[i].Cells[j].Value?.ToString();
                    }
                }

                // Get the destination range to set values in bulk
                Microsoft.Office.Interop.Excel.Range range = worksheet.Range[worksheet.Cells[2, 1], worksheet.Cells[rowsCount + 1, columnsCount]];

                // Set the values in the range
                range.Value = data;

                // Save the file
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = "GoodsIncoming" + DateTime.Now.ToShortDateString().Replace("/", "");
                saveFileDialog.DefaultExt = ".xlsx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    workbook.SaveAs(saveFileDialog.FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing);
                }

                app.Quit();
            }
        }
    }
}
