using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R
{
    public class ConditionMeteo
    {
        public string City { get; set; }
        public string DayOfWeek = DateTime.Now.DayOfWeek.ToString();
        public string Condition { get; set; }
        public string Temperature { get; set; }
        public string Humidity { get; set; }
        public string Wind { get; set; }
        public string High { get; set; }
        public string Low { get; set; }
    }

    public static class ConversionMeteo
    {
        public static string ToDegres(this string fahrenheit)
        {
            try
            {
                if (fahrenheit.IsNullOrEmpty()) return null;
                int degF = fahrenheit.ToInt();
                return ((degF - 32) * 5 / 9).ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
