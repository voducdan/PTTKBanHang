using System;
using System.Collections.Generic;
using System.Text;

namespace PTTKBanHang.Models
{
    public class AdSelectedProduct
    {
        private int _id;
        private string _name;
        private string _type;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }
        public AdSelectedProduct(string name, int id, string type)
        {
            this._id = id;
            this._name = name;
            this._type = type;
        }
    }
}
