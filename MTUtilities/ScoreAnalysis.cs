﻿using gudusoft.gsqlparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTUtilities
{
    public class ScoreAnalysis
    {
        public string TableName = "";
        public string ColumnName = "";
        public long TableScore = 0;
        public long ColumnScore = 0;
        public long ColumnHashScore = 0;
        public long ColumnBinaryScore = 0;
        
        /// <summary>
        /// This method scores the tables according to the number of occurences in the query workload.
        /// This is used for frequency of access while analyzing the importance of a table in the index suggestions
        /// </summary>
        public void ScoreTables()
        {
            // This is the sum of all the scores of individual columns for this table which will be multiplied with table occurences to give the table Weightage in this Workload.
            long CombinedColumnScore = 0;

            // run the Scoring for all the tables spotted in the workload.
            foreach (KeyValuePair<string,DBTable> tablename in Utilities.DictParsedTables)
            {
                DBTable dBTable = tablename.Value;
                
                // Calculate the scores for each columns as per the algo defined in Column Class and store this total for calculating the total Score of this table.
                foreach (KeyValuePair<long, Column> ColmItem in dBTable.DictColumns)
                {
                    CombinedColumnScore += ColmItem.Value.CalculateScore();
                }

                // Calculate the table score based on the combined column Score agaisnt the occurence of table on the frequency of Select, upate and insert queries.
                // We increment the score for index candidacy for Select Occurences and decrement them for Insert and update occurences since that deters us due to Write performance tradeoff.
                foreach (KeyValuePair<string, long> occurence in dBTable.DictOccurencesStmts)
                {
                    if(ESqlStatementType.sstselect.ToString() == occurence.Key)
                    {
                        //Utilities.TableScore[tablename.Key] += occurence.Value * CombinedColumnScore;
                        dBTable.Score += occurence.Value * CombinedColumnScore;
                    }
                    else if (ESqlStatementType.sstupdate.ToString() == occurence.Key)
                    {
                        //Utilities.TableScore[tablename.Key] -= occurence.Value * CombinedColumnScore;
                        dBTable.Score -= occurence.Value * CombinedColumnScore;
                    }
                    else if (ESqlStatementType.sstinsert.ToString() == occurence.Key)
                    {
                        // Utilities.TableScore[tablename.Key] -= occurence.Value * CombinedColumnScore;
                        dBTable.Score -= occurence.Value * CombinedColumnScore;
                    }

                }

            } 

        }

        /// <summary>
        /// This method scores the columns of a table according to the number of occurences, type of operator, clause in which the column was used, etc.
        /// This is part of a scoring algorithm which will decide which columns are good candidates for an index on a particular table.
        /// </summary>
        public void ScoreColumn()
        {

            // run the Scoring for all the tables spotted in the workload.
            foreach (KeyValuePair<string, DBTable> tablename in Utilities.DictParsedTables)
            {


            }
        }

    }
}
