using System;
using System.Collections.Generic;
using System.Text;

namespace PTTKBanHang.Models
{
    public class Provider
    {
        private string _name;
        private int _id;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public int Id
        {
            get { return _id; }
            set { }
        }
        public Provider(string name, int id)
        {
            this._name = name;
            this._id = id;
        }
    }
}
