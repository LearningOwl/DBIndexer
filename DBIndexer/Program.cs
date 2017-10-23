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

                    MTUtilities.Validation validation = new Validation();
                    validation.CheckInputParameters("", "", "", "", "");
                    // Implement the calls here to MTUtilities.Utilities.PerformCheckUp for checking.

                }

            }
            catch (Exception Ex)
            {

                throw;
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
