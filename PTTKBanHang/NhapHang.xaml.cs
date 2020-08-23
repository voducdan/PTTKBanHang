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
using System.ComponentModel;
using System.Data.SqlClient;
using PTTKBanHang.Models;

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
            ProvidersViewModel vm = new ProvidersViewModel();
            DataContext = vm;
        }
    }

    public class OracleDBAccess
    {
        public static OracleConnection ConnectOracle()
        {

            OracleConnection con = new OracleConnection();
            OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();
            ocsb.UserID = "PTTK";
            ocsb.Password = "qqq";
            ocsb.DataSource = @"(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XE)))";
            con.ConnectionString = ocsb.ConnectionString;
            return con;
        }
        public static IList<Provider> GetProviders()
        {
            IList<Provider> providers = new List<Provider>();
            OracleConnection con = OracleDBAccess.ConnectOracle();
            SqlParameter providerName = new SqlParameter();
            string query = "SELECT n.TENNCC FROM NHACUNGCAP n";
            OracleCommand occmd = new OracleCommand(query, con);
            con.Open();
            OracleDataReader ocr = occmd.ExecuteReader();
            while (ocr.Read())
            {
                var provider = new Provider(ocr.GetValue(0).ToString());
                providers.Add(provider);
            }
            con.Close();
            return providers;
        }
    }
    public class ProvidersViewModel
    {
        private readonly CollectionView _providers;
        private readonly string _created_date;
        private string _provider;

        public event PropertyChangedEventHandler PropertyChanged;
        
        public ProvidersViewModel()
        {
            IList<Provider> providers = new List<Provider>();
            providers = OracleDBAccess.GetProviders();

            _providers = new CollectionView(providers);
            string created_date = DateTime.Today.Day.ToString() + "/" + DateTime.Today.Month.ToString() + "/" + DateTime.Today.Year.ToString();
            _created_date = created_date;
        }
        public CollectionView Providers
        {
            get { return _providers; }
        }

        public string Provider
        {
            get { return _provider; }
            set
            {
                if (_provider == value) return;
                _provider = value;
                OnPropertyChanged("Provider");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public string CreatedDate 
        {
            get { return _created_date; }
        }
    }

}
