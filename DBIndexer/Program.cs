using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTUtilities;

namespace DBIndexer
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("We are taking the parameters in reality. This statement is just for test purposes. \n Type Yes to start the execution or help to get item options.");

                string input = Console.ReadLine();
                if (input == "help")
                {
                    // Print the sample way to access this tool with all the options specified.
                    Console.WriteLine("The Command for the tool can be written as follows:\n" +
                        @"DBIndexer -S[Servername]\[InstanceName] -D[DatabaseName] -U[Username] -P[Password] -I[IntegratedSecutiryMode]");
                    Console.ReadKey();
                }
                else
                {
                    //string[] parameters = input.Split(' ');

                    // Calls MTUtilities.Utilities.PerformCheckUp for checking.
                    MTUtilities.Utilities utilities = new Utilities();
                    bool CheckUpSucessful = utilities.PerformChecksUp("", "", "", "", "");

                    if(CheckUpSucessful)
                    {
                        // Start parsing and gathering the statistics of the workload:
                        ColumnTableStmt.ParseWorkload("AdventureWorks2014.sql");
                        
                        Console.WriteLine("\n{0,10} {1,30} {2,30} {3,20} {4,20} {5,20} {6,20} {7,20} {8,20}", "TableName", "ColumnName", "TotalOccurences", "GroupByClauseOccurences", "WhereClauseOccurences", "HavingClauseOccurences", "ProjectClauseOccurences", "JoinClauseOccurences", "OrderByClauseOccurences");

                        // Iterate over each table to print the collected values of tables and columns and their stats.
                        foreach (DBTable table in Utilities.DictParsedTables.Select(tab=>tab.Value).ToList<DBTable>())
                        {
                            List<Column> lstColumns = table.DictColumns.Select(col => col.Value).ToList<Column>();
                            foreach (Column col in lstColumns)
                            {
                                Console.WriteLine("{0,10} {1,30} {2,30} {3,20} {4,20} {5,20} {6,20} {7,20} {8,20}", table.name, col.Name, col.TotalNumOfOccurrences, col.GroupByOccurences, col.WhereOccurences, col.HavingOccurences, col.ProjectOccurences, col.UsedForJoinOccurrences, col.OrderByOccurences);
                            }
                        }

                        // Execute the workload queries one by one and benchmark them
                        // Then get all the info about tables and columns from the database.
                        // Also get the information about the
                    
                    }
                    else
                    {
                        Console.WriteLine("Please check the provided credentials again and make sure you have access to the database using those credentials.");
                    }
                }
                Console.ReadKey();
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
            }

            
            
        }

        /// <summary>
        /// 
        /// </summary>
        public void UltityCalls()
        {

        }
    }
}
