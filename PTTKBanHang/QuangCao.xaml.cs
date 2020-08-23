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
using System.Data.Common;
using System.Collections.ObjectModel;

namespace PTTKBanHang
{
    /// <summary>
    /// Interaction logic for QuangCao.xaml
    /// </summary>
    public partial class QuangCao : Window
    {
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

        public QuangCao()
        {
            InitializeComponent();
            //string created_date = DateTime.Today.Day.ToString() + "/" + DateTime.Today.Month.ToString() + "/" + DateTime.Today.Year.ToString();
            //Created_date.Text = created_date;
            //try
            //{
            //    OracleConnection con = ConnectOracle();

            //    string selectColls = "SELECT MASP, TENSP, LOAISP, DONGIA FROM SANPHAM";
            //    OracleCommand occmd = new OracleCommand(selectColls, con);
            //    con.Open();
            //    OracleDataReader ocr = occmd.ExecuteReader();
            //    DataTable ds = new DataTable();
            //    ds.Load(ocr);
            //    ProviderProducts.ItemsSource = ds.DefaultView;
            //    con.Close();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.ToString());
            //}
        }
    }

    public class Providers
    {
        private string _Providers
        {
            get { return _Providers; }
            set
            {
                _Providers = value;
            }
        }
    }
}

//private string _Colloborator;
//public string Colloborator
//{
//    get { return _Colloborator; }
//    set { _Colloborator = value;
//        if (_Colloborator != null)
//            getColl(_Colloborator);
//        OnPropertyChanged("Colloborator");
//    }
//}

//private ObservableCollection<Colloborator> _colls;

//private void getColl(string selectedColl)
//{
//    try
//    {
//        OracleConnection con = ConnectOracle();
//        string sqlColl = "SELECT TENNCC FROM NHACUNGCAP";
//        OracleCommand occmd = new OracleCommand(sqlColl, con);
//        con.Open();
//        OracleDataReader ocr = occmd.ExecuteReader();
//        DataTable ds = new DataTable();
//        ds.Load(ocr);
//        ProviderProducts.ItemsSource = ds.DefaultView;
//        con.Close();
//    }
//    catch (Exception e)
//    {
//        Console.WriteLine(e.ToString());
//    }
//}
