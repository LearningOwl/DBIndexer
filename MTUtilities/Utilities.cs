using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XsPDF.Charting;
using XsPDF.Pdf;
using XsPDF.Drawing;
using XsPDF.Fonts;
using System.Data.SqlClient;

namespace MTUtilities
{
    

    public class Utilities
    {
        public static string ExceptionMessage = "";

        // Holds all the parsed tables during the query workload execution.
        public static Dictionary<string, DBTable> DictParsedTables = new Dictionary<string, DBTable>();
        
        // Holds all the queries that were executed in the workload.
        public static Dictionary<Guid, SQLQueryStmt> DictWorkloadQueries = new Dictionary<Guid, SQLQueryStmt>();

        // Holds all the tables from the database Catalog
        public static Dictionary<string, DBTable> DictTablesCatalog = new Dictionary<string, DBTable>();
        
        // Holds the final score for each table.
        //public static Dictionary<string, long> TableScore = new Dictionary<string, long>();


        public Boolean PerformChecksUp(string ServerName, string DBFilepath, string UserName, string Password, string DBName)
        {
            try
            {
                Boolean CheckUpSuccess = false;

                Validation validation = new Validation();
                CheckUpSuccess = validation.CheckInputParameters(ServerName, DBFilepath, UserName, Password, DBName);


                return CheckUpSuccess;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }


        public static void CreateIndexSuggestions(List<string> QueryList,string ServerName, string DBFilePath, string UserName, string Password, string DBName, Boolean CreateMode)
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
            using (SqlConnection connection =
                new SqlConnection(ConnectionString))
            {

                string querytext = "";
                SqlCommand cmd;
                connection.Open();
                if (CreateMode == true)
                {
                    // Creating indexes
                    foreach (string item in QueryList)
                    {
                        cmd = new SqlCommand(item, connection);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Dropping indexes
                    foreach (string item in QueryList)
                    {
                        cmd = new SqlCommand(item, connection);
                        cmd.ExecuteNonQuery();
                    }
                }

            }

        }


        public static void ExportToPDF(ConsoleTable consoletable , string TypeOfDAta)
        {
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            
            Chart chart = LineChart();
            ChartFrame chartFrame = new ChartFrame();
            chartFrame.Location = new XPoint(30, 30);
            chartFrame.Size = new XSize(500, 600);
            chartFrame.Add(chart);
            XGraphics g = XGraphics.FromPdfPage(page);
            chartFrame.Draw(g);
            document.Save("IndexResults.pdf");
            // This is the code to open the PDF file that we just created.
            //Process.Start("IndexResults.pdf");

        }


        public static Chart LineChart()
        {

            Chart chart = new Chart(ChartType.Line);
            Series series = chart.SeriesCollection.AddSeries();
            series.Name = "Time after Index";
            series.Add(new double[] { 1, 12, -6, 15, 11 }); //Time after Index
            series = chart.SeriesCollection.AddSeries();
            series.Name = "Time before index";
            series.Add(new double[] { 22, 4, 12, 8, 12 });//Time before index
            chart.XAxis.MajorTickMark = TickMarkType.Outside;
            chart.XAxis.Title.Caption = "Query";
            //chart.XAxis.HasMajorGridlines = true;
            chart.YAxis.MajorTickMark = TickMarkType.Outside;
            chart.YAxis.Title.Caption = "Time";
            chart.YAxis.HasMajorGridlines = true;
            //chart.PlotArea.LineFormat.Color = XColors.DarkGray;
            chart.PlotArea.LineFormat.Color = XColors.Red;
            chart.PlotArea.LineFormat.Width = 1;
            chart.PlotArea.LineFormat.Visible = true;
            chart.Legend.Docking = DockingType.Bottom;
            chart.Legend.LineFormat.Visible = true;
            XSeries xseries = chart.XValues.AddXSeries();
            xseries.Add("Q1", "Q2", "Q3", "Q4", "Q5", "Q6");
            return chart;
        }


    }
}
