using System;
using System.Collections.Generic;
using System.Text;

namespace PTTKBanHang.Models
{
    public class InfoProduct
    {
        private string _tenSP;
        private string _donGia;

        public string DonGia
        {
            get { return _donGia; }
            set { _donGia = value; }
        }

        public string TenSP
        {
            get { return _tenSP; }
            set { _tenSP = value; }
        }
        public InfoProduct()
        {

        }
        public InfoProduct(string tenSP, string donGia)
        {
            this._tenSP = tenSP;
            this._donGia = donGia;
        }
    }
}
