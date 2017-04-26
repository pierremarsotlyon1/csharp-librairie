using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R
{
    public static class RDouble
    {
        public static string DeuxDecimal(this double d)
        {
            try
            {
                return d.ToString("F");
            }
            catch (Exception)
            {
                return d.ToString(CultureInfo.InvariantCulture);
            }   
        }
    }
}
