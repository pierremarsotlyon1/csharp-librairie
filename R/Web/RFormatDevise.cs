using System;
using System.Globalization;

namespace R
{
    public static class RFormatDevise
    {
        /// <summary>
        /// Permet de formater un string avec la devise euros
        /// </summary>
        /// <param name="str">Le string à formater</param>
        /// <returns>string</returns>
        public static string FormatDeviseEuros(this string str)
        {
            try
            {
                return str.FormatDevise("€");
            }
            catch (Exception)
            {
                return str;
            }
        }

        /// <summary>
        /// Permet de formater un int avec la devise euros
        /// </summary>
        /// <param name="i">Le int à formater</param>
        /// <returns>string</returns>
        public static string FormatDeviseEuros(this int i)
        {
            try
            {
                var str = i.ToString(CultureInfo.InvariantCulture);
                return str.FormatDevise("€");
            }
            catch (Exception)
            {
                return i.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Permet de formater un float avec la devise euros
        /// </summary>
        /// <param name="f">Le float à formater</param>
        /// <returns>string</returns>
        public static string FormatDeviseEuros(this float f)
        {
            try
            {
                var str = f.ToString(CultureInfo.InvariantCulture);
                return str.FormatDevise("€");
            }
            catch (Exception)
            {
                return f.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Permet de formater un double avec la devise euros
        /// </summary>
        /// <param name="d">Le double à formater</param>
        /// <returns>string</returns>
        public static string FormatDeviseEuros(this double d)
        {
            try
            {
                var str = d.ToString(CultureInfo.InvariantCulture);
                return str.FormatDevise("€");
            }
            catch (Exception)
            {
                return d.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Permet de formater un string avec une devise
        /// </summary>
        /// <param name="str">Le string à formater</param>
        /// <param name="devise">La devise à ajouter</param>
        /// <returns>string</returns>
        public static string FormatDevise(this string str, string devise = null)
        {
            try
            {
                if (str.IsNullOrEmpty() || devise.IsNull()) return str;

                return str + " " + devise;
            }
            catch (Exception)
            {
                return str;
            }
        }
    }
}
