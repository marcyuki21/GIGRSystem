using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;

namespace GIGRSystem
{
    class Class1
    {
        public MySqlConnection mycon;
        public MySqlCommand mycom;
        public MySqlDataReader myread;
        public MySqlDataAdapter myadapt;
        public string connectionHS = "Server=192.168.4.137;Port=3306;Database=hs;User ID=root;Password=pass1234;";
        public string connection137 = "Server=192.168.4.137;Port=3306;Database=mmios;User ID=root;Password=pass1234;";


        public Boolean dataExist(string query)
        {
            Boolean ex = false;
            try
            {
                mycon = new MySqlConnection(connection137);
                mycon.Open();
                mycom = new MySqlCommand(query, mycon);
                myread = mycom.ExecuteReader();
                while (myread.Read())
                {
                    ex = true;
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

            return ex;
        }


        public string getdata(string query)
        {
            string data = string.Empty;
            try
            {
                mycon = new MySqlConnection(connection137);
                mycon.Open();
                mycom = new MySqlCommand(query, mycon);
                myread = mycom.ExecuteReader();
                while (myread.Read())
                {
                    data = myread.GetValue(0).ToString();
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
            return data;


        }
        public Boolean hsdataExist(string query)
        {
            Boolean ex = false;
            try
            {
                mycon = new MySqlConnection(connectionHS);
                mycon.Open();
                mycom = new MySqlCommand(query, mycon);
                myread = mycom.ExecuteReader();
                while (myread.Read())
                {
                    ex = true;
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

            return ex;
        }


        public string hsgetdata(string query)
        {
            string data = string.Empty;
            try
            {
                mycon = new MySqlConnection(connectionHS);
                mycon.Open();
                mycom = new MySqlCommand(query, mycon);
                myread = mycom.ExecuteReader();
                while (myread.Read())
                {
                    data = myread.GetValue(0).ToString();
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
            return data;


        }
        public void iData(string query)
        {
            try
            {
                mycon = new MySqlConnection(connection137);
                mycon.Open();
                mycom = new MySqlCommand(query, mycon);
                mycom.ExecuteNonQuery();
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

        public DataSet filldata(string query)
        {

            DataSet dt = new DataSet();
            try
            {
                mycon = new MySqlConnection(connection137);
                mycon.Open();
                myadapt = new MySqlDataAdapter(query, mycon);
                myadapt.Fill(dt);

                mycon.Close();
                mycon.Dispose();

            }
            catch (Exception)
            {

                mycon.Close();
                mycon.Dispose();

                throw;
            }

            return dt;

        }

        public void udata(string query)
        {

            try
            {
                mycon = new MySqlConnection(connection137);
                mycon.Open();
                mycom = new MySqlCommand(query, mycon);
                myread = mycom.ExecuteReader();
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


        public void deldata(string query)
        {

            try
            {
                mycon = new MySqlConnection(connection137);
                mycon.Open();
                mycom = new MySqlCommand(query, mycon);
                myread = mycom.ExecuteReader();
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



    }
}
