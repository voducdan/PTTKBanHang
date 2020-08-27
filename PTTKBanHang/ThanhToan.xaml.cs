using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Oracle.ManagedDataAccess.Client;
using System.ComponentModel;
using PTTKBanHang.Models;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Security.Policy;
using Microsoft.VisualBasic;
using System.Windows.Markup;
using System.Printing;

namespace PTTKBanHang
{
    /// <summary>
    /// Interaction logic for ThanhToan.xaml
    /// </summary>
    public partial class ThanhToan : Window
    {
        public ThanhToan()
        {
            InitializeComponent();
            CustomersViewModel vm = new CustomersViewModel();
            DataContext = vm;
        }

        public class OracleDBAccessTT
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
            public static IList<Customer> GetCustomers()
            {
                IList<Customer> customers = new List<Customer>();
                OracleConnection con = OracleDBAccessTT.ConnectOracle();
                string query = @"
                            SELECT USERNAME FROM KHACHHANG
                            ";
                OracleCommand occmd = new OracleCommand(query, con);
                con.Open();
                OracleDataReader ocr = occmd.ExecuteReader();
                while (ocr.Read())
                {
                    var customer = new Customer(ocr.GetValue(0).ToString());
                    customers.Add(customer);
                }
                con.Close();
                return customers;
            }
            public static InfoCustomer GetInfoCustomer(string customer)
            {
                OracleConnection con = OracleDBAccessTT.ConnectOracle();
                con.Open();
                string c = customer.Trim();
                string query = @"SELECT TENKH, EMAIL, SDT, DIACHI FROM KHACHHANG WHERE USERNAME = '" + c + "'";
                OracleCommand occmd = new OracleCommand(query, con);
                OracleDataReader ocr = occmd.ExecuteReader();
                string name = "";
                string email = "";
                string phone = "";
                string address = "";
                while (ocr.Read())
                {
                    name = ocr.GetValue(0).ToString();
                    email = ocr.GetValue(1).ToString();
                    phone = ocr.GetValue(2).ToString();
                    address = ocr.GetValue(3).ToString();
                }
                InfoCustomer infoCustomer = new InfoCustomer(name, email, phone, address);
                con.Close();
                return infoCustomer;

            }
            public static List<Bill> GetMaDDH(string username)
            {
                OracleConnection con = OracleDBAccessTT.ConnectOracle();
                List<Bill> Bills = new List<Bill>();
                con.Open();
                string c = username.Trim();
                string query = @"SELECT D.MADDH
                                FROM DONDATHANG D, KHACHHANG K
                                WHERE D.MAKH = K.MAKH AND K.USERNAME= '" + c + "'";
                OracleCommand occmd = new OracleCommand(query, con);
                OracleDataReader ocr = occmd.ExecuteReader();
                while (ocr.Read())
                {
                    var temp = new Bill(ocr.GetValue(0).ToString());
                    Bills.Add(temp);
                }
               
                con.Close();
                return Bills;

            }

