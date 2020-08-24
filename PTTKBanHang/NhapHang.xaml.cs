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
            string query = @"
                            SELECT DISTINCT N.TENNCC , N.MANCC
                            FROM NHACUNGCAP_SANPHAM ns 
                            INNER JOIN SANPHAM s 
                            ON NS.MASP = S.MASP 
                            INNER JOIN NHACUNGCAP n 
                            ON N.MANCC = NS.MANCC 
                            WHERE S.LOAISP <> 'QUANG CAO'
                            ";
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
        public static IList<ProviderProduct> GetProviderProducts(string ProviderName)
        {
            IList<ProviderProduct> providerProducts = new List<ProviderProduct>();
            OracleConnection con = OracleDBAccess.ConnectOracle();
            con.Open();
            OracleCommand oraCommand = new OracleCommand(@"SELECT S.TENSP, S.DONGIA, S.MASP
                                                            FROM NHACUNGCAP_SANPHAM ns
                                                            INNER JOIN NHACUNGCAP n
                                                            ON NS.MANCC = N.MANCC
                                                            INNER JOIN SANPHAM s
                                                            ON NS.MASP = S.MASP
                                                            WHERE N.TENNCC = :ProviderName", con);
            oraCommand.Parameters.Add(new OracleParameter("ProviderName", OracleDbType.NVarchar2)).Value = ProviderName;
            OracleDataReader ocr = oraCommand.ExecuteReader();
            while (ocr.Read())
            {
                var product = new ProviderProduct(ocr.GetValue(0).ToString(), Int32.Parse(ocr.GetValue(1).ToString()), Int32.Parse(ocr.GetValue(2).ToString()));
                providerProducts.Add(product);
            }
            con.Close();
            return providerProducts;
        }
        public static int InsertOrderReport(int MaNCC, int MaDNH)
        {
            try
            {
                OracleConnection con = OracleDBAccess.ConnectOracle();
                con.Open();
                OracleCommand oraCommand = new OracleCommand($"INSERT INTO DONNHAPHANG (MADNH ,MANV ,MANCC ,NGAYLAPDON) VALUES ({MaDNH},3 ,{MaNCC} ,TO_DATE(SYSDATE))", con);
                int res = oraCommand.ExecuteNonQuery();
                return res;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return 0;
            }
        }
        public static int InsertOrderReportDetail(int MaDNH, List<SelectedProduct> selectedProducts)
        {
            try
            {
                OracleConnection con = OracleDBAccess.ConnectOracle();
                con.Open();
                foreach (SelectedProduct selectedProduct in selectedProducts)
                {
                    OracleCommand oraCommand = new OracleCommand($"INSERT INTO CHITIETNHAPHANG(MADNH ,MASP,SOLUONG) VALUES({MaDNH},{selectedProduct.Id},{selectedProduct.Quantity})", con);
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
        public static int GetOrderReportId()
        {
            try
            {
                OracleConnection con = OracleDBAccess.ConnectOracle();
                int MaDNH = 0;
                con.Open();
                OracleCommand oraCommand = new OracleCommand("SELECT max(d.MADNH ) FROM DONNHAPHANG d", con);
                OracleDataReader res = oraCommand.ExecuteReader();
                while (res.Read())
                {
                    MaDNH = Int32.Parse(res.GetValue(0).ToString());
                }
                con.Close();
                return MaDNH + 1;
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
        private ProviderProduct _product;
        private SelectedProduct _selectedProduct;
        private ObservableCollection<ProviderProduct> _providerProducts;
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<SelectedProduct> _selectedProducts = new ObservableCollection<SelectedProduct>();
        private float _totalPrice;
        private ICommand _addProduct;
        private ICommand _removeProduct;
        private ICommand _confirm;
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
        public ObservableCollection<SelectedProduct> SelectedProducts
        {
            get { return _selectedProducts; }
            set
            {
                if (_selectedProducts == value) return;
                _selectedProducts = value;
                //OnPropertyChanged("ProviderProducts");
            }
        }

        public ObservableCollection<ProviderProduct> ProviderProducts
        {
            get { return _providerProducts; }
            set
            {
                if (_providerProducts == value) return;
                _providerProducts = value;
                //OnPropertyChanged("ProviderProducts");
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
                _selectedProducts.Clear();
                OnPropertyChanged("Provider");
                OnPropertyChanged("ProviderProducts");
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
        public ProviderProduct Product
        {
            get { return _product; }
            set
            {
                if (_product == value) return;
                _product = value;
            }
        }
        public SelectedProduct SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                if (_selectedProduct == value) return;
                _selectedProduct = value;
            }
        }
        public float TotalPrice
        {
            get
            {
                return _totalPrice;
            }
            set
            {
                if (_totalPrice == value)
                    return;
                _totalPrice = value;
            }
        }
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                IList<ProviderProduct> providerProducts = new List<ProviderProduct>();
                providerProducts = OracleDBAccess.GetProviderProducts(_provider);
                _providerProducts = new ObservableCollection<ProviderProduct>(providerProducts);
            }
        }
        public string CreatedDate
        {
            get { return _created_date; }
        }
        public ICommand AddProduct
        {
            get
            {
                if (_addProduct == null)
                {
                    _addProduct = new RelayCommand(
                          param => this.Add(),
                          param => this.CanAdd()
                    );
                }
                return _addProduct;
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
                foreach (SelectedProduct temp in _selectedProducts)
                {
                    if (temp.Name == _product.Name)
                    {
                        int idx = _selectedProducts.IndexOf(temp);
                        _selectedProducts.RemoveAt(idx);
                        break;
                    }
                }
                SelectedProduct selectedProduct = new SelectedProduct(_product.Name, _product.UnitPrice * _product.Quantity, _product.Id, _product.Quantity);
                _selectedProducts.Add(selectedProduct);
                List<SelectedProduct> selectedProductsList = new List<SelectedProduct>(_selectedProducts);
                TotalPrice = BusinessHandle.CalcTotalPrice(selectedProductsList);
                OnPropertyChanged("TotalPrice");
            }
        }
        public ICommand RemoveProduct
        {
            get
            {
                if (_removeProduct == null)
                {
                    _removeProduct = new RelayCommand(
                          param => this.Remove(),
                          param => this.CanRemove()
                    );
                }
                return _removeProduct;
            }
        }
        private bool CanRemove()
        {
            return true;
        }

        private void Remove()
        {
            if (_selectedProduct == null)
            {
                MessageBox.Show("Please select product!!!");
            }
            else
            {
                _selectedProducts.Remove(_selectedProduct);
                List<SelectedProduct> selectedProductsList = new List<SelectedProduct>(_selectedProducts);
                TotalPrice = BusinessHandle.CalcTotalPrice(selectedProductsList);
                OnPropertyChanged("SelectedProducts");
                OnPropertyChanged("TotalPrice");
            }
        }

        public ICommand Confirm
        {
            get
            {
                if (_confirm == null)
                {
                    _confirm = new RelayCommand(
                          param => this.ConfirmExecute(),
                          param => this.CanConfirm()
                    );
                }
                return _confirm;
            }
        }

        private bool CanConfirm()
        {
            return true;
        }

        private void ConfirmExecute()
        {
            int MaNCC = _providerId;
            int MaDNH = OracleDBAccess.GetOrderReportId();
            BusinessHandle.Order(new List<SelectedProduct>(_selectedProducts), MaNCC, MaDNH);
            _selectedProducts.Clear();
            MessageBox.Show("Order completed");
        }
    }

    public class BusinessHandle
    {

        public static float CalcTotalPrice(List<SelectedProduct> selectedProducts)
        {
            float totalPrice = 0;
            foreach (SelectedProduct selectedProduct in selectedProducts)
            {
                totalPrice += selectedProduct.Price;
            }
            return totalPrice;
        }
        public static void Order(List<SelectedProduct> selectedProducts, int MaNCC, int MaDNH)
        {
            OracleDBAccess.InsertOrderReport(MaNCC, MaDNH);
            OracleDBAccess.InsertOrderReportDetail(MaDNH, selectedProducts);
        }
    }

    public class RelayCommand : ICommand
    {
        #region Fields

        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        #endregion // Fields

        #region Constructors

        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion // Constructors

        #region ICommand Members

        public bool CanExecute(object parameters)
        {
            return _canExecute == null ? true : _canExecute(parameters);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameters)
        {
            _execute(parameters);
        }

        #endregion // ICommand Members
    }


}
