using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R
{
    public static class RBool
    {
        public static bool IsTrue(this bool b)
        {
            return b;
        }

        public static bool IsFalse(this bool b)
        {
            return b == false;
        }

        /// <summary>
        /// Permet de connaître le titre du genre d'une personne
        /// 1 = Homme
        /// 2 = Femme
        /// </summary>
        /// <param name="i">Int du genre</param>
        /// <returns>string</returns>
        public static string GetTitreGenre(this bool i)
        {
            try
            {
                if (i.IsTrue())
                    return "Monsieur";
                if (i.IsFalse())
                    return "Madame";
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
