using System;
using System.Collections.Generic;
using System.Text;

namespace PTTKBanHang.Models
{
    public class Customer
    {
        private string _username;

        public string userName
        {
            get { return _username; }
            set { _username = value; }
        }
        public Customer(string username)
        {
            this._username = username;
        }
    }
}
