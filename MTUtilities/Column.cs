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

        /// <summary>
        /// This is the scoring method for the columns based on the type of operator used and the frequency of the occurence of that column.
        /// The algo is as follows:
        /// Each column receives a score of how many times it was called for against which operator. Each opertor has a precedence score which are in consideration with the index suggestions.
        /// </summary>
        public long CalculateScore()
        {
            if (this.TotalNumOfOccurrences != 0
                    && (TotalNumOfOccurrences == UsedForJoinOccurrences + ProjectOccurences + WhereOccurences + GroupByOccurences + HavingOccurences + OrderByOccurences + UnknownOccurences))
            {
                Score =
                        (UsedForJoinOccurrences * Convert.ToInt64(ClauseType.Join))
                    + (ProjectOccurences * Convert.ToInt64(ClauseType.Project))
                    + (OrderByOccurences * Convert.ToInt64(ClauseType.orderby))
                    //+   (WhereOccurences + Convert.ToInt64(OperatorScore.))
                    + (HavingOccurences * Convert.ToInt64(ClauseType.having))
                    + (GroupByOccurences * Convert.ToInt64(ClauseType.GroupBy));
                // implement where clause
                foreach (KeyValuePair<string, long> op in this.WhereComparisonOperators)
                {
                    switch (op.Key)
                    {
                        case "equalsTo":
                            Score = Score + op.Value * Convert.ToInt64(OperatorScore.EqualTo);
                            break;
                        case "greaterThan":
                            Score = Score + op.Value * Convert.ToInt64(OperatorScore.Greater_Or_LessThan);
                            break;
                        case "lessThan":
                            Score = Score + op.Value * Convert.ToInt64(OperatorScore.Greater_Or_LessThan);
                            break;
                        case "unknown":
                            Score = Score + op.Value * Convert.ToInt64(OperatorScore.unknown);
                            break;
                        default:
                            break;
                    }

                }

                return Score;
            }
            else
                return Score;

        }

    }
}


public enum OperatorScore
{
    EqualTo = 5,
    Greater_Or_LessThan = 4,
    Like_Or_BetweenAnd = 3,
    NotEqualTo = 2, // This will not be included in the score calculation since NotEqualTo clause is as good as a tableScan.
    unknown = 1
    
}

public enum ClauseType
{
    Project = 1,
    having = 2,
    GroupBy = 3,
    orderby = 3,
    Join = 4,
    Where = 5
}