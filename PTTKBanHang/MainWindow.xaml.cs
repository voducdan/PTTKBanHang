using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PTTKBanHang
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NhapHang_Click(object sender, RoutedEventArgs e)
        {
            var pttk = new NhapHang();
            pttk.Show();
        }

        private void QuangCao_Click(object sender, RoutedEventArgs e)
        {
            var quangcao = new QuangCao();
            quangcao.Show();
        }
    }
}
