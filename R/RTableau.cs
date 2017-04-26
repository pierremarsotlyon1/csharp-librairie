using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace R
{
    public static class RTableau
    {
        public delegate void Boucle();
        
        /// <summary>
        /// Vérifie si un élément est null ou vide
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns>bool</returns>
        public static bool NullOrEmpty<T>(this IEnumerable<T> o)
        {
            try
            {
                var enumerable = o as T[] ?? o.ToArray();
                return !enumerable.Any() || enumerable.Any(elem => String.IsNullOrEmpty(elem.ToString()));
            }
            catch (Exception)
            {
                return true;
            }
        }

        /// <summary>
        /// Permet de remplacer une chaine dans un élément
        /// </summary>
        /// <param name="o"></param>
        /// <param name="strResearch">La chaîne à chercher</param>
        /// <param name="strReplace">La chaîne à remplacer</param>
        public static void Replace(this string[] o, string strResearch, string strReplace)
        {
            try
            {
                foreach (var s in o.Where(s => s.Contains(strReplace)))
                {
                    // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                    s.Replace(strResearch, strReplace);
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Permet de faire une boucle
        /// </summary>
        /// <param name="nbTour"></param>
        /// <param name="b">L'action à efféctuer</param>
        public static void DoLoop(this int nbTour, Boucle b)
        {
            for (int i = 0; i < nbTour; i++)
            {
                b();
            }
        }

        /// <summary>
        /// Permet d'ordonner un tableau de façon ascendante
        /// </summary>
        /// <param name="liste">Le tableau à ordonner</param>
        public static void TriAscendant(this Array liste)
        {
            try
            {
                Array.Sort(liste);
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Permet d'ordonner un tableau de façon descendante
        /// </summary>
        /// <param name="liste">Le tableau à ordonner</param>
        public static void TriDescendant(this Array liste)
        {
            try
            {
                TriAscendant(liste);
                Array.Reverse(liste);
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }
    }
}
