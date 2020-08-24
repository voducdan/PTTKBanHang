using System;
using System.Collections.Generic;
using System.Text;

namespace PTTKBanHang.Models
{
    public class AdProviderProduct
    {
        private string _name;
        private string _type;
        private float _price;
        private int _id;

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

        public float Price
        {
            get { return _price; }
            set { _price = value; }
        }

        public AdProviderProduct(string name, float price, string type,  int id)
        {
            this._id = id;
            this._name = name;
            this._type = type;
            this._price = price;
        }
    }
}
