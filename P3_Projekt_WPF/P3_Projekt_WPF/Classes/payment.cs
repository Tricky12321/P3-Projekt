using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3_Projekt_WPF.Classes.Utilities;
using P3_Projekt_WPF.Classes.Database;
namespace P3_Projekt_WPF.Classes
{
    public class Payment : MysqlObject
    {
        public int ID;
        public decimal Amount;
        public int ReceiptID;
        public PaymentMethod_Enum PaymentMethod;

        public Payment(int ReceiptID, decimal Amount, PaymentMethod_Enum PaymentMethod)
        {
            this.ReceiptID = ReceiptID;
            this.Amount = Amount;
            this.PaymentMethod = PaymentMethod;
        }

        public Payment(Row Data)
        {
            CreateFromRow(Data);
        }
        public Payment(int ID)
        {
            this.ID = ID;
            GetFromDatabase();
        }

        public void GetFromDatabase()
        {
            string sql = $"SELECT * FROM `payments` WHERE `id` = '{ID}'";
            CreateFromRow(Mysql.RunQueryWithReturn(sql).RowData[0]);
        }

        public void CreateFromRow(Row Table)
        {
            ID = Convert.ToInt32(Table.Values[0]);
            PaymentMethod = (PaymentMethod_Enum)Convert.ToInt32(Convert.ToDecimal(Table.Values[1]));
            Amount = Convert.ToDecimal(Table.Values[2]);
            ReceiptID = Convert.ToInt32(Table.Values[3]);
        }

        public void UploadToDatabase()
        {
            string sql = "INSERT INTO `payments` (`id`, `amount`, `payment_method`, `receipt_id`)" +
                $" VALUES (NULL, '{Amount.ToString().Replace(',', '.')}', '{Convert.ToInt32(PaymentMethod)}','{ReceiptID}')";
            Mysql.RunQuery(sql);
        }

        public void UpdateInDatabase()
        {
            string sql = $"UPDATE `payments` SET " +
                $"`amount` = '{Amount.ToString().Replace(',', '.')}'," +
                $"`payment_method` = '{Convert.ToInt32(PaymentMethod)}'," +
                $"`receipt_id` = '{ReceiptID}' " +
                $"WHERE `id` = {ID};";
            Mysql.RunQuery(sql);
        }
    }
}
