using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes
{
    class Receipt
    {
        private int _id;
        private List<Transaction> _transactions;
        private int _numberOfProducts;
        private int _totalPrice;
        private int _paidPrice;
        private string _paymentMethod;
        private DateTime _date;
    }
}
