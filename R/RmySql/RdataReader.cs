using System;
using MySql.Data.MySqlClient;

// ReSharper disable once CheckNamespace
namespace R
{
    public static class RdataReader
    {
        /// <summary>
        /// Permet de récupérer la valeur d'un champ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataReader"></param>
        /// <param name="name">Nom du champ</param>
        /// <param name="type">Type du champ</param>
        /// <returns>T</returns>
        public static T GetValue<T>(this MySqlDataReader dataReader, string name, string type)
        {
            try
            {
                switch (type)
                {
                    case "string":
                        return (T)Convert.ChangeType(dataReader.GetString(name), typeof(T));
                    case "int":
                        return (T)Convert.ChangeType(dataReader.GetInt32(name), typeof(T));
                    case "double":
                        return (T)Convert.ChangeType(dataReader.GetDouble(name), typeof(T));
                    case "float":
                        return (T)Convert.ChangeType(dataReader.GetFloat(name), typeof(T));
                    case "DateTime":
                        return (T)Convert.ChangeType(dataReader.GetDateTime(name), typeof(T));
                    default:
                        return default(T);
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}