            public static List<InfoProduct> GetInfoProduct(string maDDH)
            {
                OracleConnection con = OracleDBAccessTT.ConnectOracle();
                List<InfoProduct> infoProducts = new List<InfoProduct>();
                con.Open();
                string c = maDDH.Trim();
                string query = @"SELECT S.TENSP, S.DONGIA
                                    FROM SANPHAM S, CHITIETDATHANG C, DONDATHANG D
                                    WHERE D.MADDH = C.MADDH  AND S.MASP=C.MASP AND D.MADDH = '" + c + "'";
                OracleCommand occmd = new OracleCommand(query, con);
                OracleDataReader ocr = occmd.ExecuteReader();
                string tenSP = "";
                string donGia = "";
                while (ocr.Read())
                {
                    tenSP = ocr.GetValue(0).ToString();
                    donGia = ocr.GetValue(1).ToString();
                    infoProducts.Add(new InfoProduct(tenSP, donGia));
                }
                InfoProduct infoProduct = new InfoProduct(tenSP,donGia);
                con.Close();
                return infoProducts;

            }
            //public static IList<ProviderProduct> GetProviderProducts(string ProviderName)
            //{
            //    IList<ProviderProduct> providerProducts = new List<ProviderProduct>();
            //    OracleConnection con = OracleDBAccess.ConnectOracle();
            //    con.Open();
            //    OracleCommand oraCommand = new OracleCommand(@"SELECT S.TENSP, S.DONGIA, S.MASP
            //                                                FROM NHACUNGCAP_SANPHAM ns
            //                                                INNER JOIN NHACUNGCAP n
            //                                                ON NS.MANCC = N.MANCC
            //                                                INNER JOIN SANPHAM s
            //                                                ON NS.MASP = S.MASP
            //                                                WHERE N.TENNCC = :ProviderName", con);
            //    oraCommand.Parameters.Add(new OracleParameter("ProviderName", OracleDbType.NVarchar2)).Value = ProviderName;
            //    OracleDataReader ocr = oraCommand.ExecuteReader();
            //    while (ocr.Read())
            //    {
            //        var product = new ProviderProduct(ocr.GetValue(0).ToString(), Int32.Parse(ocr.GetValue(1).ToString()), Int32.Parse(ocr.GetValue(2).ToString()));
            //        providerProducts.Add(product);
            //    }
            //    con.Close();
            //    return providerProducts;
        }

        public class CustomersViewModel : INotifyPropertyChanged
        {
            private readonly CollectionView _customers;
            private readonly string _created_date;
            private string _customer;
            private InfoCustomer _infoCustomer = new InfoCustomer("", "", "", "");
            public event PropertyChangedEventHandler PropertyChanged;
            private ObservableCollection<SelectedProduct> _selectedProducts = new ObservableCollection<SelectedProduct>();
            private string _maDDH;
            private List<Bill> _bills;
            private ObservableCollection<InfoProduct> _infoProducts;
            private float _totalPrice;
            private ICommand _addProduct;
            private ICommand _removeProduct;
            private ICommand _confirm;

            public CustomersViewModel()
            {
                IList<Customer> customers = new List<Customer>();

                customers = OracleDBAccessTT.GetCustomers();

                _customers = new CollectionView(customers);

                string created_date = DateTime.Today.Day.ToString() + "/" + DateTime.Today.Month.ToString() + "/" + DateTime.Today.Year.ToString();
                _created_date = created_date;

            }
            public string DDH
            {
                get
                {
                    return _maDDH;
                }
                set
                {
                    if (_maDDH == value) return;
                    _maDDH = value;
                    _infoProducts = new ObservableCollection<InfoProduct>(OracleDBAccessTT.GetInfoProduct(_maDDH));
                    OnPropertyChanged("InfoProducts");

                }
            }
            public string Customer
            {
                get { return _customer; }
                set
                {
                    if (_customer == value) return;
                    _customer = value;
                    _infoCustomer = OracleDBAccessTT.GetInfoCustomer(_customer);
                    _bills = OracleDBAccessTT.GetMaDDH(_customer);
                    OnPropertyChanged("InfoCustomer");
                    OnPropertyChanged("Bills");


                }
            }
            //public string InfoProdcut
            //{
            //    get { return _infoProduct; }
            //    set
            //    {
            //        if (_infoProduct == value) return;
            //        _infoProduct = value;
            //        _infoCustomer = OracleDBAccessTT.GetInfoProduct(_infoProduct);
            //        _bills = OracleDBAccessTT.GetMaDDH(_customer);
            //        OnPropertyChanged("InfoProduct");
            //    }
            //}

            public InfoCustomer InfoCustomer
            {
                get { return _infoCustomer; }
            }
            public CollectionView Customers
            {
                get { return _customers; }
            }
            public string CreatedDate
            {
                get { return _created_date; }
            }
            public List<Bill> Bills
            {
                get { return _bills; }
            }

            public ObservableCollection<InfoProduct> InfoProducts
            {
                get { return _infoProducts; }
            }

            //public InfoCustomer InfoCustomer
            //{
            //    get { return _infoCustomer; }
            //    set
            //    {
            //        if (_infoCustomer == value) return;
            //        _infoCustomer = value;
            //    }
            //}
            private void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Order completed");
        }
    }
}
