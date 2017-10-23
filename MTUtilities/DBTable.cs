using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTUtilities
{
    public class DBTable
    {
        public long Table_Object_Id = 0; // Unique identifier of each table in the system catalog.
        public List<Column> columnList = new List<Column>();

    }
}
