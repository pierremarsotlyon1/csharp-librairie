using System;
using System.IO;
using System.IO.Compression;

// ReSharper disable once CheckNamespace
namespace R
{
    public static class ZipArchive
    {
        /// <summary>
        /// Permet de zipper un fichier
        /// </summary>
        /// <param name="cheminSrc">Le chemin du fichier</param>
        /// <param name="cheminDest">Le chemin du zip</param>
        /// <param name="action">Action à faire</param>
        /// <returns>bool</returns>
        public static bool ToZip(this string cheminSrc, string cheminDest = null, Action action = null)
        {
            try
            {
                if (cheminDest.IsNullOrEmpty()) return false;
                ZipFile.CreateFromDirectory(cheminSrc, cheminDest);
                if (action.IsNotNull())
                    action();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permet de dézipper un fichier
        /// </summary>
        /// <param name="cheminSrc">Le chemin du zip</param>
        /// <param name="cheminDest">Le chemin du fichier</param>
        /// <param name="action">L'action à faire</param>
        /// <returns></returns>
        public static bool ToFile(this string cheminSrc, string cheminDest = null, Action action = null)
        {
            try
            {
                if (cheminDest.IsNullOrEmpty()) return false;
                if (RFichier.FileNotExist(cheminDest))
                {
                    RFichier.CreerFichier(cheminDest);
                }
                using (var archive = ZipFile.OpenRead(cheminSrc))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        entry.ExtractToFile(Path.Combine(cheminDest, entry.FullName));
                    }
                }
                if (action.IsNotNull())
                    action();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
