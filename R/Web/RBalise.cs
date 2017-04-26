using System;
using System.Linq;

namespace R
{
    public static class RBalise
    {
        /// <summary>
        ///     Permet de créer une balise span
        /// </summary>
        /// <param name="str"></param>
        /// <param name="attr">Tableau d'attribut</param>
        /// <returns>string</returns>
        public static string BaliseP(this string str, RFieldBalise[] attr = null)
        {
            return "<p" + GenererAttr(ref attr) + ">" + str + "</p>";
        }

        /// <summary>
        ///     Permet de créer un span
        /// </summary>
        /// <param name="str"></param>
        /// <param name="attr">Tableau d'attribut</param>
        /// <returns>string</returns>
        public static string BaliseSpan(this string str, RFieldBalise[] attr = null)
        {
            return "<span" + GenererAttr(ref attr) + ">" + str + "</span>";
        }

        /// <summary>
        ///     Permet de créer un titre h1
        /// </summary>
        /// <param name="str"></param>
        /// <param name="attr">Tableau d'attribut</param>
        /// <returns>string</returns>
        public static string BaliseH1(this string str, RFieldBalise[] attr = null)
        {
            return "<h1" + GenererAttr(ref attr) + ">" + str + "</h1>";
        }

        /// <summary>
        ///     Permet de créer un titre h2
        /// </summary>
        /// <param name="str"></param>
        /// <param name="attr">Tableau d'attribut</param>
        /// <returns>string</returns>
        public static string BaliseH2(this string str, RFieldBalise[] attr = null)
        {
            return "<h2" + GenererAttr(ref attr) + ">" + str + "</h2>";
        }

        /// <summary>
        ///     Permet de créer un titre h3
        /// </summary>
        /// <param name="str"></param>
        /// <param name="attr">Tableau d'attribut</param>
        /// <returns>string</returns>
        public static string BaliseH3(this string str, RFieldBalise[] attr = null)
        {
            return "<h3" + GenererAttr(ref attr) + ">" + str + "</h3>";
        }

        /// <summary>
        ///     Permet de créer un titre h4
        /// </summary>
        /// <param name="str"></param>
        /// <param name="attr">Tableau d'attribut</param>
        /// <returns>string</returns>
        public static string BaliseH4(this string str, RFieldBalise[] attr = null)
        {
            return "<h4" + GenererAttr(ref attr) + ">" + str + "</h4>";
        }

        /// <summary>
        ///     Permet de créer un titre h5
        /// </summary>
        /// <param name="str"></param>
        /// <param name="attr">Tableau d'attribut</param>
        /// <returns>string</returns>
        public static string BaliseH5(this string str, RFieldBalise[] attr = null)
        {
            return "<h5" + GenererAttr(ref attr) + ">" + str + "</h5>";
        }

        /// <summary>
        ///     Permet de créer un titre h6
        /// </summary>
        /// <param name="str"></param>
        /// <param name="attr">Tableau d'attribut</param>
        /// <returns>string</returns>
        public static string BaliseH6(this string str, RFieldBalise[] attr = null)
        {
            return "<h6" + GenererAttr(ref attr) + ">" + str + "</h6>";
        }

        /// <summary>
        /// Permet de générer une table
        /// </summary>
        /// <param name="str"></param>
        /// <param name="attr">Tableau d'attribut</param>
        /// <returns></returns>
        public static string BaliseTable(this string str, RFieldBalise[] attr = null)
        {
            return "<table " + GenererAttr(ref attr) + ">" + str + "</table>";
        }

        /// <summary>
        ///     Permet de créer une balise td
        /// </summary>
        /// <param name="str"></param>
        /// <param name="attr">Tableau d'attribut</param>
        /// <returns>string</returns>
        public static string BaliseTd(this string str, RFieldBalise[] attr = null)
        {
            return "<td" + GenererAttr(ref attr) + ">" + str + "</td>";
        }

        /// <summary>
        ///     Permet de créer une balise tr
        /// </summary>
        /// <param name="str"></param>
        /// <param name="attr">Tableau d'attribut</param>
        /// <returns>string</returns>
        public static string BaliseTr(this string str, RFieldBalise[] attr = null)
        {
            return "<tr" + GenererAttr(ref attr) + ">" + str + "</tr>";
        }

        /// <summary>
        ///     Permet de créer une balise button avec des attributs
        /// </summary>
        /// <param name="str"></param>
        /// <param name="tabAttr">Tableau d'attribut</param>
        /// <returns>string</returns>
        public static string BaliseButton(this string str, RFieldBalise[] tabAttr = null)
        {
            return "<button" + GenererAttr(ref tabAttr) + ">" + str + "</button>";
        }

        /// <summary>
        ///     Permet de créer une balise input avec des attributs
        /// </summary>
        /// <param name="tabAttr">Tableau d'attribut</param>
        /// <returns>string</returns>
        public static string BaliseInput(RFieldBalise[] tabAttr = null)
        {
            return "<input" + GenererAttr(ref tabAttr) + "/>";
        }

        /// <summary>
        ///     Permet de créer une balise label avec des attributs
        /// </summary>
        /// <param name="str"></param>
        /// <param name="tabAttr">Tableau d'attribut</param>
        /// <returns>string</returns>
        public static string BaliseLabel(this string str, RFieldBalise[] tabAttr = null)
        {
            return "<label" + GenererAttr(ref tabAttr) + ">" + str + "</label>";
        }

