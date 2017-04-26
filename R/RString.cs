using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace R
{
    public static class RString
    {
        /// <summary>
        ///     Convertit un string en int
        /// </summary>
        /// <param name="str">String d'origine</param>
        /// <returns>int</returns>
        public static int ToInt(this string str)
        {
            int number;
            return int.TryParse(str, out number) ? number : -1;
        }

        /// <summary>
        ///     Convertit un string en double
        /// </summary>
        /// <param name="str">String d'origine</param>
        /// <returns>double</returns>
        public static double ToDouble(this string str)
        {
            str = str.Replace(".", ",");
            double number;
            return double.TryParse(str, out number) ? number : -1;
        }

        /// <summary>
        ///     Convertit un string en float
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>float</returns>
        public static float ToFloat(this string str)
        {
            float number;
            return float.TryParse(str, out number) ? number : -1;
        }

        /// <summary>
        ///     Convertit un string en byte
        /// </summary>
        /// <param name="str">le string à convertir</param>
        /// <returns>byte</returns>
        public static byte ToByte(this string str)
        {
            try
            {
                return Convert.ToByte(str);
            }
            catch (Exception)
            {
                return new byte();
            }
        }

        /// <summary>
        ///     Convertit un string en tableau de byte
        /// </summary>
        /// <param name="str">Le string à convertir</param>
        /// <returns>byte[]</returns>
        public static byte[] ToTabByte(this string str)
        {
            try
            {
                return new ASCIIEncoding().GetBytes(str);
            }
            catch (Exception)
            {
                return new byte[0];
            }
        }

        /// <summary>
        ///     Ecrit sur la console l'object avec un retour à la ligne
        /// </summary>
        /// <param name="str">Object à écrire sur la console</param>
        public static void WriteLn(this Object str)
        {
            string s = str.ToString();
            if (!string.IsNullOrEmpty(s))
                Console.WriteLine(s);
        }

        /// <summary>
        ///     Ecrit sur la console de debug avec un retour à la ligne
        /// </summary>
        /// <param name="str"></param>
        public static void DebugWriteLn(this Object str)
        {
            string s = str.ToString();
            if (!string.IsNullOrEmpty(s))
                Debug.WriteLine(s);
        }

        /// <summary>
        ///     Ecrit sur la console l'object sans retour à la ligne
        /// </summary>
        /// <param name="str">String à écrire sur la console</param>
        public static void Write(this Object str)
        {
            string s = str.ToString();
            if (!string.IsNullOrEmpty(s))
                Console.Write(s);
        }

        /// <summary>
        ///     Ecrit sur la console de debug sans retour à la ligne
        /// </summary>
        /// <param name="str"></param>
        public static void DebugWrite(this Object str)
        {
            string s = str.ToString();
            if (!string.IsNullOrEmpty(s))
                Debug.Write(s);
        }

        /// <summary>
        ///     Permet de récupérer les infos saisies par l'utilisateur
        /// </summary>
        /// <returns>int</returns>
        public static int ConsoleRead()
        {
            return Console.Read();
        }

        /// <summary>
        ///     Permet de récupérer les infos saisies par l'utilisateur avec un saut de ligne
        /// </summary>
        /// <returns>string</returns>
        public static string ConsoleReadLine()
        {
            return Console.ReadLine();
        }

        /// <summary>
        ///     Convertit un string en tableau de string via le séparateur
        /// </summary>
        /// <param name="str"></param>
        /// <param name="separator">Le séparateur</param>
        /// <returns>string[]</returns>
        public static string[] ToArray(this string str, char separator)
        {
            return str.Split(separator);
        }

        /// <summary>
        ///     Vérifie qu'un string est un entier
        /// </summary>
        /// <param name="str"></param>
        /// <returns>bool</returns>
        public static bool CheckInt(this string str)
        {
            int i;
            return int.TryParse(str, out i);
        }

        /// <summary>
        ///     Supprime les espaces qui se suivent
        /// </summary>
        /// <param name="str">String d'origine</param>
        /// <returns>string</returns>
        public static string EnleverEspaceMultiple(this string str)
        {
            try
            {
                if (str.IsNullOrEmpty()) return str;
                string contenuTemp = null;
                for (int i = 0; i < str.Length; i++)
                {
                    if ((i + 1) < str.Length)
                    {
                        if (!str[i].Equals(' ') || !str[i + 1].Equals(' '))
                        {
                            contenuTemp += str[i];
                        }
                    }
                    else
                    {
                        contenuTemp += str[i];
                    }
                }
                return contenuTemp;
            }
            catch (Exception)
            {
                return str;
            }
        }

        /// <summary>
        ///     Permet de supprimer les retour chariot
        /// </summary>
        /// <param name="str"></param>
        /// <returns>string</returns>
        public static string DeleteRetourChariot(this string str)
        {
            try
            {
                if (str.IsNullOrEmpty()) return str;
                return str.Replace("\r\n", string.Empty);
            }
            catch (Exception)
            {
                return str;
            }
        }

        /// <summary>
        ///     Permet de supprimer une sous-chaine personnalisée ex : an ayant un href, supprimer la balise img
        /// </summary>
        /// <param name="str">Le string d'origine</param>
        /// <param name="debutSubstring">La chaîne qui symbolise le début de la recherche</param>
        /// <param name="finSubstring">La chaîne qui symbolise la fin de la recherche</param>
        /// <returns>string</returns>
        public static string EnleverChainePerso(this string str, string debutSubstring, string finSubstring)
        {
            if (str.IsNullOrEmpty()) return str;

            int i = 0;
            int secours = -1;
            while ((i = str.IndexOf(debutSubstring, i, StringComparison.Ordinal)) != -1)
            {
                if (i == secours) break;
                secours = i;
                //On a trouvé le début de la chaîne, on récup la position de la fin de chaîne
                int a = str.IndexOf(finSubstring, i, StringComparison.Ordinal);

                if (a <= 0) break;

                str = str.Remove(i, ((a + finSubstring.Length) - i));
            }

            return str;
        }

        /// <summary>
        ///     Vérifie si compare existe dans le sous-chaîne spécifié grâce au index depart et arrivée
        /// </summary>
        /// <param name="str">Le string d'origine</param>
        /// <param name="depart">Index de départ</param>
        /// <param name="arrivee">Index de fin de recherche</param>
        /// <param name="compare">La chaîne à rechercher</param>
        /// <returns>bool</returns>
        public static bool ContainsEntre2Positions(this string str, int depart, int arrivee, string compare)
        {
            string temp = str.Substring(depart, (arrivee - depart));
            return temp.Contains(compare);
        }

        /// <summary>
        ///     Supprime une sous-chaîne entre les 2 positions
        /// </summary>
        /// <param name="str">Le string d'origine</param>
        /// <param name="depart">Index de départ</param>
        /// <param name="arrivee">Index de fin</param>
        /// <returns>string</returns>
        public static string DeleteEntre2Index(this string str, int depart, int arrivee)
        {
            return str.Where((t, i) => i < depart || i > arrivee)
                .Aggregate<char, string>(null, (current, t) => current + t);
        }

        /// <summary>
        ///     Définit si un string est null ou vide
        /// </summary>
        /// <param name="str"></param>
        /// <returns>bool</returns>
        public static bool IsNullOrEmpty(this string str)
        {
            try
            {
                return string.IsNullOrWhiteSpace(str);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsNotNullOrEmpty(this string str)
        {
            try
            {
                return !IsNullOrEmpty(str);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsExtensionImage(this string str)
        {
            try
            {
                return !str.IsNullOrEmpty() && str.InArray(new[] {"jpg", "png", "gif", "jpeg"});
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Permet de remplacer une sous-chaîne par une autre sous-chaîne
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strRecherche">La chaîne à rechercher</param>
        /// <param name="strReplace">La chaîne à remplacer</param>
        /// <returns>string</returns>
        public static string RmyReplace(this string str, string strRecherche, string strReplace)
        {
            if (str.IsNullOrEmpty()) return str;

            if (str.Contains(strRecherche))
                str = str.Replace(strRecherche, strReplace);

            return str;
        }

        /// <summary>
        ///     Permet de spliter un string via un autre string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="split">Le stirng à chercher</param>
        /// <returns>string[]</returns>
        public static string[] SplitString(this string str, string split)
        {
            try
            {
                return str.Split(new[] { split }, StringSplitOptions.None);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Permet de formater du texte en HTML
        /// </summary>
        /// <param name="str"></param>
        /// <returns>string</returns>
        public static string HtmlEncode(this string str)
        {
            try
            {
                return str.IsNullOrEmpty() ? str : HttpUtility.HtmlEncode(str);
            }
            catch (Exception)
            {
                return str;
            }
        }

        public static string DecodeUtf8String(this string utf8Str)
        {
            try
            {
                if (utf8Str.IsNullOrEmpty()) return null;
                Encoding iso = Encoding.GetEncoding("ISO-8859-1");

                Encoding utf8 = Encoding.UTF8;

                byte[] utfBytes = utf8.GetBytes(utf8Str);

                byte[] isoBytes = Encoding.Convert(utf8, iso, utfBytes);

                var encoding = new UTF8Encoding();

                return encoding.GetString(isoBytes);
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        ///     Permet de formater de l'html en texte
        /// </summary>
        /// <param name="str"></param>
        /// <returns>string</returns>
        public static string HtmlDecode(this string str)
        {
            try
            {
                return str.IsNullOrEmpty() ? null : HttpUtility.HtmlDecode(str);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Permet l'affichage d'une messageBox
        /// </summary>
        /// <param name="str"></param>
        public static void MessageBox(this string str)
        {
            if (str.IsNullOrEmpty()) return;
            System.Windows.Forms.MessageBox.Show(str);
        }

        /// <summary>
        ///     Permet de formater un string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="tab"></param>
        /// <returns>string</returns>
        public static string Format(string str, string[] tab)
        {
            string strResult = str;

            int count;
            for (count = 0; count < tab.Length; count++)
            {
                if (strResult.Contains("{" + count + "}"))
                {
                    strResult = strResult.RmyReplace("{" + count + "}", tab.Length > count ? tab[count] : "");
                }
            }

            //On vérifie que d'autres arguments ne sont pas en attente
            bool check = true;
            while (check)
            {
                if (strResult.Contains("{" + count + "}"))
                {
                    strResult = strResult.RmyReplace("{" + count + "}", "");
                }
                else
                {
                    check = false;
                }

                count++;
            }

            return strResult;
        }

        /// <summary>
        ///     Permet de lancer un naviguateur web via une url
        /// </summary>
        /// <param name="str"></param>
        /// <returns>bool</returns>
        public static bool OpenBrowser(this string str)
        {
            try
            {
                var sInfo = new ProcessStartInfo(str);
                Process.Start(sInfo);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Permet de créer un nom composé
        /// </summary>
        /// <param name="nom">Nom de la personne</param>
        /// <param name="prenom">Prénom de la personne</param>
        /// <returns>string</returns>
        public static string ToNomCompose(this string nom, string prenom)
        {
            try
            {
                if (nom.IsNullOrEmpty() || prenom.IsNullOrEmpty()) return null;
                nom = nom.ToLower();
                prenom = prenom.ToLower();

                nom = nom[0].ToString(CultureInfo.InvariantCulture).ToUpper() + nom.Substring(1);
                prenom = prenom[0].ToString(CultureInfo.InvariantCulture).ToUpper() + prenom.Substring(1);

                return nom + " " + prenom;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Permet de formater un nom
        /// </summary>
        /// <param name="nom">Le nom à formater</param>
        /// <returns>string</returns>
        public static string ToFormatNom(this string nom)
        {
            try
            {
                if (nom.IsNullOrEmpty()) return nom;

                //On regarde si tremat
                if (nom.Contains("-"))
                {
                    var tab = nom.Split('-').ToList();
                    string reponse = null;
                    foreach (var s in tab)
                    {
                        reponse += s[0].ToString(CultureInfo.InvariantCulture).ToUpper() + s.Substring(1);
                        if (!tab.CheckLastElement(s))
                            reponse += "-";
                    }
                    return reponse;
                }
                if (nom.Contains(" "))
                {
                    var tab = nom.Split(' ').ToList();
                    string reponse = null;
                    foreach (var s in tab)
                    {
                        reponse += s[0].ToString(CultureInfo.InvariantCulture).ToUpper() + s.Substring(1);
                        if (!tab.CheckLastElement(s))
                            reponse += "-";
                    }
                    return reponse;
                }

                return nom[0].ToString(CultureInfo.InvariantCulture).ToUpper() + nom.Substring(1);
            }
            catch (Exception)
            {
                return nom;
            }
        }

        public static string RSubstring(this string str, int depart = 0, int arrive = 0)
        {
            try
            {
                if (depart == 0 && arrive == 0) return str;

                return arrive == 0
                    ? str.Substring(depart)
                    : (arrive > str.Length ? str.Substring(depart) : str.Substring(depart, arrive));
            }
            catch (Exception)
            {
                return str;
            }
        }

        /// <summary>
        ///     Formater un string en titre anglais
        /// </summary>
        /// <param name="str">La phrase</param>
        /// <returns>string</returns>
        public static string ToTitleEnglish(this string str)
        {
            try
            {
                if (str.IsNullOrEmpty()) return null;
                string[] tab = str.Split(' ');
                if (tab.IsNull() || tab.Length == 0) return null;

                var tabExcepetion = new[] { "to", "in", "a", "the", "of", "for", "on", "it", "is", "as", "at" };
                string reponse = null;
                foreach (string mot in tab)
                {
                    if (!mot.InArray(tabExcepetion))
                        if (tab.Last().Equals(mot))
                            reponse += mot.ToFormatNom();
                        else
                            reponse += mot.ToFormatNom() + " ";
                    else if (tab.Last().Equals(mot))
                        reponse += mot + " ";
                    else
                        reponse += mot + " ";
                }

                return reponse;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Permet de convertir l'encodage d'un string en UTF8
        /// </summary>
        /// <param name="str">La chaîne</param>
        /// <returns>string</returns>
        public static string ToUtf8(this string str)
        {
            try
            {
                if (str.IsNullOrEmpty()) return str;
                byte[] bytes = Encoding.Default.GetBytes(str);
                return Encoding.UTF8.GetString(bytes);
            }
            catch (Exception)
            {
                return str;
            }
        }

        public static string ToDefault(this string str)
        {
            try
            {
                if (str.IsNullOrEmpty()) return str;
                byte[] bytes = Encoding.Default.GetBytes(str);
                return Encoding.Default.GetString(bytes);
            }
            catch (Exception)
            {
                return str;
            }
        }

        /// <summary>
        ///     Permet d'enlever l'extension d'un chemin
        /// </summary>
        /// <param name="str"></param>
        /// <returns>string</returns>
        public static string WithoutExtension(this string str)
        {
            try
            {
                if (str.IsNullOrEmpty()) return null;
                List<string> tab = str.Split('.').ToList();
                return tab.TakeWhile(ext => !tab.CheckLastElement(ext))
                    .Aggregate<string, string>(null, (current, ext) => current + ext);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Permet d'obtenir l'extension d'un fichier
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetExtension(this string str)
        {
            try
            {
                if (str.IsNullOrEmpty()) return null;

                var tab = str.Split('.');
                if (tab.IsEmpty() || tab.Length < 2) return null;

                return tab.ToList().GetLastElement();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Permet de dire si deux chaînes sont différentes
        /// </summary>
        /// <param name="str"></param>
        /// <param name="str2"></param>
        /// <returns>bool</returns>
        public static bool IsNotEquals(this string str, string str2)
        {
            try
            {
                if (str.IsNullOrEmpty() || str2.IsNullOrEmpty()) return false;
                return !str.Equals(str2);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permet de supprimer es commentaires web d'un string
        /// </summary>
        /// <param name="str"></param>
        /// <returns>string</returns>
        public static string DeleteCommentaireWeb(this string str)
        {
            try
            {
                if (str.IsNullOrEmpty()) return str;
                while (true)
                {
                    //On recherche la premiere occurence du comm
                    int first = str.IndexOf("<!--", StringComparison.Ordinal);
                    if (first < 0)
                        break;
                    //On recherche la fin du comm
                    int end = str.IndexOf("-->", StringComparison.Ordinal);
                    if (end < 0)
                        break;
                    str = str.Remove(first, ((end + 3) - first));
                }
                return str;
            }
            catch (Exception)
            {
                return str;
            }
        }

        /// <summary>
        /// Permet de supprimer les balises script web d'un string
        /// </summary>
        /// <param name="str"></param>
        /// <returns>string</returns>
        public static string DeleteJsWeb(this string str)
        {
            try
            {
                if (str.IsNullOrEmpty()) return str;
                const string strFirst = "<script";
                const string strEnd = "</script>";
                while (true)
                {
                    //On recherche la premiere occurence du comm
                    int first = str.IndexOf(strFirst, StringComparison.Ordinal);
                    if (first < 0)
                        break;
                    //On recherche la fin du comm
                    int end = str.IndexOf(strEnd, StringComparison.Ordinal);
                    if (end < 0)
                        break;
                    str = str.Remove(first, ((end + strEnd.Length) - first));
                }
                return str;
            }
            catch (Exception)
            {
                return str;
            }
        }

        /// <summary>
        /// Permet de vérifier si une chaîne est contenue dans une autre
        /// </summary>
        /// <param name="str"></param>
        /// <param name="recherche">La recherche</param>
        /// <param name="ignoreCase">Ignorer la case</param>
        /// <returns></returns>
        public static bool Contains(this string str, string recherche, bool ignoreCase = false)
        {
            try
            {
                if (!ignoreCase) return str.Contains(recherche);
                recherche = recherche.ToLower();
                str = str.ToLower();

                return str.Contains(recherche);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permet de vérifier si plusieurs chaînes sont contenues dans une chaînes
        /// </summary>
        /// <param name="str"></param>
        /// <param name="tabRecherce">Le tableau de chaînes</param>
        /// <param name="ignoreCase">Ignore case</param>
        /// <returns></returns>
        public static bool Contains(this string str, string[] tabRecherce, bool ignoreCase = true)
        {
            try
            {
                if (ignoreCase) str = str.ToLower();
                return tabRecherce.Any(s => str.Contains(s));
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permet de replacer dans une chaîne plusieurs chaînes
        /// </summary>
        /// <param name="str"></param>
        /// <param name="tab"></param>
        /// <returns></returns>
        public static string Replace(this string str, Replace[] tab)
        {
            try
            {
                if (tab.NullOrEmpty() || str.IsNullOrEmpty()) return str;
                foreach (var replace in tab.Where(replace => str.Contains(replace.OldChar)))
                {
                    str = str.Replace(replace.OldChar, replace.NewChar);
                }

                return str;
            }
            catch (Exception)
            {
                return str;
            }
        }

        public static bool IsNumber(this char str)
        {
            try
            {
                return str >= '0' && str <= '9';
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CompareIgnoreNonSpace(this string str, string toCompare = null)
        {
            try
            {
                if (str.IsNullOrEmpty() || toCompare.IsNullOrEmpty()) return false;
                return
                    String.Compare(str, toCompare, CultureInfo.CurrentCulture,
                        CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permet de mettre la première lettre d'une chaîne en majuscule
        /// </summary>
        /// <param name="str">La chaîne à modifier</param>
        /// <returns>string</returns>
        public static string UpperFirstLetter(this string str)
        {
            try
            {
                if (str.IsNullOrEmpty()) return str;
                str = str.ToLower();
                return str[0].ToString(CultureInfo.InvariantCulture).ToUpper() + str.Substring(1);
            }
            catch (Exception)
            {
                return str;
            }
        }

        public static bool IsCodePostal(this string str)
        {
            try
            {
                const RegexOptions options = RegexOptions.None;
                Regex regex = new Regex(@"^\d{5}$", options);
                return regex.IsMatch(str);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public class Replace
    {
        public string OldChar { get; set; }
        public string NewChar { get; set; }
    }
}