using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gudusoft.gsqlparser;
using gudusoft.gsqlparser.nodes;
using System.Data.SqlClient;

namespace MTUtilities
{
    public class ColumnTableStmt
    {
        // This is a global variable to return the value of the where clause condition to the doIt()
        public static string ExpressionStatement = string.Empty;

        //public static List<NestedStmtWhereClauses> StatementWhereClauses = new List<NestedStmtWhereClauses>();

        public static void ParseWorkload(String sqlFilename)
        {

            EDbVendor dbVendor = EDbVendor.dbvmssql;
            Console.WriteLine("\n");
            Console.WriteLine("Selected SQL dialect: " + ((dbVendor.ToString() == "dbvmssql") ? "Transact SQL for Microsoft SQL Server" : "SQL for Oracle."));
            Console.WriteLine("\n");

            TGSqlParser sqlparser = new TGSqlParser(dbVendor);
            sqlparser.sqlfilename = sqlFilename;

            int ret = sqlparser.parse();

            if (ret == 0)
            {
                Console.WriteLine("###################### Parsing Started : " + sqlFilename );

                // Print output Headers
                var consoletable = new ConsoleTable("Statement Number", "Statement Type", "Parse OK ?", "Query Text [Truncated]");

                for (int i = 0; i < sqlparser.sqlstatements.size(); i++)
                {
                    
                    ESqlStatementType sqlStatementType = sqlparser.sqlstatements.get(i).sqlstatementtype;       // Get the statementtype of the sql qwuery

                    if (sqlStatementType == ESqlStatementType.sstselect || sqlStatementType == ESqlStatementType.sstupdate) // Take the statement in dictionary only if it is a DML(SELECT/UPDATE) statement
                    {
                        // We have taken iterate statement inside if block since parsong for "Use Database", "Go", "Set variable values", etc is not useful for our applicaiton scope.
                        iterateStmt(sqlparser.sqlstatements.get(i));


                        // Create a new query object
                        SQLQueryStmt query = new SQLQueryStmt();

                        // get the query text of the sql statement from the workload
                        query.QueryText = sqlparser.sqlstatements.get(i).String;

                        // check if this statement appeared for the first time and add it to the dictionary, else increment the count of already added statement from the dictionary.
                        if (Utilities.DictWorkloadQueries.Where(qry=>qry.Value.QueryText == query.QueryText).Any())     // If the statement is occuring again.
                        {
                            Guid guid = Utilities.DictWorkloadQueries.FirstOrDefault(qry => qry.Value.QueryText == query.QueryText).Key;    // Get the queryId of the SQL already in the Dictionary.
                            Utilities.DictWorkloadQueries[guid].NumOfOccurences += 1;       // Increment the count of occurence.
                        }
                        else        // If the statement occurs for the first time.
                        {
                            query.QueryId = Guid.NewGuid();     // New key for query
                            Utilities.DictWorkloadQueries.Add(query.QueryId, query);       // Add the query to Dictionary.
                            Utilities.DictWorkloadQueries[query.QueryId].NumOfOccurences = 1;
                        }

                        consoletable.AddRow( "Parsed Statement["+i+"]", (sqlStatementType == ESqlStatementType.sstselect)?"SELECT": "OTHER" ,"Successful" ,query.QueryText.Replace(System.Environment.NewLine, " ").Substring(0, 35) + "..." );

                    }

                }
                consoletable.Write(Format.MarkDown);
                Console.WriteLine();
                Console.WriteLine("###################### "+sqlFilename+" Parsing Complete ####################");
            }
            else
            {
                Console.WriteLine(sqlparser.Errormessage);
            }
        }

        protected internal static void iterateStmt(TCustomSqlStatement stmt)
        {
            for (int i = 0; i < stmt.tables.size(); i++)
            {
                TTable table = stmt.tables.getTable(i);
                string table_name = table.Name;

                DBTable dbtable = new DBTable();
                Dictionary<long, Column> dictColumns = new Dictionary<long, Column>();

                if (Utilities.DictParsedTables.ContainsKey(table_name))
                {
                    dbtable = Utilities.DictParsedTables[table_name];
                    dictColumns = dbtable.DictColumns;
                }

                // If the column occurs first time, we will have to fetch the column id and set it for that column from the database.
                if (dbtable.Table_Object_Id == 0)
                {
                    long Table_Object_id = 0;
                    string Table_Schema_Name = "";
                    SqlConnection scon = new SqlConnection();
                    //Connection string example with Windows Authentication mode or Integrated Security mode.
                    string ConnectionString = @"Data Source=DESKTOP-R4E3L7J\LEARNINGOWL;
                          Initial Catalog=AdventureWorks2016;
                          Asynchronous Processing=True;
                          Integrated Security=True;
                          Connect Timeout=30";
                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        // select top 1 c.object_id as 'Table_Object_id', object_Name(c.object_id) as 'TableName', c.column_id, c.name as ColumnName  from sys.all_columns c where object_Name(c.object_id) = '" + column.Name + "' and c.name = '" + table_name + "'"
                        SqlCommand cmd = new SqlCommand("select top 1 c.object_id as 'Table_Object_id', OBJECT_SCHEMA_NAME(c.object_id) as 'Table_Schema_Name' from sys.all_columns c where object_name(c.object_id) = '" + table_name + "'", connection);
                        connection.Open();

                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Table_Object_id = Convert.ToInt64(reader["Table_Object_Id"]);
                            Table_Schema_Name = reader["Table_Schema_Name"].ToString();
                        }

                        connection.Close();
                    }

                    // Assign the column dictionary along with the dbtable object back to the global dbtable dictionary
                    if (dbtable.Table_Object_Id != Table_Object_id && Table_Object_id != 0)
                    {
                        dbtable.Table_Object_Id = Table_Object_id;
                        dbtable.Table_Schema_Name = Table_Schema_Name;
                    }

                }

