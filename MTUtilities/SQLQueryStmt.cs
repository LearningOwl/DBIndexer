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
        public Guid QueryId = Guid.Empty;           // Id of the query
        public string QueryText = string.Empty;     // The entire SQL query Text

        // Query Occurences and timing
        public long NumOfOccurences;                                        // Total of the occurences of a query in the workload
        public long OriginalQueryExecutionTime = 0;         // The benchmark execution time of the query in the first workload execution regards to CPUTime
        public long IndexedQueryExecutionTime = 0;          // The benchmark execution time of the query after the supposedly optimized indexed execution regards to CPUTime
        public string ExecutedFrom;
        public string ExecutedTill;

        // Query execution metadata
        public long PhysicalReads_Before;                                  // Total Physical Reads done while executing the query before index suggestions
        public long LogicalReads_Before;                                   // Total Logical Reads done while executing the query before index suggestions
        public long TotalIOCost_Before;                                    // Total IO Cost of executing the query before index suggestions
        public long PhysicalReads_After;                                  // Total Physical Reads done while executing the query after index suggestions
        public long LogicalReads_After;                                   // Total Logical Reads done while executing the query after index suggestions
        public long LogicalWrites_Before;                                  // Total Logical Writes done while executing the query before index suggestions
        public long LogicalWrites_After;                                   // Total Logical Writes done while executing the query after index suggestions
        public long TotalIOCost_After;                                    // Total IO Cost of executing the query before index suggestions
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


        }

        /// <summary>
        /// Executes the query agaisnt the database and benchmarks the execution parameters for each query.
        /// </summary>
        public static void BenchmarkWorload(string ServerName, string DBFilepath, string UserName, string Password, string DBName, Boolean IsBeforeSuggestions)
        {
            string ConnectionString = "";

            if (UserName == "" && Password == "")
            {
                //Connection string example with Windows Authentication mode or Integrated Security mode.
                ConnectionString = @"Data Source=" + ServerName + @";
                          Initial Catalog=" + DBName + @";
                          Asynchronous Processing=True;
                          Integrated Security=True;
                          Connect Timeout=30";
            }
            else
            {
                // Connection string example with UserName and Password:
                ConnectionString = @"Data Source=" + ServerName + @";
                          Initial Catalog=" + DBName + @";
                          User Id= " + UserName + @" ;
                          Password= " + Password + @";
                          Asynchronous Processing=True;
                          Connect Timeout=30";

            }
            // Create and open the connection in a using block. This
            // ensures that all resources will be closed and disposed
            // when the code exits.
            using (SqlConnection connection =
                new SqlConnection(ConnectionString))
            {

                string querytext = "";
                SqlCommand cmd = new SqlCommand(querytext, connection);
                connection.Open();

                if (IsBeforeSuggestions)
                {
                    // Stage the SQLServer for execution.
                    querytext = @"ALTER DATABASE AdventureWorks2016 SET QUERY_STORE CLEAR;";
                    cmd.CommandText = querytext;
                    cmd.ExecuteNonQuery();

                }

                querytext = @"ALTER DATABASE AdventureWorks2016 SET QUERY_STORE(QUERY_CAPTURE_MODE = All);";
                cmd.CommandText = querytext;
                cmd.ExecuteNonQuery();

                // Create Header for the data to be printed after benchmarking.
                var consoletable = new ConsoleTable("Query Id", "Query Text [Truncated]", "Rows Returned", "Successful Execution");


                foreach (KeyValuePair<Guid, SQLQueryStmt> qry in Utilities.DictWorkloadQueries)
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(qry.Value.QueryText.ToString(), connection);
                    DataSet dataSet = new DataSet();
                    
                    // Open the connection in a try/catch block. 
                    try
                    {
                        adapter.Fill(dataSet);
                        qry.Value.RowsAffected = dataSet.Tables[0].Rows.Count;
                        consoletable.AddRow(qry.Key, qry.Value.QueryText.Replace(System.Environment.NewLine, " ").Substring(0, 35) + "...", dataSet.Tables[0].Rows.Count, (dataSet.Tables[0].Rows.Count != 0) ? "Execution Successful" : "Execution Successful but zero rows returned.");

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(qry.Value.QueryText.ToString());
                        throw ex;
                    }

                }

                querytext = @"ALTER DATABASE AdventureWorks2016 SET QUERY_STORE(QUERY_CAPTURE_MODE = None);";
                cmd.CommandText = querytext;
                cmd.ExecuteNonQuery();

                if (IsBeforeSuggestions)
                {
                    querytext = @"select q.query_id, qt.query_sql_text, pl.plan_id, rs.count_executions,
                                        rs.first_execution_time, rs.last_execution_time,
                                         rs.min_logical_io_reads, rs.max_logical_io_reads,
                                         rs.min_physical_io_reads, rs.max_physical_io_reads,
                                         rs.min_logical_io_writes, rs.max_logical_io_writes,
                                         rs.min_cpu_time, rs.max_cpu_time
                                  from sys.query_store_query q
                                    inner join sys.query_store_query_text qt
                                        on q.query_id = qt.query_text_id
                                    inner join sys.query_store_plan pl
                                        on qt.query_text_id = pl.query_id
                                    inner join sys.query_store_runtime_stats rs
                                        on pl.plan_id = rs.plan_id";
                    
                }
                else
                {

                    querytext = @"select * from 
                                 (select q.query_id, qt.query_sql_text, pl.plan_id, rs.count_executions,
                                        rs.first_execution_time, rs.last_execution_time,
                                         rs.min_logical_io_reads, rs.max_logical_io_reads,
                                         rs.min_physical_io_reads, rs.max_physical_io_reads,
                                         rs.min_logical_io_writes, rs.max_logical_io_writes,
                                         rs.min_cpu_time, rs.max_cpu_time,
                                         Row_Number() OVER (PARTITION BY q.query_id ORDER BY pl.plan_id desc , rs.last_execution_time DESC) as RN from
                                  sys.query_store_query q
                                    inner join sys.query_store_query_text qt
                                        on q.query_id = qt.query_text_id
                                    inner join sys.query_store_plan pl
                                        on qt.query_text_id = pl.query_id
                                    inner join sys.query_store_runtime_stats rs
                                        on pl.plan_id = rs.plan_id
                                  ) as A
                                  where A.RN = 1";
                }
                cmd.CommandText = querytext;
                // Fetch the query execution details
                SqlDataReader dataReader = cmd.ExecuteReader();

                // Implement the recording of query execution details.
                while (dataReader.Read())
                {
                    string qrystring = dataReader[1].ToString();
                    qrystring += ";";
                    // Find the stmt that we recorded the details for.
                    KeyValuePair<Guid,SQLQueryStmt> stmt = Utilities.DictWorkloadQueries.Where(x => x.Value.QueryText.Replace(System.Environment.NewLine, " ") == qrystring.Replace(System.Environment.NewLine, " ")).FirstOrDefault();

                    if (stmt.Value != null)
                    {
                        if (IsBeforeSuggestions)
                        {
                            stmt.Value.PhysicalReads_Before = Convert.ToInt64(dataReader[9]);
                            stmt.Value.LogicalReads_Before = Convert.ToInt64(dataReader[7]);
                            stmt.Value.LogicalWrites_Before = Convert.ToInt64(dataReader[11]);
                            stmt.Value.OriginalQueryExecutionTime = Convert.ToInt64(dataReader[13]);
                            stmt.Value.ExecutedFrom = dataReader[5].ToString();
                            stmt.Value.ExecutedTill = dataReader[6].ToString();
                            stmt.Value.TotalIOCost_Before = stmt.Value.LogicalReads_Before + stmt.Value.LogicalWrites_Before + stmt.Value.PhysicalReads_Before;
                        }
                        else
                        {
                            stmt.Value.PhysicalReads_After = Convert.ToInt64(dataReader[8]);
                            stmt.Value.LogicalReads_After = Convert.ToInt64(dataReader[6]);
                            stmt.Value.LogicalWrites_After = Convert.ToInt64(dataReader[10]);
                            stmt.Value.IndexedQueryExecutionTime = Convert.ToInt64(dataReader[12]);
                            stmt.Value.ExecutedFrom = dataReader[5].ToString();
                            stmt.Value.ExecutedTill = dataReader[6].ToString();
                            stmt.Value.TotalIOCost_After = stmt.Value.LogicalReads_After + stmt.Value.LogicalWrites_After + stmt.Value.PhysicalReads_After;

                        }
                        Utilities.DictWorkloadQueries[stmt.Key] = stmt.Value;
                    }
                }
                



                connection.Close();

                consoletable.Write(Format.MarkDown);
                Console.WriteLine();



            }



        }
    }
}
