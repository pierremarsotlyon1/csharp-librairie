using System;
using System.Configuration;

namespace R
{
    public static class RFichierConfig
    {
        /// <summary>
        /// Obtient une variable d'un fichier de configuration
        /// </summary>
        /// <param name="name">Nom de la variable</param>
        /// <returns>string</returns>
        public static string GetVar(this string name)
        {
            return ConfigurationManager.AppSettings[name];
        }

        /// <summary>
        /// Permet d'attribuer une valeur à une variable d'un fichier de configuration
        /// </summary>
        /// <param name="name">Nom de la variable</param>
        /// <param name="value">Valeur de la variable</param>
        /// <returns>int</returns>
        public static int SetVar(this string name, string value)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                name.RemoveVar(ref config);
                name.AddVar(value, ref config);
                config.Save(ConfigurationSaveMode.Modified);
                "appSettings".RefreshSectionConfigManager();
                return 1;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Permet de supprimer une variable d'un fichier de configuration
        /// </summary>
        /// <param name="name">string</param>
        /// <param name="config">Configuration</param>
        /// <returns>int</returns>
        public static int RemoveVar(this string name, ref Configuration config)
        {
            try
            {
                config.AppSettings.Settings.Remove(name);
                return 1;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Permet d'ajouter une variable à un fichier de configuration
        /// </summary>
        /// <param name="name">string</param>
        /// <param name="value">stirng</param>
        /// <param name="config">Configuration</param>
        /// <returns>int</returns>
        public static int AddVar(this string name, string value, ref Configuration config)
        {
            try
            {
                config.AppSettings.Settings.Add(name, value);
                return 1;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Permet de refraichir une section d'un fichier de configuration
        /// </summary>
        /// <param name="name">string</param>
        /// <returns>int</returns>
        public static int RefreshSectionConfigManager(this string name)
        {
            try
            {
                ConfigurationManager.RefreshSection(name);
                return 1;
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