                // Assign/Reassign name to the tableboject.
                dbtable.name = table_name;

                // Increment the occurence count of the table in the workload
                dbtable.TotalNumOfOccurrences += 1;

                // Increment the count for the type of occurence
                if (stmt.sqlstatementtype == ESqlStatementType.sstselect || stmt.sqlstatementtype == ESqlStatementType.sstmssqlselect)
                {
                    if (dbtable.DictOccurencesStmts.ContainsKey(ESqlStatementType.sstselect.ToString()))
                        dbtable.DictOccurencesStmts[ESqlStatementType.sstselect.ToString()] += 1;
                    else
                        dbtable.DictOccurencesStmts[ESqlStatementType.sstselect.ToString()] = 1;

                }
                else if (stmt.sqlstatementtype == ESqlStatementType.sstupdate || stmt.sqlstatementtype == ESqlStatementType.sstmssqlupdatetext)
                {
                    if (dbtable.DictOccurencesStmts.ContainsKey(ESqlStatementType.sstupdate.ToString()))
                        dbtable.DictOccurencesStmts[ESqlStatementType.sstupdate.ToString()] += 1;
                    else
                        dbtable.DictOccurencesStmts[ESqlStatementType.sstupdate.ToString()] = 1;

                }
                else if (stmt.sqlstatementtype == ESqlStatementType.sstinsert)
                {
                    if (dbtable.DictOccurencesStmts.ContainsKey(ESqlStatementType.sstinsert.ToString()))
                        dbtable.DictOccurencesStmts[ESqlStatementType.sstinsert.ToString()] += 1;
                    else
                        dbtable.DictOccurencesStmts[ESqlStatementType.sstinsert.ToString()] = 1;

                }


