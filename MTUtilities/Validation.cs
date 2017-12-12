using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace MTUtilities
{
    public class Validation
    {
        /// <summary>
        /// Checks the valididty of input parameters by connecting to the database using given credentials and also validating permissions of the user.
        /// </summary>
        /// <param name="DBFilepath">the access path of the actual Database file.</param>
        /// <param name="UserName">login username for the database.</param>
        /// <param name="Password">password for the the database login.</param>
        /// <param name="DBName">name of the database to be connected.</param>
        /// <returns>Boolean</returns>
        public Boolean CheckInputParameters(string ServerName,string DBFilepath, string UserName, string Password, string DBName)
        {
            bool GreenLights = false;

            /*
             * This implementation is pending
             * check the connection here and send true if all works fine. Check if all works fine by querying if the database exists. 
             * check the user permission of the login to be of DBO or Sysadmin levels.
             * 
             * The following code does a test run of the access to the database after the above mentioned initial checks are done.
            */

            /*
             * The following code does a test run of the access to the database after the above mentioned initial checks are done.
             */
            // AttachDbFilename=C:\Program Files\Microsoft SQL Server\MSSQL13.LEARNINGOWL\MSSQL\DATA\AdventureWorks2016_Data.mdf;

            string ConnectionString = "";

            if (UserName == "" && Password == "")
            {
                //Connection string example with Windows Authentication mode or Integrated Security mode.
                ConnectionString = @"Data Source= " + ServerName + @";
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

            string queryString = "SELECT TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = @FilterCondition";  // WHERE ColumnName > @FilterCondition
            // Create and open the connection in a using block. This
            // ensures that all resources will be closed and disposed
            // when the code exits.
            using (SqlConnection connection =
                new SqlConnection(ConnectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(queryString, connection);
                string paramValue = "BASE TABLE";  // Just an example. Could be anything.
                command.Parameters.AddWithValue("@FilterCondition", paramValue);

                // Open the connection in a try/catch block. 
                // Create and execute the DataReader, writing the result
                // set to the console window.
                try
                {


                    // Print output Headers
                    var consoletable = new ConsoleTable("Database", "SchemaName", "TableName");

                    // Print declaration statement
                    Console.WriteLine("######################  Printing schemas and tables that are accessible to provided user credential  ####################");

                    connection.Open();

                    SqlDataReader OutputReader = command.ExecuteReader();
                    while (OutputReader.Read())
                    {
                        consoletable.AddRow(OutputReader[0], OutputReader[1], OutputReader[2]);
                        DBTable tab = new DBTable();
                        tab.Table_Schema_Name = OutputReader[1].ToString();
                        tab.name = OutputReader[2].ToString();

                        Utilities.DictTablesCatalog[tab.name] = tab;
                        
                    }

                    OutputReader.Close();

                    consoletable.Write(Format.MarkDown);
                    Utilities.ExportToPDF(consoletable, "PrintSchema");
                    Console.WriteLine();

                    GreenLights = true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                
            }
            

            return GreenLights;
        }

        /// <summary>
        /// Sets the static variables with values that we will require througout the analysis such as TableCount, IndexCount, Tablewise Index list, 
        /// When was the last time Statistics updated, which type of indexes are created, what is the size of the table, size of the index,
        /// </summary>
        /// <returns>Boolean</returns>
        public Boolean RunTableDiagnostic()
        {
            bool GreenLights = false;

            /*
             * check the connection here and send true if all works fine. Check if all works fine by querying if the database exists. 
             * check the user permission of the login to be of DBO or Sysadmin levels.
            */

            return GreenLights;
        }
    }
}
