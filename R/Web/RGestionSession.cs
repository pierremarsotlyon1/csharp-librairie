using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.SessionState;

// ReSharper disable once CheckNamespace

namespace R
{
    public static class RGestionSession
    {
        /// <summary>
        ///     Permet d'ajouter une variable à la Session
        /// </summary>
        /// <param name="key">Clef de la variable</param>
        /// <param name="value">Valeur de la variable</param>
        /// <returns>bool</returns>
        public static bool Add<T>(string key, T value)
        {
            try
            {
                HttpSessionState session = RecupSession();
                if (session == null) return false;
                if (Check(key))
                    return Update(key, value);
                session.Add(key, value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Permet de supprimer une variable de la Session
        /// </summary>
        /// <param name="tab">Tableau de clef à supprimer</param>
        /// <returns>bool</returns>
        public static bool Delete(string[] tab)
        {
            try
            {
                HttpSessionState session = RecupSession();
                if (session == null) return false;
                {
                    foreach (string key in from key in tab let temp = key where Check(temp) select key)
                    {
                        session.Remove(key);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Permet de supprimer une variable de la Session
        /// </summary>
        /// <param name="tab">Tableau de clef à supprimer</param>
        /// <returns>bool</returns>
        public static bool Delete(string str)
        {
            try
            {
                if (str.IsNullOrEmpty()) return false;

                HttpSessionState session = RecupSession();
                if (session == null) return false;
                
                session.Remove(str);
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Permet de récupèrer une variable de Session
        /// </summary>
        /// <typeparam name="T">Type de retour</typeparam>
        /// <param name="key">Cle de la variable</param>
        /// <returns>T</returns>
        public static T Get<T>(string key)
        {
            try
            {
                HttpSessionState session = RecupSession();
                if (!Check(key) || session == null) return default(T);
                object objet = session[key];
                return (T)objet;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        ///     Permet de modifier une variable de Session
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Clef de la variable</param>
        /// <param name="value">Valeur de la variable</param>
        /// <returns>bool</returns>
        public static bool Update<T>(string key, T value)
        {
            try
            {
                if (!Check(key)) return false;
                HttpSessionState session = RecupSession();

                //Type type = session[key].GetType();
                //if (type != typeof(T)) return false;
                session[key] = value;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Permet de supprimer toutes les variables de Session
        /// </summary>
        /// <returns>bool</returns>
        public static bool ClearSession()
        {
            try
            {
                HttpSessionState session = RecupSession();
                session.Clear();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Permet de supprimer toutes les variables de Session en gardant celles qui sont dans le tableau
        /// </summary>
        /// <returns>bool</returns>
        public static bool ClearSession(string[] variablesToKeep)
        {
            try
            {
                //On récup la session
                HttpSessionState session = RecupSession();
                //On récup la liste des variables de la session
                var listeKeys = session.Keys;
                //On compare les deux tableaux
                var liste = new List<string>();
                foreach (var listeKey in listeKeys)
                {
                    var check = false;
                    object key = listeKey;
                    // ReSharper disable once UnusedVariable
                    foreach (var s in variablesToKeep.Where(s => s.Equals(key.ToString())))
                    {
                        check = true;
                    }

                    if (!check)
                        liste.Add(listeKey.ToString());
                }

                //On supprime les variables de session
                foreach (var varSession in liste)
                {
                    session.Remove(varSession);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Permet de récupèrer la Session
        /// </summary>
        /// <returns>HttpSessionState</returns>
        private static HttpSessionState RecupSession()
        {
            try
            {
                return HttpContext.Current.Session;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Permet de vérifier si une clef est dans la Session
        /// </summary>
        /// <param name="key">Clef à vérifier</param>
        /// <returns>bool</returns>
        public static bool Check(string key)
        {
            try
            {
                HttpSessionState session = RecupSession();
                if (session.IsNull()) return false;
                NameObjectCollectionBase.KeysCollection listeKeys = session.Keys;
                var x = listeKeys.Cast<object>();
                return x.Contains(key);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public class SessionMysql
    {
        [Rsqlgnore(true)]
        public int Id { get; set; }

        [RsqlInt(true)]
        public int IdSession;

        [RsqlText(true)]
        public string Serialize { get; set; }
    }
}