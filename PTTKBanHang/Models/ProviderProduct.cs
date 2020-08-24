using System;
using System.Collections.Generic;
using System.Text;

namespace PTTKBanHang.Models
{
    public class ProviderProduct
    {
        private int _id;
        private string _name;
        private float _unitPrice;
        private int _quantity = 1;
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
        public float UnitPrice
        {
            get { return _unitPrice; }
            set { _unitPrice = value; }
        }
        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }
        public ProviderProduct(string name, float price, int id)
        {
            this._id = id;
            this._name = name;
            this._unitPrice = price;
        }
        public ProviderProduct(string name,float price,int quantity, int id)
        {
            this._id = id;
            this._name = name;
            this._unitPrice = price;
            this._quantity = quantity;
        }
    }
}
