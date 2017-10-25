using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt_WPF.Classes.Database
{
    public interface MysqlObject
    {
        void GetFromDatabase();
        /*runquerywithreturn*/
        void CreateFromRow(Row Table);
        /*creates object from mysql row */
        void UploadToDatabase();
        /*insert*/
        void UpdateInDatabase();
        /*update*/
    }
}
