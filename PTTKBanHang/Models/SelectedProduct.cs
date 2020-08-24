using System;
using System.Collections.Generic;
using System.Text;

namespace PTTKBanHang.Models
{
    public class SelectedProduct
    {
        private int _id;
        private string _name;
        private float _price;
        private int _quantity;
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
        public float Price
        {
            get { return _price; }
            set { _price = value; }
        }
        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }
        public SelectedProduct(string name, float price, int id, int quantity)
        {
            this._id = id;
            this._name = name;
            this._price = price;
            this._quantity = quantity;
        }
    }
}
