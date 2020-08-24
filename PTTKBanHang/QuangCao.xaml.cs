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


namespace PTTKBanHang
{
    /// <summary>
    /// Interaction logic for QuangCao.xaml
    /// </summary>
    public partial class QuangCao : Window
    {
        public QuangCao()
        {
            InitializeComponent();
            ProvidersViewModel vm = new ProvidersViewModel();
            DataContext = vm;
        }

        public class OracleDBAccessQC
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
                OracleConnection con = OracleDBAccessQC.ConnectOracle();
                string query = "SELECT DISTINCT NCC.TENNCC, NCC.MANCC " +
                               "FROM NHACUNGCAP NCC " +
                               "INNER JOIN NHACUNGCAP_SANPHAM NCC_SP " +
                               "ON NCC_SP.MANCC = NCC.MANCC " +
                               "INNER JOIN SANPHAM SP " +
                               "ON SP.MASP = NCC_SP.MASP " +
                               "WHERE SP.LOAISP = 'QUANG CAO'";
                OracleCommand occmd = new OracleCommand(query, con);
                con.Open();
                OracleDataReader ocr = occmd.ExecuteReader();
                while (ocr.Read())
                {
                    var provider = new Provider(ocr.GetValue(0).ToString(), Int32.Parse(ocr.GetValue(1).ToString()));
                    providers.Add(provider);
                }
                con.Close();
                return providers;
            }
            public static IList<AdProviderProduct> GetProviderProducts(string AdProviderName)
            {
                IList<AdProviderProduct> adProviderProducts = new List<AdProviderProduct>();
                OracleConnection con = OracleDBAccessQC.ConnectOracle();
                con.Open();
                OracleCommand oraCommand = new OracleCommand("SELECT DISTINCT SP.TENSP, SP.LOAISP, SP.DONGIA, SP.MASP " +
                                                             "FROM SANPHAM SP " +
                                                             "INNER JOIN NHACUNGCAP_SANPHAM NCC_SP " +
                                                             "ON SP.MASP = NCC_SP.MASP " +
                                                             "INNER JOIN NHACUNGCAP NCC " +
                                                             "ON NCC.MANCC = NCC_SP.MANCC " +
                                                             "WHERE SP.LOAISP <> 'QUANG CAO'", con);
                oraCommand.Parameters.Add(new OracleParameter("AdProviderName", OracleDbType.NVarchar2)).Value = AdProviderName;
                OracleDataReader ocr = oraCommand.ExecuteReader();
                while (ocr.Read())
                {
                    var adProduct = new AdProviderProduct(ocr.GetValue(0).ToString(), float.Parse(ocr.GetValue(2).ToString()), ocr.GetValue(1).ToString(), Int32.Parse(ocr.GetValue(3).ToString()));
                    adProviderProducts.Add(adProduct);
                }
                con.Close();
                return adProviderProducts;
            }

