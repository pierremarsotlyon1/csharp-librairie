using System;

namespace R
{
    public static class RChar
    {
        /// <summary>
        /// Permet de vérifier si un char est un nombre
        /// </summary>
        /// <param name="c">char</param>
        /// <returns>bool</returns>
        public static bool CheckInt(this char c)
        {
            return Char.IsNumber(c);
        }

        /// <summary>
        /// Permet de convertir un char en string
        /// </summary>
        /// <param name="c">char</param>
        /// <returns>string</returns>
        public static string RToString(this char c)
        {
            return Char.ToString(c);
        }

        /// <summary>
        /// Permet de convertir un char en double
        /// </summary>
        /// <param name="c"></param>
        /// <returns>double</returns>
        public static double ToDouble(this char c)
        {
            double d;
            return Double.TryParse(c.RToString(), out d) ? d : -1;
        }

        /// <summary>
        /// Permet de convertir un char en int
        /// </summary>
        /// <param name="c"></param>
        /// <returns>int</returns>
        public static int ToInt(this char c)
        {
            int i;
            return Int32.TryParse(c.RToString(), out i) ? i : -1;
        }

        /// <summary>
        /// Permet de convertir un char en float
        /// </summary>
        /// <param name="c"></param>
        /// <returns>float</returns>
        public static float ToFloat(this char c)
        {
            float f;
            return float.TryParse(c.RToString(), out f) ? f : -1;
        }

        /// <summary>
        /// Permet de convertir un char en int
        /// </summary>
        /// <param name="c">char</param>
        /// <returns>int</returns>
        public static int AlphaToInt(this char c)
        {
            if (c.Equals(null)) return 0;

            var x = c.RToString().ToUpper();
            switch (x)
            {
                case "A":
                    return 1;
                case "B":
                    return 2;
                case "C":
                    return 3;
                case "D":
                    return 4;
                case "E":
                    return 5;
                case "F":
                    return 6;
                case "G":
                    return 7;
                case "H":
                    return 8;
                case "I":
                    return 9;
                case "J":
                    return 10;
                case "K":
                    return 11;
                case "L":
                    return 12;
                case "M":
                    return 13;
                case "N":
                    return 14;
                case "O":
                    return 15;
                case "P":
                    return 16;
                case "Q":
                    return 17;
                case "R":
                    return 18;
                case "S":
                    return 19;
                case "T":
                    return 20;
                case "U":
                    return 21;
                case "V":
                    return 22;
                case "W":
                    return 23;
                case "X":
                    return 24;
                case "Y":
                    return 25;
                case "Z":
                    return 26;
            }

            return 0;
        }
    }
}
