using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R
{
    public static class RTimeSpan
    {
        public static string ToFrench(this TimeSpan timespan)
        {
            try
            {
                return timespan.ToString(@"hh\:mm");
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static TimeSpan RoundTo(this TimeSpan timeSpan, int n)
        {
            return TimeSpan.FromMinutes(n * Math.Ceiling(timeSpan.TotalMinutes / n));
        }
    }
}
