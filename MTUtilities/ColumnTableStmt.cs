using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gudusoft.gsqlparser;
using gudusoft.gsqlparser.nodes;


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
                for (int i = 0; i < sqlparser.sqlstatements.size(); i++)
                {
                    iterateStmt(sqlparser.sqlstatements.get(i));
                }
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
                    if (dictColumns.Where(col=>col.Value.Name == objectName.ColumnNameOnly.ToLower()).Any())
                    {
                        column = dictColumns.FirstOrDefault(col => col.Value.Name == objectName.ColumnNameOnly.ToLower()).Value;
                    }

                    // Set the column name and occurence irrespective of the item being found in dictionary or not.
                    column.Name = objectName.ColumnNameOnly.ToLower();
                    column.TotalNumOfOccurrences += 1;

                    // Switch over the location of the column and update the occurence accrodingly.
                    switch (objectName.Location)
                    {
                        case ESqlClause.unknown:
                            {
                                column.UnknownOccurences += 1;
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

                    // Assign the column dictionary along with the dbtable object back to the global dbtable dictionary.
                    long KeyValue = dictColumns.FirstOrDefault(col => col.Value.Name == column.Name).Key;
                    if (KeyValue == 0)
                    {
                        dictColumns[objectName.ColumnNo] = column;
                    }
                    else
                    {
                        dictColumns[KeyValue] = column;
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
