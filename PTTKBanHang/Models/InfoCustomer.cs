using System;
using System.Collections.Generic;
using System.Text;

namespace PTTKBanHang.Models
{
    public class InfoCustomer
    {
        private string _name;
        private string _email;
        private string _phonenumber;
        private string _address;

        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string email
        {
            get { return _email; }
            set { _email = value; }
        }

        public string phonenumber
        {
            get { return _phonenumber; }
            set { _phonenumber = value; }
        }

        public string address
        {
            get { return _address; }
            set { _address = value; }
        }
        public InfoCustomer()
        {

        }
        public InfoCustomer(string name, string email, string phonenumber, string address)
        {
            this._name = name;
            this._email = email;
            this._phonenumber = phonenumber;
            this._address = address;
        }
    }
}
