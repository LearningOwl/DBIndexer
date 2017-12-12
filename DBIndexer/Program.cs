using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTUtilities;
using System.IO;
using XsPDF.Charting;
using XsPDF.Pdf;
using XsPDF.Fonts;

namespace DBIndexer
{
    public class Program
    {
        static void Main(string[] args)
        {
            string ServerName = string.Empty;
            string InstanceName = string.Empty;
            string UserName = string.Empty;
            string Password = string.Empty;
            string DBFilePath = string.Empty;
            string DBName = string.Empty;
            Boolean IntegratedMode = false;

            try
            {
                //Console.WriteLine("We are taking the parameters in reality. This statement is just for test purposes. \n Type Yes to start the execution or help to get item options.");

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
                    string[] parameters = input.Split(' ');
                    foreach (string parmetr in parameters)
                    {
                        if (parmetr.StartsWith("-S"))
                        {
                            ServerName = parmetr.Substring(2, parmetr.Length - 2);
                        }
                        else if (parmetr.StartsWith("-D"))
                        {
                            DBName = parmetr.Substring(2, parmetr.Length - 2);
                        }
                        else if (parmetr.StartsWith("-U"))
                        {
                            UserName = parmetr.Substring(2, parmetr.Length - 2);
                        }
                        else if (parmetr.StartsWith("-P"))
                        {
                            Password = parmetr.Substring(2, parmetr.Length - 2);
                        }
                        else if (parmetr.StartsWith("-I"))
                        {
                            IntegratedMode = true;
                        }

                    }

                    if (ServerName == String.Empty || DBName == string.Empty || (IntegratedMode = false && ( UserName == string.Empty || Password == string.Empty)) ) 
                    {
                        throw new InvalidDataException("Atleast one of the input parameters was not provided. Please check that all the values are entered for required parameters. If not sure, type 'help' to get the required details. ");
                    }
                    // DBIndexer -SDESKTOP-R4E3L7J\LEARNINGOWL -DAdventureWorks2016 -Usa -PJayant123* -ITrue
                    // @"DESKTOP-R4E3L7J\LEARNINGOWL", "", @"sa", @"Jayant123*", @"AdventureWorks2016"

                    // Checkup ifthe credentials provided are correct and have the proper privileges for this database.
                    MTUtilities.Utilities utilities = new Utilities();
                    bool CheckUpSucessful = utilities.PerformChecksUp(ServerName, DBFilePath, UserName, Password, DBName);

                    
                    if (CheckUpSucessful)           // Start parsing and gathering the statistics of the workload:
                    {
                        // Get all the SQL files of the workload to be parsed.
                        string[] filenames = Directory.GetFiles(@"C:\Users\Learning_Owl\source\repos\DBIndexer\DBIndexer\bin\Debug", "Adve*.sql", SearchOption.TopDirectoryOnly);

                        // Parse each file.
                        foreach (string filename in filenames)
                        {
                            ColumnTableStmt.ParseWorkload(filename);
                        }


                        // Print Formatted Console Output:

                        // Print output Headers
                        var consoletable = new ConsoleTable("TableName", "ColumnName", "Total", "GroupBy", "Where", "Having", "Project", "Join", "OrderBy", "Unknown", "WhereOperators");

                        // Iterate over each table to print the collected values of tables and columns and their stats.
                        foreach (DBTable table in Utilities.DictParsedTables.Select(tab => tab.Value).ToList<DBTable>())
                        {
                            List<Column> lstColumns = table.DictColumns.Select(col => col.Value).ToList<Column>();
                            foreach (Column col in lstColumns)
                            {
                                string CommaSeparatedOperator = string.Empty;

                                foreach (KeyValuePair<string,long> item in col.WhereComparisonOperators)
                                {
                                    if (CommaSeparatedOperator == string.Empty)
                                        CommaSeparatedOperator = item.Key + " : " + item.Value;
                                    else
                                        CommaSeparatedOperator = CommaSeparatedOperator + " | " + item.Key + " : " + item.Value;

                                }
                                consoletable.AddRow(table.name, col.Name, col.TotalNumOfOccurrences, col.GroupByOccurences, col.WhereOccurences, col.HavingOccurences, col.ProjectOccurences, col.UsedForJoinOccurrences, col.OrderByOccurences, col.UnknownOccurences, CommaSeparatedOperator);
                            }
                        }

                        Console.WriteLine("######################  Printing Column stats as per the Clause occurence in Query. ####################");
                        consoletable.Write(Format.MarkDown);
                        Console.WriteLine();

                        
                        // Printing Query Text and its occurences as a whole query
                        SQLQueryStmt.PrintQueryOccurenceStats();


                        // Execute the workload queries one by one and benchmark them 
                        SQLQueryStmt.BenchmarkWorload(ServerName, DBFilePath, UserName, Password, DBName, true);

                        // Start ranking now based on the collected data.
                        ScoreAnalysis ScoreMaster = new ScoreAnalysis(); ;
                        ScoreMaster.ScoreTables();

                        // Create a sheet like structure from data members for futher analysis. 
                        List<ScoreAnalysis> ScoreBoard = new List<ScoreAnalysis>();
                        
                        // Get all the calculated scores in the ScoreBaord list.
                        foreach (KeyValuePair<string, DBTable> table in Utilities.DictParsedTables)
                        {
                            foreach (KeyValuePair<long,Column> colitem in table.Value.DictColumns)
                            {
                                ScoreMaster = new ScoreAnalysis();

                                // Update the table data for each column.
                                ScoreMaster.TableName =  table.Value.name;
                                ScoreMaster.TableScore = table.Value.Score;

                                // update the column scoring data for each column.
                                ScoreMaster.ColumnName = colitem.Value.Name;
                                ScoreMaster.ColumnScore = colitem.Value.Score;
                                ScoreMaster.ColumnHashScore = colitem.Value.HashScore;
                                ScoreMaster.ColumnBinaryScore = colitem.Value.BinaryScore;

                                // Add the final score for each column to the Scoreboard.
                                ScoreBoard.Add(ScoreMaster);
                            }
                        }
                        
                        // Sort the List by Scores:
                        //List<ScoreAnalysis> FinalScoreBoard = ScoreBoard.OrderByDescending(x => x.TableScore).ThenByDescending(x => x.ColumnScore).ToList<ScoreAnalysis>();

                        ConsoleTable ScoreBoardOutput = new ConsoleTable("Table_Name", "Table_Score", "Column_Name", "Column_Score", "HashScore", "Binary Score");
                        ScoreBoard = ScoreBoard.OrderByDescending(x => x.TableScore).ThenByDescending(x => x.ColumnScore).ToList<ScoreAnalysis>();
                        
                        
                        foreach (ScoreAnalysis Score in ScoreBoard)
                        {
                            ScoreBoardOutput.AddRow(Score.TableName, Score.TableScore, Score.ColumnName, Score.ColumnScore, Score.ColumnHashScore, Score.ColumnBinaryScore);
                        }

                        // Print the output
                        Console.WriteLine("######################  Printing final scores for each table and column by the order of highest scores first. ####################");
                        
                        ScoreBoardOutput.Write(Format.MarkDown);
                        //Utilities.ExportToPDF(ScoreBoardOutput,"TableAndColumnScores");
                        Console.WriteLine();



                        // Start deciding on the index options for each table based on the  column heuristics.
                        // Algo
                        // Check for each column's hash/binary choice based on their respective scores.
                        // check for combination of columns marked as hash/ binary to be implemented as index pairs based on the highest scored column.
                        // suggest the index with highest score along with the hash and binary combinations as final choices.

                        Dictionary<string, List<string>> HashChoices = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> BinaryChoices = new Dictionary<string, List<string>>();
 
                        // Compare Hash and Binary choices
                        foreach (KeyValuePair<string,DBTable> table in Utilities.DictParsedTables)
                        {
                            List<string> HashColumns = new List<string>();
                            List<string> BinaryColumns = new List<string>();

                            List<ScoreAnalysis> ScoredTable = ScoreBoard.Where(x => x.TableName == table.Value.name).ToList<ScoreAnalysis>();

                            long TopScore = ScoredTable.Max(x => x.ColumnScore);
                            ScoreAnalysis TopScoreColName = ScoredTable.Where(x => x.ColumnScore == TopScore).Select(x=>x).FirstOrDefault();

                            

                            if (TopScoreColName.ColumnHashScore.CompareTo(TopScoreColName.ColumnBinaryScore) > 0) // Hash Score is more
                            {
                                // Check if hash is more by 8% or more.
                                if ((TopScoreColName.ColumnHashScore * 92 / 100) > TopScoreColName.ColumnBinaryScore)
                                {
                                    HashColumns.Add(TopScoreColName.ColumnName);
                                    HashChoices[TopScoreColName.TableName] = HashColumns;
                                }
                            }
                            else if (TopScoreColName.ColumnHashScore.CompareTo(TopScoreColName.ColumnBinaryScore) == 0) // Hash and Binary scores are equal
                            {
                                // Add an entry for Hash
                                HashColumns.Add(TopScoreColName.ColumnName);
                                HashChoices[TopScoreColName.TableName] = HashColumns;
                                
                                // Add an entry for Binary
                                BinaryColumns.Add(TopScoreColName.ColumnName);
                                BinaryChoices[TopScoreColName.TableName] = BinaryColumns;
                            }
                            else if (TopScoreColName.ColumnHashScore.CompareTo(TopScoreColName.ColumnBinaryScore) < 0) // Binary score is more.
                            {
                                BinaryColumns.Add(TopScoreColName.ColumnName);
                                BinaryChoices[TopScoreColName.TableName] = BinaryColumns;
                            }

                            foreach (ScoreAnalysis item in ScoredTable)
                            {
                                if (item.ColumnName != TopScoreColName.ColumnName) // For all columns other than the top scoring column
                                {
                                    if (item.ColumnHashScore == TopScoreColName.ColumnHashScore && TopScoreColName.ColumnHashScore != 0) // if hash score is equal to max hash score
                                    {
                                        if (HashChoices.Count == 0) // Case when the binary score was more than hash for TopScoredColumn
                                        {
                                            string CompositeHashcol = TopScoreColName.ColumnName + ',' + item.ColumnName;
                                            HashColumns.Add(CompositeHashcol);
                                            HashChoices[TopScoreColName.TableName] = HashColumns;
                                        }
                                        else // Case when the top scored hash column is already added to the choices
                                        {
                                            string CompositeHashcol = HashColumns.Where(x => x.StartsWith(TopScoreColName.ColumnName)).FirstOrDefault();
                                            if (CompositeHashcol != "" && CompositeHashcol != string.Empty)
                                            {
                                                HashColumns.Remove(CompositeHashcol);
                                                CompositeHashcol = CompositeHashcol + ',' + item.ColumnName;
                                                HashColumns.Add(CompositeHashcol);
                                                HashChoices[TopScoreColName.TableName] = HashColumns;
                                            }
                                        }
                                        
                                    }
                                    else if (((TopScoreColName.ColumnHashScore * 92) / 100) <= item.ColumnHashScore && TopScoreColName.ColumnHashScore != 0) // if hash score is close enough to be together as a composite hash index
                                    {
                                        HashColumns.Add(item.ColumnName);
                                    }
                                    else if ( (item.ColumnBinaryScore == TopScoreColName.ColumnBinaryScore && TopScoreColName.ColumnBinaryScore != 0) 
                                        || (((TopScoreColName.ColumnBinaryScore * 75) / 100) <= item.ColumnBinaryScore) )  // if the binary score is equal to the max binary score
                                    {
                                        if (BinaryChoices.Count == 0) // Case when the hash score was more than Binary for TopScoredColumn
                                        {
                                            string CompositeBinarycol = TopScoreColName.ColumnName + ',' + item.ColumnName;
                                            BinaryColumns.Add(CompositeBinarycol);
                                            BinaryChoices[TopScoreColName.TableName] = BinaryColumns;
                                        }
                                        else // Case when the top scored binary column is already added to the choices
                                        {
                                            string CompositeBinarycol = BinaryColumns.Where(x => x.StartsWith(TopScoreColName.ColumnName)).FirstOrDefault();
                                            if (CompositeBinarycol != "" && CompositeBinarycol != string.Empty)
                                            {
                                                BinaryColumns.Remove(CompositeBinarycol);
                                                CompositeBinarycol = CompositeBinarycol + ',' + item.ColumnName;
                                                BinaryColumns.Add(CompositeBinarycol);
                                                BinaryChoices[TopScoreColName.TableName] = BinaryColumns;
                                            }
                                        }
                                    }
                                    
                                }


                            }
                                    
                        }

                        // Create the output table
                        ConsoleTable ct = new ConsoleTable("Table_Name", "Index_Columns", "Index_Type");
                        List<OutputUtil> Output = new List<OutputUtil>();

                        // Add the final Hash choices to the output.
                        foreach (KeyValuePair<string,List<string>> choice in HashChoices)
                        {
                            foreach (string columns in choice.Value)
                            {
                                OutputUtil ot = new OutputUtil();
                                ot.Table_Name = choice.Key;
                                ot.Index_Columns = columns;
                                ot.Index_type = "Hash";
                                Output.Add(ot);
                                
                            }
                        }

                        // Add the final Binary choices to the output.
                        foreach (KeyValuePair<string, List<string>> choice in BinaryChoices)
                        {
                            foreach (string columns in choice.Value)
                            {
                                OutputUtil ot = new OutputUtil();
                                ot.Table_Name = choice.Key;
                                ot.Index_Columns = columns;
                                ot.Index_type = "Binary";
                                Output.Add(ot);
                            }
                        }

                        // Sort the output by the tablenames.
                        Output = Output.OrderBy(table => table.Table_Name).ToList<OutputUtil>();

                        //Store all the index creation queries to be executed in this array.
                        List<string> IndexCreationQueries = new List<string>();

                        
                        // Add the final output after sorting table wise to the Console Printer.
                        foreach (OutputUtil item in Output)
                        {
                            ct.AddRow(item.Table_Name, item.Index_Columns, item.Index_type);

                            
                            // Create the indexes as suggested above.
                            string[] columns = item.Index_Columns.Split(',');
                            if (columns.Count() > 1)
                            {
                                string CreateText = "CREATE INDEX [Idx_" + item.Table_Name + "_" + item.Index_Columns.Replace(',','_') + "] ON [" + Utilities.DictTablesCatalog.First(x => x.Key == item.Table_Name).Value.Table_Schema_Name + "].[" + item.Table_Name + "] ([" + columns[0] + "] ASC)";
                                for (int i = 1; i < columns.Length; i++)
                                {
                                    if (i == 1)
                                    {
                                        CreateText += "INCLUDE ([" + columns[i] + "]";
                                    }
                                    else
                                    {
                                        CreateText += ", [" + columns[i] + "]";
                                    }
                                }
                                CreateText += ");";
                                
                                IndexCreationQueries.Add(CreateText);

                            }
                            else
                            {
                                IndexCreationQueries.Add("CREATE INDEX [Idx_"+ item.Table_Name + "_" + item.Index_Columns + "] ON [" + Utilities.DictTablesCatalog.First(x => x.Key == item.Table_Name).Value.Table_Schema_Name + "].[" + item.Table_Name + "] ([" + item.Index_Columns + "]);");
                            }
                            
                            

                        }
                        ct.Write(Format.MarkDown);
                        Console.WriteLine();

                        // Create the indexes as per the suggestions
                        Utilities.CreateIndexSuggestions(IndexCreationQueries, ServerName, DBFilePath, UserName, Password, DBName, true);

                        #region Benchmark after Suggestions and capture the difference
                        // Benchmark
                        SQLQueryStmt.BenchmarkWorload(ServerName, DBFilePath, UserName, Password, DBName, false);

                        List<string> DropIndexes = new List<string>();
                        foreach (string item in IndexCreationQueries)
                        {
                            DropIndexes.Add(item.Replace("CREATE", "DROP").Substring(0, item.IndexOf('(') - 1).Replace('(', ';'));
                        }
                        // Drop the indexes created for benchmarking
                        Utilities.CreateIndexSuggestions(DropIndexes, ServerName, DBFilePath, UserName, Password, DBName, false);

                        // Print out the differences:
                        ConsoleTable ResultsConsole = new ConsoleTable("QueryText","Occurences","IOCost_Before", "IOCost_After", "Execution_Time_Before", "Execution_Time_After", "Status", "Change Percentage");
                        foreach (KeyValuePair<Guid, SQLQueryStmt> qry in Utilities.DictWorkloadQueries)
                        {
                            ResultsConsole.AddRow(
                                qry.Value.QueryText.Replace(System.Environment.NewLine, " ").Substring(0, 35) + "...",
                                qry.Value.NumOfOccurences,
                                qry.Value.TotalIOCost_Before * qry.Value.NumOfOccurences
                               , qry.Value.TotalIOCost_After * qry.Value.NumOfOccurences
                               , qry.Value.OriginalQueryExecutionTime * qry.Value.NumOfOccurences
                               , qry.Value.IndexedQueryExecutionTime * qry.Value.NumOfOccurences
                               , (((qry.Value.LogicalReads_After + qry.Value.PhysicalReads_After) < (qry.Value.LogicalReads_Before + qry.Value.PhysicalReads_Before)) ? "Improved" : (((qry.Value.LogicalReads_After + qry.Value.PhysicalReads_After) > (qry.Value.LogicalReads_Before + qry.Value.PhysicalReads_Before)) ? "Degraded" : "NoChange"))
                               , (qry.Value.TotalIOCost_Before == 0)? 0 : ( ( (double) (Math.Abs( (double)qry.Value.TotalIOCost_Before - (double)qry.Value.TotalIOCost_After) / (double)qry.Value.TotalIOCost_Before) * 100.00 )));

                        }
                        ResultsConsole.Write(Format.MarkDown);
                        Console.WriteLine();
                        #endregion

                        // check if statistics need to be updated.
                        // Give suggestions about which queries can be written in a better way.


                        Console.ReadKey();
                        //Console.WriteLine("Do you want the analysis in a PDF ?");
                        if (Console.ReadLine() == "Yes")
                        {
                            // write the code to output it to the pdf.
                            string filepath = "";

                            #region PDF Export
                            //Utilities.ExportToPDF(ct,"PrintAllGraphs");

                            #endregion  

                            Console.WriteLine("PDF created at location : {0}", filepath);
                            Console.WriteLine("Processing Complete.");
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.WriteLine("Processing Complete.");
                            Console.ReadKey();
                        }

                        

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

    }

    public class OutputUtil
    {
        public string Table_Name, Index_Columns, Index_type;
    }
}
