using System;
using System.Globalization;

namespace R
{
    public static class RInt
    {
        /// <summary>
        /// Permet de connaître le genre d'une personne
        /// 1 = Homme
        /// 2 = Femme
        /// </summary>
        /// <param name="i">Int du genre</param>
        /// <returns>bool</returns>
        public static bool GetGenre(this int i)
        {
            try
            {
                return i == 1;
            }
            catch (Exception)
            {
                return true;
            }
        }

        /// <summary>
        /// Vérifi si un int est égal à zéro
        /// </summary>
        /// <param name="i">Le chiffre à controler</param>
        /// <returns>bool</returns>
        public static bool IsDefault(this int i)
        {
            try
            {
                return i == 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Vérifi si un chiffre n'est pas égal à zéro
        /// </summary>
        /// <param name="i">Le chiffre à controler</param>
        /// <returns>bool</returns>
        public static bool IsNotDefault(this int i)
        {
            try
            {
                return i != 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Convertit un int en string
        /// </summary>
        /// <param name="i">int</param>
        /// <returns>string</returns>
        public static string RToString(this int i)
        {
            return i.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convertit un int en double
        /// </summary>
        /// <param name="i">int</param>
        /// <returns>double</returns>
        public static double ToDouble(this int i)
        {
            double d;
            return double.TryParse(RToString(i), out d) ? d : 0;
        }

        /// <summary>
        /// Convertit un int en float
        /// </summary>
        /// <param name="i">int</param>
        /// <returns>float</returns>
        public static float ToFloat(this int i)
        {
            float f;
            return float.TryParse(RToString(i), out f) ? f : 0;
        }

        /// <summary>
        /// Permet de convertir un int en lettre
        /// </summary>
        /// <param name="i">int</param>
        /// <returns>string</returns>
        public static string ToLettreAlpha(this int i)
        {
            if (i < 1 || i > 26) return null;
            switch (i)
            {
                case 1:
                    return "A";
                case 2:
                    return "B";
                case 3:
                    return "C";
                case 4:
                    return "D";
                case 5:
                    return "E";
                case 6:
                    return "F";
                case 7:
                    return "G";
                case 8:
                    return "H";
                case 9:
                    return "I";
                case 10:
                    return "J";
                case 11:
                    return "K";
                case 12:
                    return "L";
                case 13:
                    return "M";
                case 14:
                    return "N";
                case 15:
                    return "O";
                case 16:
                    return "P";
                case 17:
                    return "Q";
                case 18:
                    return "R";
                case 19:
                    return "S";
                case 20:
                    return "T";
                case 21:
                    return "U";
                case 22:
                    return "V";
                case 23:
                    return "W";
                case 24:
                    return "X";
                case 25:
                    return "Y";
                case 26:
                    return "Z";
                default:
                    return null;
            }
        }
    }
}
