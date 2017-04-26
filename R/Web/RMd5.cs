using System;
using System.Security.Cryptography;
using System.Text;

namespace R.Web
{
    public static class RMd5
    {
        /// <summary>
        /// Permet de créer un MD5 via un string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="upper">bool</param>
        /// <returns>string</returns>
        public static string ToMd5(this string str, bool upper = false)
        {
            MD5 md5 = null;
            try
            {
                if (str.IsNullOrEmpty()) return null;
                md5 = MD5.Create();
                byte[] inputBytes = Encoding.ASCII.GetBytes(str);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                //Conversion du tableau de byte en string hexa
                var sb = new StringBuilder();
                foreach (byte t in hashBytes)
                {
                    sb.Append(upper ? t.ToString("X2") : t.ToString("x2"));
                }
                return sb.ToString();
            }
            catch
            {
                return null;
            }
            finally
            {
                if (md5 != null)
                {
                    md5.Clear();
                    md5.Dispose();
                }
            }
        }

        /// <summary>
        /// Compare 2 scryptes MD5
        /// </summary>
        /// <param name="str"></param>
        /// <param name="md5AComparer">MD5 à comparer au string</param>
        /// <returns>bool</returns>
        public static bool CompareMd5(this string str, string md5AComparer)
        {
            try
            {
                string md5 = str.ToMd5();
                return md5.Equals(md5AComparer);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
