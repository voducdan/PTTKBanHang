using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using Oracle.ManagedDataAccess.Client;


namespace PTTKBanHang
{
    /// <summary>
    /// Interaction logic for NhapHang.xaml
    /// </summary>
    public partial class NhapHang : Window
    {
        public NhapHang()
        {
            InitializeComponent();
            string created_date = DateTime.Today.Day.ToString() + "/" +  DateTime.Today.Month.ToString() + "/" + DateTime.Today.Year.ToString();
            Created_date.Text = created_date;
            try
            {
                OracleConnection con = ConnectOracle();

                string selectRoles = "SELECT * FROM EMP_1712317";

                OracleCommand occmd = new OracleCommand(selectRoles, con);
                con.Open();
                OracleDataReader ocr = occmd.ExecuteReader();
                DataTable ds = new DataTable();
                ds.Load(ocr);
                ProviderProducts.ItemsSource = ds.DefaultView;
                con.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static OracleConnection ConnectOracle()
        {

            OracleConnection con = new OracleConnection();
            OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();
            ocsb.UserID = "PTTK";
            ocsb.Password = "qqq";
            ocsb.DataSource = @"(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)))";
            con.ConnectionString = ocsb.ConnectionString;
            return con;
        }
    }
}
