using System;
using System.Collections.Generic;
using System.Text;

namespace PTTKBanHang.Models
{
    public class Bill
    {
        private string _maDDH;

        public string MaDDH
        {
            get { return _maDDH; }
            set { _maDDH = value; }
        }
        public Bill(string maDDH)
        {
            this._maDDH = maDDH;
        }

    }
}
