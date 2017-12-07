using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTUtilities
{
    public class ScoreAnalysis
    {
        // Holds the final score for each table and column in the 2 different dictionaries.
        Dictionary<string, long> TableScore = new Dictionary<string, long>();
        Dictionary<string, long> ColumnScore = new Dictionary<string, long>();

        /// <summary>
        /// This method scores the tables according to the number of occurences in the query workload.
        /// This is used for frequency of access while analyzing the importance of a table in the index suggestions
        /// </summary>
        public void ScoreTables()
        {
            // run the Scoring for all the tables spotted in the workload.
            foreach (KeyValuePair<string,DBTable> tablename in Utilities.DictParsedTables)
            {
                DBTable dBTable = tablename.Value;
                foreach (KeyValuePair<string,long> occurence in dBTable.DictOccurencesStmts)
                {
                    TableScore[tablename.Key] += occurence.Value;
                }

                foreach (KeyValuePair<long, Column> ColmItem in dBTable.DictColumns)
                {
                    
                }


            } 

        }

        /// <summary>
        /// This method scores the columns of a table according to the number of occurences, type of operator, clause in which the column was used, etc.
        /// This is part of a scoring algorithm which will decide which columns are good candidates for an index on a particular table.
        /// </summary>
        public void ScoreColumn()
        {

        }

        

    }
}
