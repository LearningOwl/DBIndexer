using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTUtilities
{
    public class Column
    {
        public long Column_Object_Id = 0;
        public float ReductionFactor= 0;
        public float Selectivity = 0;
        public Boolean IsNull = false;
        public Boolean IsIndexed = false;


    }
}
