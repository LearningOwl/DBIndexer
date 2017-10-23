using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTUtilities
{
    public class DBIndex
    {
        public long Index_Object_Id = 0;
        public string Index_Type = "";
        public int Index_Pages = 0;
        public Boolean Index_Updated = false;
        public Boolean IsComposite = false;
        public Boolean IsClustered = true;

    }
}