            public static int InsertContractReport(int MAHOPDONG, int MANCC)
            {
                try
                {
                    OracleConnection con = OracleDBAccessQC.ConnectOracle();
                    con.Open();
                    OracleCommand oraCommand = new OracleCommand($"INSERT INTO HOPDONG (MAHOPDONG, MANCC, NGAYLAP, NOIDUNG) VALUES ({MAHOPDONG},{MANCC}, TO_DATE(SYSDATE), NULL)", con);
                    int res = oraCommand.ExecuteNonQuery();
                    return res;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                    return 0;
                }
            }
            public static int InsertContractReportDetail(int MAHOPDONG, List<AdSelectedProduct> adSelectedProducts)
            {
                try
                {
                    OracleConnection con = OracleDBAccessQC.ConnectOracle();
                    con.Open();
                    foreach (AdSelectedProduct adSelectedProduct in adSelectedProducts)
                    {
                        OracleCommand oraCommand = new OracleCommand($"INSERT INTO CHITIETHOPDONG(MAHOPDONG, MASANPHAM) VALUES ({MAHOPDONG},{adSelectedProduct.Id})", con);
                        oraCommand.ExecuteNonQuery();
                    }
                    con.Close();
                    return 1;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                    return 0;
                }
            }
            public static int GetContractReportId()
            {
                try
                {
                    OracleConnection con = OracleDBAccessQC.ConnectOracle();
                    int MAHOPDONG = 0;
                    con.Open();
                    OracleCommand oraCommand = new OracleCommand("SELECT max(HD.MAHOPDONG) FROM HOPDONG HD", con);
                    OracleDataReader res = oraCommand.ExecuteReader();
                    while (res.Read())
                    {
                        MAHOPDONG = Int32.Parse(res.GetValue(0).ToString());
                    }
                    con.Close();
                    return MAHOPDONG + 1;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                    return 0;
                }
            }
        }
        public class ProvidersViewModel : INotifyPropertyChanged
        {
            private readonly CollectionView _providers;
            private readonly string _created_date;
            private string _provider;
            private int _providerId;
            private AdSelectedProduct _adSelectedProduct;
            private AdProviderProduct _product;
            private ObservableCollection<AdProviderProduct> _adProviderProducts;
            private ObservableCollection<AdSelectedProduct> _adSelectedProducts = new ObservableCollection<AdSelectedProduct>();
            public event PropertyChangedEventHandler PropertyChanged;
            private ICommand _adAddProduct;
            private ICommand _adRemoveProduct;
            private ICommand _adConfirm;

            public ProvidersViewModel()
            {
                IList<Provider> providers = new List<Provider>();
                providers = OracleDBAccessQC.GetProviders();
                _providers = new CollectionView(providers);
                string created_date = DateTime.Today.Day.ToString() + "/" + DateTime.Today.Month.ToString() + "/" + DateTime.Today.Year.ToString();
                _created_date = created_date;
            }

            public CollectionView Providers
            {
                get { return _providers; }
            }

            public ObservableCollection<AdSelectedProduct> AdSelectedProducts
            {
                get { return _adSelectedProducts; }
                set
                {
                    if (_adSelectedProducts == value) return;
                    _adSelectedProducts = value;
                }
            }

            public string Provider
            {
                get { return _provider; }
                set
                {
                    if (_provider == value) return;
                    _provider = value;
                    foreach (Provider provider in _providers)
                    {
                        if (provider.Name == _provider)
                        {
                            _providerId = provider.Id;
                            break;
                        }
                    }
                    _adSelectedProducts.Clear();
                    OnPropertyChanged("Provider");
                    OnPropertyChanged("AdProviderProducts");
                }
            }

            public int ProviderId
            {
                get { return _providerId; }
                set
                {
                    if (_providerId == value) return;
                    _providerId = value;
                }
            }

            public AdProviderProduct ListProduct
            {
                get { return _product; }
                set
                {
                    if (_product == value) return;
                    _product = value;
                }
            }

            public AdSelectedProduct AdSelectedProduct
            {
                get { return _adSelectedProduct; }
                set
                {
                    if (_adSelectedProduct == value) return;
                    _adSelectedProduct = value;
                }
            }

            public ObservableCollection<AdProviderProduct> AdProviderProducts
            {
                get { return _adProviderProducts; }
                set
                {
                    if (_adProviderProducts == value) return;
                    _adProviderProducts = value;
                }
            }

            private void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                    IList<AdProviderProduct> adProviderProducts = new List<AdProviderProduct>();
                    adProviderProducts = OracleDBAccessQC.GetProviderProducts(_provider);
                    _adProviderProducts = new ObservableCollection<AdProviderProduct>(adProviderProducts);
                }
            }

            public string CreatedDate
            {
                get { return _created_date; }
            }

            public ICommand AdAddProduct
            {
                get
                {
                    if (_adAddProduct == null)
                    {
                        _adAddProduct = new RelayCommand(
                              param => this.Add(),
                              param => this.CanAdd()
                        );
                    }
                    return _adAddProduct;
                }
            }

            private bool CanAdd()
            {
                return true;
            }

            private void Add()
            {
                if (_product == null)
                {
                    MessageBox.Show("Please select product!!!");
                }
                else
                {
                    foreach (AdSelectedProduct temp in _adSelectedProducts)
                    {
                        if (temp.Name == _product.Name)
                        {
                            int idx = _adSelectedProducts.IndexOf(temp);
                            _adSelectedProducts.RemoveAt(idx);
                            break;
                        }
                    }
                    AdSelectedProduct adSelectedProduct = new AdSelectedProduct(_product.Name, _product.Id, _product.Type);
                    _adSelectedProducts.Add(adSelectedProduct);
                    List<AdSelectedProduct> adSelectedProductsList = new List<AdSelectedProduct>(_adSelectedProducts);
                }
            }

            public ICommand AdRemoveProduct
            {
                get
                {
                    if (_adRemoveProduct == null)
                    {
                        _adRemoveProduct = new RelayCommand(
                              param => this.Remove(),
                              param => this.CanRemove()
                        );
                    }
                    return _adRemoveProduct;
                }
            }

            private bool CanRemove()
            {
                return true;
            }

            private void Remove()
            {
                if (_adSelectedProduct == null)
                {
                    MessageBox.Show("Please select product!!!");
                }
                else
                {
                    _adSelectedProducts.Remove(_adSelectedProduct);
                    List<AdSelectedProduct> adSelectedProductsList = new List<AdSelectedProduct>(_adSelectedProducts);
                    OnPropertyChanged("AdSelectedProducts");
                }
            }

            public ICommand Confirm
            {
                get
                {
                    if (_adConfirm == null)
                    {
                        _adConfirm = new RelayCommand(
                              param => this.ConfirmExecute(),
                              param => this.CanConfirm()
                        );
                    }
                    return _adConfirm;
                }
            }

            private bool CanConfirm()
            {
                return true;
            }

            private void ConfirmExecute()
            {
                int MaNCC = _providerId;
                int MAHOPDONG = OracleDBAccessQC.GetContractReportId();
                BusinessHandleQC.Order(new List<AdSelectedProduct>(_adSelectedProducts), MaNCC, MAHOPDONG);
                _adSelectedProducts.Clear();
                MessageBox.Show("Order completed");
            }
        }
        public class BusinessHandleQC
        {
            public static void Order(List<AdSelectedProduct> adSelectedProducts, int MANCC, int MAHOPDONG)
            {
                OracleDBAccessQC.InsertContractReport(MAHOPDONG, MANCC);
                OracleDBAccessQC.InsertContractReportDetail(MAHOPDONG, adSelectedProducts);
            }
        }
    }
}
