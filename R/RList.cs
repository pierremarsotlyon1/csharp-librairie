using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace R
{
    public static class RList
    {
        /// <summary>
        /// Permet d'ajouter un élément en vérifiant les doublons
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o">List</param>
        /// <param name="value">T</param>
        public static bool AjouterSansDoublon<T>(this List<T> o, T value)
        {
            try
            {
                var verif = false;

                // ReSharper disable once UnusedVariable
                foreach (var c in o.Where(c => Equals(c, value)))
                {
                    verif = true;
                }

                if (!verif)
                    o.Add(value);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permet d'ajouter un élément sans que le précédent soit identique à celui courant
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o">List</param>
        /// <param name="value">T</param>
        public static void AjouterSansDoublonQuiSeSuivent<T>(this List<T> o, T value)
        {
            if (o.Count > 0)
            {
                if (!Equals(o[o.Count - 1], value))
                    o.Add(value);
            }
            else
            {
                o.Add(value);
            }
        }

        /// <summary>
        /// Récupère un élément de la liste
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o">List</param>
        /// <param name="index">int</param>
        /// <returns>bool</returns>
        public static object GetElementViaIndex<T>(this List<T> o, int index)
        {
            if (index > -1 && index < (o.Count))
            {
                return o[index];
            }
            return null;
        }

        /// <summary>
        /// Récupère le dérnier élément de la liste
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o">List</param>
        /// <returns>T</returns>
        public static T GetLastElement<T>(this List<T> o)
        {
            return o.Count > 0 ? o[o.Count - 1] : default(T);
        }

        /// <summary>
        /// Permet de supprimer le dernier élément d'une liste
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="liste"></param>
        /// <returns>bool</returns>
        public static bool RemoveLastElement<T>(this List<T> liste)
        {
            try
            {
                return liste.Any() && liste.RemoveElement(liste.GetLastElement());
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permet d'ajouter un objet à un index spécifique
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="liste"></param>
        /// <param name="obj">L'objet</param>
        /// <param name="index">L'index</param>
        /// <returns>bool</returns>
        public static bool Add<T>(this List<T> liste, T obj, int index)
        {
            try
            {
                liste.Insert(index, obj);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///  Permet d'ajouter un élément au début d'une liste
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="l"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<T> AddFirst<T>(this List<T> l, T obj)
        {
            try
            {
                var reponse = new List<T> {obj};
                reponse.AddRange(l);
                return reponse;
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// Renvoie le premier élément de la liste
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o">List</param>
        /// <returns>T</returns>
        public static T GetFirstElement<T>(this List<T> o)
        {
            try
            {
                return o.Count > 0 ? o[0] : default(T);
            }   
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Vérifie si la liste ne posséde pas d'enregistrement
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="liste">List</param>
        /// <returns>bool</returns>
        public static bool IsEmpty<T>(this IEnumerable<T> liste)
        {
            try
            {
                return liste.IsNull() || !liste.Any();
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Vérifie si la liste posséde un enregistrement
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="liste">List</param>
        /// <returns>bool</returns>
        public static bool IsNotEmpty<T>(this IEnumerable<T> liste)
        {
            return !liste.IsEmpty();
        }

        /// <summary>
        /// Permet de savoir si il y a un doublon en fonction de l'element passé
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="liste"></param>
        /// <param name="o">Element à checker</param>
        /// <returns>bool</returns>
        public static bool CheckDoublon<T>(this IEnumerable<T> liste, Object o)
        {
            try
            {
                if (liste.IsNull()) return false;
                return liste.IsNotEmpty() && liste.Any(e => e.Equals(o));
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Récupère le nombre d'élément sans compter les éléments du tableau
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="liste"></param>
        /// <param name="tab">2léments à exclure</param>
        /// <returns>int</returns>
        public static int CountWithoutElem<T>(this IEnumerable<T> liste, Object[] tab = null)
        {
            var enumerable = liste as T[] ?? liste.ToArray();
            if (!enumerable.Any()) return 0;
            if (tab != null && tab.Length == 0) return enumerable.Length;

            int count = 0;
            if (tab == null) return count;
            foreach (var o in enumerable)
            {
                bool verif = false;
                foreach (var l in tab.Where(l => l.Equals(o)))
                {
                    verif = true;
                }

                if (verif) continue;
                count++;
            }

            return count;
        }

        /// <summary>
        /// Déplace un object de la liste à une nouvelle position
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="liste"></param>
        /// <param name="o">Object à déplacer</param>
        /// <param name="position">Position</param>
        public static void MoveElement<T>(this List<T> liste, Object o, int position = 0)
        {
            if (liste.IsNull() || liste.IsEmpty() || position > liste.Count || position < 0) return;
            //On vérifie que la liste contient l'object
            var b = false;
            // ReSharper disable once UnusedVariable
            foreach (var v in liste.Where(v => v.Equals(o)))
            {
                b = true;
            }

            if (!b) return;

            //Si la liste contient l'object à déplacer, on fait le boulot

            liste.Remove((T)o);
            liste.Insert(position, (T)o);
        }

        public static bool RemoveElement<T>(this List<T> liste, T o)
        {
            try
            {
                if (liste.IsNull() || liste.IsEmpty()) return false;
                if (!liste.Contains(o)) return false;
                liste.Remove(o);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Supprime les enregistrements de la liste courante en fonction des éléments d'une autre liste
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="other">IEnumerable</param>
        /// <returns>List{T}</returns>
        public static List<T> DeleteDoublonWiteOtherListEquals<T>(this List<T> o, IEnumerable<T> other)
        {
            try
            {
                var b = false;
                var temp = new List<T>();

                foreach (var elem in o)
                {
                    // ReSharper disable once UnusedVariable
                    // ReSharper disable once PossibleMultipleEnumeration
                    // ReSharper disable once AccessToForEachVariableInClosure
                    foreach (var elem2 in other.Where(elem2 => elem2.Equals(elem)))
                    {
                        b = true;
                    }

                    if (!b)
                        temp.Add(elem);

                    b = false;
                }

                return temp;
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }

        /// <summary>
        /// Supprime les enregistrements de la List qui contiennent les éléments du tableau other
        /// </summary>
        /// <param name="o"></param>
        /// <param name="other">Element à supprimer</param>
        /// <returns>List{string}</returns>
        public static List<string> DeleteDoublonContains(this List<string> o, IEnumerable<string> other)
        {
            try
            {
                var b = false;
                var temp = new List<string>();

                foreach (var elem in o)
                {
                    // ReSharper disable once UnusedVariable
                    // ReSharper disable once PossibleMultipleEnumeration
                    // ReSharper disable once AccessToForEachVariableInClosure
                    foreach (var elem2 in other.Where(elem2 => elem.Contains(elem2)))
                    {
                        b = true;
                    }

                    if (!b)
                        temp.Add(elem);
                    b = false;
                }

                return temp;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public static List<string> DeleteDoublonEquals(this List<string> liste, IEnumerable<string> other)
        {
            try
            {
                var b = false;
                var temp = new List<string>();

                foreach (var elem in liste)
                {
                    // ReSharper disable once UnusedVariable
                    // ReSharper disable once PossibleMultipleEnumeration
                    // ReSharper disable once AccessToForEachVariableInClosure
                    foreach (var elem2 in other.Where(elem2 => elem.Equals(elem2)))
                    {
                        b = true;
                    }

                    if (!b)
                        temp.Add(elem);
                    b = false;
                }

                return temp;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Supprimer un élément d'une liste
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="l"></param>
        /// <param name="o">Object</param>
        /// <returns>List{T}</returns>
        public static void DeleteObject<T>(this List<T> l, T o)
        {
            if (l.IsNull() || l.IsEmpty()) return;

            foreach (var obj in l)
            {
                l.Remove(obj);
            }

        }

        /// <summary>
        /// Vérifie si la liste contient le string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="str">string à verifier</param>
        /// <returns>bool</returns>
        public static bool Contains<T>(this List<T> o, string str)
        {
            try
            {
                return o.Any(v => v.Equals(str));
            }
            catch (Exception)
            {
                return true;
            }
        }

        /// <summary>
        /// Récupére l'index de l'objet dans la liste
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="elem">Object</param>
        /// <returns>int</returns>
        public static int IndexElement<T>(this List<T> o, T elem)
        {
            try
            {
                if (o.IsNull() || o.IsEmpty()) return -1;

                int count = 0;
                foreach (var i in o)
                {
                    if (i.Equals(elem))
                        return count;
                    count++;
                }

                return 0;
            }
            catch (Exception)
            {
                return -2;
            }
        }

        /// <summary>
        /// Permet de créer un string qui sépare chaque éléments de la liste par le séparateur
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="l"></param>
        /// <param name="separator">Le séparateur</param>
        /// <returns>string</returns>
        public static string Join<T>(this IEnumerable<T> l, string separator)
        {
            try
            {
                return String.Join(separator, l);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Permet d'ordonner une liste de façon ascendante
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="liste">La liste à ordonner</param>
        public static void TriAscendant<T>(this List<T> liste)
        {
            try
            {
                liste.Sort();
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Permet d'ordonner une liste de façon descendante
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="liste">La liste à ordonner</param>
        public static void TriDescendant<T>(this List<T> liste)
        {
            try
            {
                TriAscendant(liste);
                liste.Reverse();
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        public static List<T> Concat<T>(this List<T> first, List<T> second = null, bool order = true)
        {
            try
            {
                if(second.IsEmpty())
                    return first;

                var newList = new List<T>();
                if (order)
                {
                    newList = first;
                    if(second.IsNotEmpty())
                        newList.AddRange(second);
                }
                else
                {
                    newList = second;
                    if(first.IsNotEmpty())
                        newList.AddRange(first);
                }

                return newList;
            }
            catch (Exception)
            {
                return first;
            }
        }

        /// <summary>
        /// Permet de créer une pagination
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="liste">La liste d'origine</param>
        /// <param name="nbPerPage">Le nombre d'élément par page</param>
        /// <returns>List{List{T}}</returns>
        public static List<List<T>> MakePagination<T>(this List<T> liste, int nbPerPage = 0)
        {
            try
            {
                if (nbPerPage.IsDefault()) return new List<List<T>> { liste };
                if (liste.IsNull()) return new List<List<T>> { liste };
                if (liste.IsEmpty()) return new List<List<T>> { liste };

                //Init des variables
                var count = 0;
                var newListe = new List<List<T>>();
                var index = -1;

                //On remplit la liste
                foreach (var obj in liste)
                {
                    if (count == 0)
                    {
                        newListe.Add(new List<T>());
                        index++;
                    }
                    newListe[index].Add(obj);
                    count++;
                    if (count >= nbPerPage)
                        count = 0;
                }
                return newListe;
            }
            catch (Exception)
            {
                return new List<List<T>> { liste };
            }
        }

        /// <summary>
        /// Permet de savoir si un objet est le dernier de la liste
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="liste"></param>
        /// <param name="o"></param>
        /// <returns>bool</returns>
        public static bool CheckLastElement<T>(this List<T> liste, T o)
        {
            try
            {
                if (!liste.Any() || o.IsNull()) return false;
                return liste.GetLastElement().Equals(o);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static List<T> CreateSubListWithInterval<T>(this List<T> liste, int depart = -1, int interval = -1)
        {
            try
            {
                if (liste.IsEmpty() || depart.Equals(-1) || interval.Equals(-1)) return new List<T>();
                var subListe = new List<T>();

                while (liste.GetElementViaIndex(depart) != null)
                {
                    subListe.Add((T) liste.GetElementViaIndex(depart));
                    depart += interval;
                }

                return subListe;
            }
            catch (Exception e)
            {
                var t = e.Message;
                return new List<T>();
            }
        }
    }
}
