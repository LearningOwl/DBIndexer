using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTUtilities
{
    

    public class Utilities
    {
        public static string ExceptionMessage = "";

        public Boolean PerformChecksUp(string ServerName, string DBFilepath, string UserName, string Password, string DBName)
        {
            try
            {
                Boolean CheckUpSuccess = false;

                Validation validation = new Validation();
                validation.CheckInputParameters(ServerName, DBFilepath, UserName, Password, DBName);


                return CheckUpSuccess;
            }
            catch (Exception Ex)
            {
                
                return false;
            }
        }
       
        
    }
}
