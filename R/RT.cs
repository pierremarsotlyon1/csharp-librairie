using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace R
{
    public static class Rt
    {
        public static T Clone<T>(this T source)
        {
            try
            {
                if (!typeof(T).IsSerializable)
                {
                    return default(T);
                }

                // Don't serialize a null object, simply return the default for that object
                if (Object.ReferenceEquals(source, null))
                {
                    return default(T);
                }

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new MemoryStream();
                using (stream)
                {
                    formatter.Serialize(stream, source);
                    stream.Seek(0, SeekOrigin.Begin);
                    return (T)formatter.Deserialize(stream);
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }


        /// <summary>
        ///     Vérifie si l'objet est un double
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">L'objet</param>
        /// <returns>bool</returns>
        public static bool IsDouble<T>(this T obj)
        {
            try
            {
                return obj is string;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Vérifie si l'objet n'est pas un double
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">L'objet</param>
        /// <returns>bool</returns>
        public static bool IsNotDouble<T>(this T obj)
        {
            try
            {
                return !obj.IsDouble();
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Vérifie si l'objet est un string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">L'objet</param>
        /// <returns>bool</returns>
        public static bool IsString<T>(this T obj)
        {
            try
            {
                return obj is string;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Vérifie si l'objet n'est pas un string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">L'objet</param>
        /// <returns>bool</returns>
        public static bool IsNotString<T>(this T obj)
        {
            try
            {
                return !obj.IsString();
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Vérifie si l'objet est un int
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">L'objet</param>
        /// <returns>bool</returns>
        public static bool IsInt<T>(this T obj)
        {
            try
            {
                return obj is int;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Vérifie si l'objet n'est pas un int
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">L'objet</param>
        /// <returns>bool</returns>
        public static bool IsNotInt<T>(this T obj)
        {
            try
            {
                return !obj.IsInt();
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Vérifie si l'objet est un float
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">L'objet</param>
        /// <returns>bool</returns>
        public static bool IsFloat<T>(this T obj)
        {
            try
            {
                return obj is float;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Vérifie si l'objet n'est pas un float
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">L'objet</param>
        /// <returns>bool</returns>
        public static bool IsNotFloat<T>(this T obj)
        {
            try
            {
                return !obj.IsFloat();
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Récupère les propriétés de l'objet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns>PropertyInfo[]</returns>
        public static PropertyInfo[] GetPropertyInfo<T>(this T o)
        {
            try
            {
                return o.Equals(null) ? null : o.GetType().GetProperties();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Permet de récupèrer les propriétés d'une propriété de classe
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="name">No de la propriété</param>
        /// <returns>PropertyInfo</returns>
        public static PropertyInfo GetPropertieByName<T>(this T o, string name)
        {
            try
            {
                return o.GetType().GetProperty(name);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Récupère tout les attributs d'une classe
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns>Dictionary{string, object}</returns>
        public static Dictionary<string, object> GetAttributAllClass<T>(this T o)
        {
            try
            {
                var attribs = new Dictionary<string, object>();

                foreach (PropertyInfo property in o.GetPropertyInfo())
                {
                    // look for attributes that takes one constructor argument
                    AttribuerValueDictionary(ref attribs, property);
                }

                return attribs;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Récupère les propriétés d'une attribut
        /// </summary>
        /// <param name="property"></param>
        /// <returns>Dictionary{string, object}</returns>
        public static Dictionary<string, object> GetAttribut(this PropertyInfo property)
        {
            try
            {
                var attribs = new Dictionary<string, object>();
                // look for attributes that takes one constructor argument
                AttribuerValueDictionary(ref attribs, property);

                return attribs;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Permet d'ajouter les valeurs d'un attribut au Dictionary
        /// </summary>
        /// <param name="attribs"></param>
        /// <param name="property">La PropertyInfo</param>
        private static void AttribuerValueDictionary(ref Dictionary<string, object> attribs, PropertyInfo property)
        {
            foreach (CustomAttributeData attribData in property.GetCustomAttributesData())
            {
                if (attribData.ConstructorArguments.Count != 1) continue;
                if (attribData.Constructor.DeclaringType == null) continue;

                string typeName = attribData.Constructor.DeclaringType.Name;
                if (typeName.EndsWith("Attribute")) typeName = typeName.Substring(0, typeName.Length - 9);
                attribs[typeName] = attribData.ConstructorArguments[0].Value;
            }
        }

        /// <summary>
        ///     Permet de récupérer les noms des propriétés d'une classe
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns>List{string}</returns>
        public static List<string> GetNameProperties<T>(this T o)
        {
            try
            {
                List<string> list = o.GetPropertyInfo().Select(prop => prop.Name).ToList();
                return o.Equals(null) ? null : list;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Permet de récupèrer le nom d'une propriété
        /// </summary>
        /// <param name="p"></param>
        /// <returns>string</returns>
        public static string NameProperty(this PropertyInfo p)
        {
            try
            {
                return p.PropertyType.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Vérifie si un objet est null
        /// </summary>
        /// <param name="o">L'object</param>
        /// <returns>bool</returns>
        public static bool IsNull(this object o)
        {
            return CheckIsObjectIsNull(ref o);
        }

        /// <summary>
        ///     Vérifie si un objet n'est pas null
        /// </summary>
        /// <param name="o">L'object</param>
        /// <returns>bool</returns>
        public static bool IsNotNull(this object o)
        {
            return !CheckIsObjectIsNull(ref o);
        }

        /// <summary>
        ///     Vérifie si un objet est null
        /// </summary>
        /// <param name="o">L'objet</param>
        /// <returns>bool</returns>
        private static bool CheckIsObjectIsNull(ref object o)
        {
            return o == null;
        }

        /// <summary>
        ///     Converti un objet en tableau de byte
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">L'objet</param>
        /// <returns>byte{}</returns>
        public static byte[] ToByte<T>(this T obj)
        {
            try
            {
                return obj.BinarySerialize();
            }
            catch (Exception)
            {
                return new byte[0];
            }
        }

        /// <summary>
        ///     Permet de parser un objet en int
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">L'objet à parser</param>
        /// <returns>int</returns>
        public static int ToInt<T>(this T obj)
        {
            try
            {
                return obj.ToString().ToInt();
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Permet de parser un objet en double
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>double</returns>
        public static double ToDouble<T>(this T obj)
        {
            try
            {
                return obj.ToString().ToDouble();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        ///     Permet de vérifier si un élément est dans un tableau
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tab">Le tableau</param>
        /// <param name="comparaison">L'élément</param>
        /// <returns>bool</returns>
        public static bool InArray<T>(this T comparaison, IEnumerable<T> tab)
        {
            try
            {
                IList<T> enumerable = tab as IList<T> ?? tab.ToList();
                if (tab.IsNull() || !enumerable.Any()) return false;

                return enumerable.Contains(comparaison);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Libère en mémoire un objet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        // ReSharper disable once RedundantAssignment
        public static void Dispose<T>(ref T obj)
        {
            obj = default(T);
        }

        /// <summary>
        ///     Permet de convertir une classe c# en classe php
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="chemin">Le chemin de sauvegarde du fichier</param>
        /// <returns>bool</returns>
        public static bool ToPhp<T>(this T o, string chemin = null)
        {
            try
            {
                if (o.IsNull() || chemin.IsNullOrEmpty()) return false;
                PropertyInfo[] listePropertie = o.GetPropertyInfo();
                if (!listePropertie.Any()) return false;

                //Header de la classe
                string classe = "<?php \r\n \r\nclass " + o.GetType().Name + "{\r\n \r\n";

                //Ecriture des propiétées
                classe = listePropertie.Aggregate(classe,
                    (current, propertyInfo) => current + ("\tprivate $" + propertyInfo.Name.ToFormatNom() + "; \r\n"));

                //Ecriture du constructeur
                classe += "\r\n \tpublic function " + o.GetType().Name + "() \r\n \t{ \r\n \t}";

                //Ecriture des méthodes magiques get/set
                classe +=
                    "\r\n \r\n \tpublic function __set($key, $value)\r\n \t{\r\n\t\tif(property_exists($this, $key))\r\n\t\t\t$this->$key = $value;\r\n\t}";
                classe +=
                    "\r\n \r\n \tpublic function __get($key, $value)\r\n \t{\r\n\t\tif(property_exists($this, $key))\r\n\t\t\treturn $this->$key;\r\n\t}";

                //Fin de la classe
                classe += "\r\n}\r\n?>";

                //Ecriture de la classe dans le fichier
                var gestionFichier = new RFichier(chemin);
                return gestionFichier.EcritureFichier(new[] { classe });
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static T CastTo<T>(this object obj) where T : class
        {
            try
            {
                return (T) obj;
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}