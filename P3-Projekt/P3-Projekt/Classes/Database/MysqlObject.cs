using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3_Projekt.Classes.Database
{
    public interface MysqlObject
    {
        void GetFromDatabase();

        void CreateFromRow(Row Table);

        void UploadToDatabase();

        void UpdateInDatabase();
    }
}