        /// <summary>
        ///     Permet de créer une balise ouverte div
        /// </summary>
        /// <param name="str"></param>
        /// <param name="tabAttr">Tableau d'attribut</param>
        /// <returns>string</returns>
        public static string BaliseOpenDiv(this string str, RFieldBalise[] tabAttr = null)
        {
            return "<div" + GenererAttr(ref tabAttr) + ">" + str + "</div>";
        }

        /// <summary>
        ///     Permet de fermer une balise div
        /// </summary>
        /// <param name="str"></param>
        /// <returns>string</returns>
        public static string BaliseCLoseDiv(this string str)
        {
            return str + "</div>";
        }

        /// <summary>
        ///     Permet de créer une balise img
        /// </summary>
        /// <param name="str"></param>
        /// <param name="tabAttr">Tableau d'attribut</param>
        /// <returns>string</returns>
        public static string BaliseImg(this string str, RFieldBalise[] tabAttr = null)
        {
            return "<img src='" + str + "' " + GenererAttr(ref tabAttr) + "/>";
        }

        /// <summary>
        ///     Permet de créer une balise strong
        /// </summary>
        /// <param name="str"></param>
        /// <param name="tabAttr">Tableau d'attribut</param>
        /// <returns>string</returns>
        public static string BaliseStrong(this string str, RFieldBalise[] tabAttr = null)
        {
            return "<strong" + GenererAttr(ref tabAttr) + ">" + str + "</strong>";
        }

        /// <summary>
        ///     Permet de créer une balise li
        /// </summary>
        /// <param name="str"></param>
        /// <param name="tabAttr">Tableau d'attribut</param>
        /// <returns>string</returns>
        public static string BaliseLi(this string str, RFieldBalise[] tabAttr = null)
        {
            return "<li" + GenererAttr(ref tabAttr) + ">" + str + "</li>";
        }

        /// <summary>
        ///     Permet de créer une liste ul
        /// </summary>
        /// <param name="str"></param>
        /// <param name="tabAttr">Tableau d'attribut</param>
        /// <returns>string</returns>
        public static string BaliseUl(this string str, RFieldBalise[] tabAttr = null)
        {
            return "<ul" + GenererAttr(ref tabAttr) + ">" + str + "</ul>";
        }

        /// <summary>
        ///     Permet de créer une liste ol
        /// </summary>
        /// <param name="str"></param>
        /// <param name="tabAttr">Tableau d'attribut</param>
        /// <returns>string</returns>
        public static string BaliseOl(this string str, RFieldBalise[] tabAttr = null)
        {
            return "<ol" + GenererAttr(ref tabAttr) + ">" + str + "</ol>";
        }

        /// <summary>
        ///     Permet de créer une balise form
        /// </summary>
        /// <param name="str"></param>
        /// <param name="tabAttr">Tableau d'attribut</param>
        /// <returns>string</returns>
        public static string BaliseForm(this string str, RFieldBalise[] tabAttr = null)
        {
            return "<form" + GenererAttr(ref tabAttr) + ">" + str + "</form>";
        }

        /// <summary>
        ///     Permet de créer une balise a (lien)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="tabAttr">Tableau d'attribut</param>
        /// <returns>string</returns>
        public static string BaliseA(this string str, RFieldBalise[] tabAttr = null)
        {
            return "<a " + GenererAttr(ref tabAttr) + " >" + str + "</a>";
        }

        /// <summary>
        /// Permet de créer une balise section
        /// </summary>
        /// <param name="str"></param>
        /// <param name="tabAttr">Tableau d'attribut</param>
        /// <returns>string</returns>
        public static string BaliseSection(this string str, RFieldBalise[] tabAttr = null)
        {
            return "<section " + GenererAttr(ref tabAttr) + ">" + str + "</section>";
        }

        public static string BaliseSelect(Object[][] tab = null)
        {
            try
            {
                if (tab.IsNull() || tab.Length == 0) return null;
                string reponse = tab.Aggregate("<select>", (current, o) => current + ("<option value='" + o[0] + "'>" + o[1] + "</option>"));
                reponse += "</select>";
                return reponse;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Permet de générer une balise Select avec des chiffres
        /// </summary>
        /// <param name="fin">Nombre de fin inclus</param>
        /// <returns>string</returns>
        public static string BaliseSelectCount(this int fin, RFieldBalise[] tabAttr = null)
        {
            try
            {
                string reponse;
                if(tabAttr.IsNotNull() && tabAttr.Length > 0)
                    reponse = "<select " + GenererAttr(ref tabAttr) + ">";
                else
                    reponse = "<select>";

                if (reponse.IsNullOrEmpty()) return null;
                
                for (int i = 0; i < (fin+1); i++)
                {
                    reponse += "<option value='" + i + "'>" + i + "</option>";
                }
                reponse += "</select>";
                return reponse;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Permet de générer les attributs voulus pour la balise
        /// </summary>
        /// <param name="tabAttr">Tableau d'attribut</param>
        /// <returns>string</returns>
        private static string GenererAttr(ref RFieldBalise[] tabAttr)
        {
            string result = null;
            if (tabAttr == null || tabAttr.Length <= 0) return null;
            // ReSharper disable once ExpressionIsAlwaysNull
            return tabAttr.Aggregate(result,
                (current, balise) => " " + current + (balise.Name + "='" + balise.Value + "' "));
        }
    }

    public class RFieldBalise
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}