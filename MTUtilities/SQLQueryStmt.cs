using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace MTUtilities
{
    public enum OperationType
    {
        TableScan = 1,
        IndexScan = 2,
        IndexSeek = 3,
        Unknown = 4

    }


    public class SQLQueryStmt
    {
        // Query text specific parameters
        public Guid QueryId = Guid.Empty;                        // Id of the query
        public string QueryText = string.Empty;     // The entire SQL query Text

        // Query Occurences and timing
        public long NumOfOccurences;                                        // Total of the occurences of a query in the workload
        public TimeSpan OriginalQueryExecutionTime = TimeSpan.Zero;         // The benchmark execution time of the query in the first workload execution
        public TimeSpan IndexedQueryExecutionTime = TimeSpan.Zero;          // The benchmark execution time of the query after the supposedly optimized indexed execution

        // Query execution metadata
        public long PhysicalReads;                                  // Total Physical Reads done while executing the query
        public long LogicalReads;                                   // Total Logical Reads done while executing the query
        public long TotalIOCost;                                    // Total IO Cost of executing the query
        public OperationType operation = OperationType.Unknown;     // Type of operation performed on the table/index to retrieve the requested records
        public int RowsAffected;                                    // Total rows affected when the query was executed.


        /// <summary>
        /// Prints the Occurence Stats collected from the workload for each query.
        /// </summary>
        public static void PrintQueryOccurenceStats()
        {

            // Print output Headers
            var consoletable = new ConsoleTable("Query Text", "Total Occurences", "Query Identifier");

            // Print declaration statement
            Console.WriteLine("######################  Printing Query Occurence Stats from the workload execution  ####################");

            foreach (KeyValuePair<Guid, SQLQueryStmt> qry in Utilities.DictWorkloadQueries)
            {
                consoletable.AddRow(qry.Value.QueryText.Replace(System.Environment.NewLine, " ").Substring(0, 35) + "...", qry.Value.NumOfOccurences, qry.Key);
                //consoletable.AddRow(qry.Value.QueryText.Replace(System.Environment.NewLine,""), qry.Value.NumOfOccurences, qry.Key);

            }

            consoletable.Write(Format.MarkDown);
            Console.WriteLine();
            Console.ReadLine();

            
        }

        /// <summary>
        /// Executes the query agaisnt the database and benchmarks the execution parameters for each query.
        /// </summary>
        public static void BenchmarkWorload(string ServerName, string DBFilepath, string UserName, string Password, string DBName)
        {
            //Connection string example with Windows Authentication mode or Integrated Security mode.
            string ConnectionString = @"Data Source=DESKTOP-R4E3L7J\LEARNINGOWL;
                          Initial Catalog=AdventureWorks2016;
                          Asynchronous Processing=True;
                          Integrated Security=True;
                          Connect Timeout=30";

            // Connection string example with UserName and Password:
            //string ConnectionString = @"Data Source=DESKTOP-R4E3L7J\LEARNINGOWL;
            //              Initial Catalog=AdventureWorks2016;
            //              User Id=sa;
            //              Password=Jayant123*;
            //              Asynchronous Processing=True;
            //              Connect Timeout=30";

            
            // Create and open the connection in a using block. This
            // ensures that all resources will be closed and disposed
            // when the code exits.
            using (SqlConnection connection =
                new SqlConnection(ConnectionString))
            {
                // Create Header for the data to be printed after benchmarking.
                var consoletable = new ConsoleTable("Query Id", "Query Text [Truncated]", "Rows Returned", "Successful Execution");

                connection.Open();
                foreach (KeyValuePair<Guid, SQLQueryStmt> qry in Utilities.DictWorkloadQueries)
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(qry.Value.QueryText.ToString(), connection);
                    DataSet dataSet = new DataSet();

                    // Open the connection in a try/catch block. 
                    try
                    {
                        adapter.Fill(dataSet);

                        consoletable.AddRow(qry.Key, qry.Value.QueryText.Replace(System.Environment.NewLine, " ").Substring(0, 35) + "...", dataSet.Tables[0].Rows.Count, (dataSet.Tables[0].Rows.Count != 0) ? "Execution Successful": "Execution Failed");
                        
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    
                }
 
                connection.Close();

                consoletable.Write(Format.MarkDown);
                Console.WriteLine();

                

            }



        }
    }
}
