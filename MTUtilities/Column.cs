using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTUtilities
{
    public class Column
    {
        // Identifying details
        public long Column_Object_Id = 0;
        public string Name = string.Empty;

        // Statistical Parameters
        public float ReductionFactor= 0;
        public float Selectivity = 0;

        // This is the score used for ranking analysis and calculated based on the Occurence parameters.
        public long Score = 0;

        // Datatype Parameters
        public Boolean IsNull = false;
        public Boolean IsIndexed = false;
        public string Datatype = string.Empty;

        // Occurence Parameters
       
        public int TotalNumOfOccurrences = 0;
        public int UsedForJoinOccurrences = 0;
        public int ProjectOccurences = 0;
        public int WhereOccurences = 0;
        public int GroupByOccurences = 0;
        public int HavingOccurences = 0;
        public int OrderByOccurences = 0;
        public int UnknownOccurences = 0; // These are the occurences when the parser could not determine the tablename associated with the column or it is ambigious.
        public bool IsCandidateCompositeKey = false;
        public Dictionary<string, long> WhereComparisonOperators = new Dictionary<string, long>();

        
        public void CalculateScore()
        {

        }


    }
}
