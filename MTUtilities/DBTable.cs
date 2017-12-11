using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTUtilities
{
    public class DBTable
    {
        // Identifying details
        public long Table_Object_Id = 0; // Unique identifier of each table in the system catalog.
        public string Table_Schema_Name = "";
        public string name = string.Empty;

        // Columns of this table
        public Dictionary<long, Column> DictColumns = new Dictionary<long, Column>();
        public Dictionary<string, long> DictOccurencesStmts = new Dictionary<string, long>();

        // Worload Occurence Details of this table
        public int TotalNumOfOccurrences = 0;
        public long TotalRecords = 0;
        
        // This is the score used for ranking analysis and calculated based on the Occurence parameters.
        public long Score = 0;

        // Database level Metadata
        public Boolean IsPartitioned = false;

        //public int SelectionOccurences = 0;
        //public int UpdateOccurences = 0;
        //public int InsertOccurences = 0;


        
    }
}