                // Loop on each of the columns in each table.
                for (int j = 0; j < table.LinkedColumns.size(); j++)
                {
                    Column column = new Column();

                    TObjectName objectName = table.LinkedColumns.getObjectName(j);

                    // check if the column is present in the column dictionary already so the same values could be updated further.
                    if (dictColumns.Where(col => col.Value.Name == objectName.ColumnNameOnly.ToLower()).Any())
                    {
                        column = dictColumns.FirstOrDefault(col => col.Value.Name == objectName.ColumnNameOnly.ToLower()).Value;
                    }
                    else
                    {
                        // Set the column name for first time occurence
                        column.Name = objectName.ColumnNameOnly.ToLower();
                    }

                    // Set the occurence irrespective of the item being found previously in dictionary or not.
                    column.TotalNumOfOccurrences += 1;

                    // Switch over the location of the column and update the occurence accrodingly.
                    switch (objectName.Location)
                    {
                        case ESqlClause.insertColumn:
                            {
                                column.InsertOccurences += 1;
                                break;
                            }
                        case ESqlClause.insertValues:
                            {
                                // This insert is being counted as Projection since this is the column value to be inserted and so it will projected to be inserted in the second table.
                                column.ProjectOccurences += 1;
                                break;
                            }
                        case ESqlClause.forUpdate:
                            {
                                column.UpdateOccurences += 1;
                                break;
                            }
                        case ESqlClause.resultColumn:
                            {
                                column.ProjectOccurences += 1;
                                break;
                            }
                        case ESqlClause.where:
                            {
                                column.WhereOccurences += 1;
                                break;
                            }
                        case ESqlClause.having:
                            {
                                column.HavingOccurences += 1;
                                break;
                            }
                        case ESqlClause.groupby:
                            {
                                column.GroupByOccurences += 1;
                                break;
                            }
                        case ESqlClause.orderby:
                            {
                                column.OrderByOccurences += 1;
                                break;
                            }
                        case ESqlClause.joinCondition:
                            {
                                column.UsedForJoinOccurrences += 1;
                                break;
                            }
                        case ESqlClause.join:
                            {
                                column.UsedForJoinOccurrences += 1;
                                break;
                            }
                        case ESqlClause.unknown:
                            {
                                column.UnknownOccurences += 1;
                                break;
                            }
                        default:
                            break;
                    }

                    // Checks the clause operator used for this column if any and maintains it.
                    if (stmt.WhereClause != null)
                    {
                        // Check if this column has been used with this operator before and update the count.
                        if (column.WhereComparisonOperators.ContainsKey(stmt.WhereClause.Condition.ComparisonType.ToString()))
                        {
                            column.WhereComparisonOperators[stmt.WhereClause.Condition.ComparisonType.ToString()] += 1;
                        }
                        else // If never used before, update the dictionary to maintain the usage of the column with this operator.
                        {
                            column.WhereComparisonOperators[stmt.WhereClause.Condition.ComparisonType.ToString()] = 1;
                        }
                    }

                    // If the column occurs first time, we will have to fetch the column id and set it for that column from the database.
                    if (column.Column_Object_Id == 0)
                    {
                        long Column_Object_id = 0;
                        SqlConnection scon = new SqlConnection();
                        //Connection string example with Windows Authentication mode or Integrated Security mode.
                        string ConnectionString = @"Data Source=DESKTOP-R4E3L7J\LEARNINGOWL;
                          Initial Catalog=AdventureWorks2016;
                          Asynchronous Processing=True;
                          Integrated Security=True;
                          Connect Timeout=30";
                        using (SqlConnection connection = new SqlConnection(ConnectionString))
                        {
                            // select top 1 c.object_id as 'Table_Object_id', object_Name(c.object_id) as 'TableName', c.column_id, c.name as ColumnName  from sys.all_columns c where object_Name(c.object_id) = '" + column.Name + "' and c.name = '" + table_name + "'"
                            SqlCommand cmd = new SqlCommand("select top 1 c.column_id as ColumnName from sys.all_columns c where Object_Schema_name(c.object_id) = '"+ dbtable.Table_Schema_Name + "' and object_name(c.object_id) =  '" + table_name + "' and c.name = '" + column.Name + "'", connection);
                            connection.Open();
                            Object obj = cmd.ExecuteScalar();
                            
                            Column_Object_id = ( (obj != null)? Convert.ToInt64(obj.ToString()) :0) ;

                            // Collect data about the Seletivity and reduction factor of the columns in consideration.
                            if (Column_Object_id != 0)
                            {
                                // Get the selectivity of this column and update it in the column properties.
                                cmd = new SqlCommand(@"select cast( 
                                                            cast(count(1) as decimal(18,2)) / 
                                                            cast(count(distinct " + column.Name + @") as decimal(18,2) ) 
                                                            as decimal(18,2)) as Selectivity 
                                                       from "+ dbtable.Table_Schema_Name + @"." + table_name, connection);

                                obj = cmd.ExecuteScalar();

                                // Selectivity = Distinct Values / Total values
                                // Reduction Factor = 1 / Selectivity
                                if (float.Parse(obj.ToString()) != 0.0)
                                {
                                    column.Selectivity = float.Parse(obj.ToString());
                                    column.ReductionFactor = 1 / column.Selectivity;
                                }
                                
                            }
                            connection.Close();

                        }

                        // Assign the column dictionary along with the dbtable object back to the global dbtable dictionary
                        if (dictColumns.FirstOrDefault(col => col.Value.Name == column.Name).Key != Column_Object_id)
                        {
                            column.Column_Object_Id = Column_Object_id;
                            dictColumns[Column_Object_id] = column;
                        }

                    }
                    else
                    {
                        dictColumns[column.Column_Object_Id] = column;
                    }

                    dbtable.DictColumns = dictColumns;

                    Utilities.DictParsedTables[table_name] = dbtable;

                }
            }


            ////Check and return the where clause if any for this statement.
            //if (stmt.WhereClause != null)
            //{
            //    // We set the expression to be the entire where clause condition of this statement.
            //    TExpression expr = stmt.WhereClause.Condition;

            //    //This it the call to parse the expression tree in post order.
            //    expr.postOrderTraverse(new exprVisitor());
            //}


            for (int i = 0; i < stmt.Statements.size(); i++)
            {
                iterateStmt(stmt.Statements.get(i));
            }

        }


    }

    internal class exprVisitor : IExpressionVisitor
    {

        public virtual bool exprVisit(TParseTreeNode pNode, bool isLeafNode)
        {
            if (!isLeafNode)
            {
                // We return the value or appended value after recursion of the where clause of this statement/nested query
                // columnTableStmt.ExpressionStatement = pNode.String;
                Console.WriteLine(pNode.String);
            }
            return true;
        }
    }
}
