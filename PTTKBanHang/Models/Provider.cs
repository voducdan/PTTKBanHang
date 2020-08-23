using System;
using System.Collections.Generic;
using System.Text;

namespace PTTKBanHang.Models
{
    public class Provider
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Provider(string name)
        {
            this._name = name;
        }
    }
}
