using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace R
{
    public static class RValidateRegex
    {
        /// <summary>
        /// Vérifie si un email est correct
        /// </summary>
        /// <param name="email">L'email</param>
        /// <returns>bool</returns>
        public static bool ValidateEmail(this string email)
        {
            try
            {
                var regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(email);

                return match.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permet de vérifier un numéro de tel
        /// </summary>
        /// <param name="tel"></param>
        /// <returns>bool</returns>
        public static bool ValideTel(this string tel)
        {
            try
            {
                if (tel.IsNullOrEmpty()) return false;
                return tel.Length == 10;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Vérifie si un numéro de tel est correcte
        /// </summary>
        /// <param name="mobile">Le numéro du mobile</param>
        /// <returns>bool</returns>
        public static bool IsPhoneNumber(this string mobile)
        {
            var all = mobile.All(c => c >= '0' && c <= '9');
            return mobile.Length == 10 && all;
        }
    }
}