using System;

namespace R
{
    public static  class Formatage
    {
        public static double FixedTo(this double number, int nombreAfterVirgule = 2)
        {
            try
            {
                var x = Math.Truncate(number*Math.Pow(10, nombreAfterVirgule));
                return x/100;
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
