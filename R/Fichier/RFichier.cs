using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

// ReSharper disable once CheckNamespace

namespace R
{
    public class RFichier
    {
        public string Fichier { get; set; }
        public bool VerifFichier;

        public RFichier()
        {
        }

        /// <summary>
        ///     Constructeur
        /// </summary>
        /// <param name="fichier">Chemin du fichier</param>
        public RFichier(string fichier)
        {
            string strAppDir = DirectoryApp();
            if (strAppDir != null)
            {
                string strFullPathToMyFile = Path.Combine(strAppDir, fichier);

                if (FileExist(strFullPathToMyFile))
                {
                    Fichier = strFullPathToMyFile;
                    VerifFichier = true;
                }
                else
                {
                    VerifFichier = CreerFichier(fichier);
                    if (VerifFichier)
                        Fichier = strFullPathToMyFile;
                }
            }
            else
            {
                VerifFichier = false;
            }
        }

        /// <summary>
        ///     Permet de lire un fichier
        /// </summary>
        /// <returns>string</returns>
        public string LireLeFichier()
        {
            try
            {
                if (!VerifFichier) return null;

                var monStreamReader = new StreamReader(Fichier);
                string text = null;
                string ligne;
                // Lecture de toutes les lignes et affichage de chacune sur la page 
                while ((ligne = monStreamReader.ReadLine()) != null)
                {
                    text += ligne + "\n";
                }
                // Fermeture du StreamReader
                monStreamReader.Close();

                return text;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool ClearFile(string fichier = null)
        {
            try
            {
                string f = null;
                f = fichier.IsNotNullOrEmpty() ? fichier : Fichier;

                if (f.IsNullOrEmpty()) return false;
                File.WriteAllText(f, String.Empty);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Permet d'écrire dans un fichier avec un saut de ligne pour chaques éléments du tableau
        /// </summary>
        /// <param name="tab">Tableau de string à écrire</param>
        /// <param name="append">Ecrire à la fin du fichier</param>
        /// <returns>bool</returns>
        public bool EcritureFichier(string[] tab, bool append = false)
        {
            try
            {
                if (!VerifFichier) return false;
                if (tab.IsEmpty()) return false;

                //Ecriture du text dans le fichier
                if (append)
                    File.AppendAllLines(Fichier, tab);
                else
                    File.WriteAllLines(Fichier, tab);
                //On écrit une retour chariot
                File.AppendAllLines(Fichier, new[] {"\n\r"});
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EcritureFichier(byte[] data)
        {
            try
            {
                if (!VerifFichier) return false;

                File.WriteAllBytes(Fichier, data);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EcritureFichier(byte[] data, string fichier)
        {
            try
            {
                File.WriteAllBytes(fichier, data);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Récupére la taille en Octet du fichier
        /// </summary>
        /// <returns>int</returns>
        public int RecupererTailleFichier()
        {
            try
            {
                if (!File.Exists(Fichier)) return 0;

                var infoFichier = new FileInfo(Fichier);
                //En octet
                return int.Parse(infoFichier.Length.ToString(CultureInfo.InvariantCulture));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        ///     Création d'un fichier
        /// </summary>
        /// <param name="fichier">Url du fichier</param>
        /// <returns>bool</returns>
        public static bool CreerFichier(string fichier)
        {
            try
            {
                FileStream file = File.Create(fichier);
                file.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Obtient le chemin du projet
        /// </summary>
        /// <returns>string</returns>
        public static string DirectoryApp()
        {
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            return directoryName != null ? directoryName.Replace("file:\\", string.Empty) : null;
        }

        /// <summary>
        ///     Permet de récupérer l'extension d'un fichier
        /// </summary>
        /// <param name="fichier">Le chemin du fichier</param>
        /// <returns>string</returns>
        public static string GetExtension(string fichier)
        {
            return Path.GetExtension(fichier);
        }

        /// <summary>
        ///     Permet d'obtenir le chemin d'un fichier à partir du chemin du projet
        /// </summary>
        /// <param name="nameFile">Nom du fichier</param>
        /// <returns>string</returns>
        public static string DirectoryAppWithFile(string nameFile)
        {
            return DirectoryApp() + "\\" + nameFile;
        }

        public static byte[] GetByte(string str)
        {
            return File.ReadAllBytes(str);
        }

        /// <summary>
        ///     Permet de récupérer les fichier d'un dossier
        /// </summary>
        /// <param name="str">Le chemin du fichier</param>
        /// <returns>DirectoryInfo{}</returns>
        public static FileInfo[] GetFiles(string str)
        {
            try
            {
                return new DirectoryInfo(str).GetFiles();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<FileInfo> GetAllFiles(string str, string[] exception = null)
        {
            try
            {
                if (str.IsNullOrEmpty()) return null;
                //Get la liste des fichiers du rep courant
                var listetemp = GetFiles(str);
                if (listetemp.IsNull()) return null;
                var liste = listetemp.ToList();

                //Get la liste des dossier du rep courant
                var folder = GetDirectories(str);
                if (folder.IsEmpty()) return liste;

                foreach (var f in folder)
                {
                    if (f.FullName.IsNullOrEmpty()) continue;
                    if (exception.IsNotNull())
                    {
                        if (f.FullName.InArray(exception))
                            continue;
                    }
                    var temp = GetAllFiles(f.FullName);
                    if (temp.IsNull() || temp.IsEmpty()) continue;
                    liste.AddRange(temp.ToList());
                }

                return liste;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Permet de récupérer les sous-dossiers d'un dossier
        /// </summary>
        /// <param name="str">Le chemin du dossier</param>
        /// <returns>DirectoryInfo{}</returns>
        public static DirectoryInfo[] GetDirectories(string str)
        {
            try
            {
                return new DirectoryInfo(str).GetDirectories();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Permet de supprimer une liste de fichier
        /// </summary>
        /// <param name="tabFichier">La liste de fichier</param>
        public static bool DeleteFilesInDirectory(FileInfo[] tabFichier)
        {
            try
            {
                if (tabFichier.IsNull() || tabFichier.Length == 0) return false;

                foreach (FileInfo fichier in tabFichier)
                {
                    try
                    {
                        File.Delete(fichier.FullName);
                    }
                        // ReSharper disable once EmptyGeneralCatchClause
                    catch (Exception)
                    {
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
        ///     Permet de supprimer touts les fichiers d'un dossier
        /// </summary>
        /// <param name="chemin">Le chemin du fichier</param>
        /// <returns>bool</returns>
        public static bool DeleteDirectory(string chemin)
        {
            try
            {
                //On récup les fichiers
                FileInfo[] listeFiles = GetFiles(chemin);
                //On récup les dossiers
                DirectoryInfo[] listeDossier = GetDirectories(chemin);
                //On supprime les fichiers
                if (listeFiles.IsNotNull() && listeFiles.Length > 0)
                {
                    DeleteFilesInDirectory(listeFiles);
                }
                //On rappel la méthode pour les sous-dossiers
                foreach (DirectoryInfo directoryInfo in listeDossier)
                {
                    DeleteDirectory(directoryInfo.FullName);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Permet de vérifier si un fichier existe
        /// </summary>
        /// <param name="file">Le chemin du fichier</param>
        /// <returns>bool</returns>
        public static bool FileExist(string file = null)
        {
            try
            {
                if (file.IsNullOrEmpty()) return false;
                return File.Exists(file);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Permet de vérifier si un fichier n'existe oas
        /// </summary>
        /// <param name="file">Le chemin du fichier</param>
        /// <returns>bool</returns>
        public static bool FileNotExist(string file = null)
        {
            try
            {
                if (file.IsNullOrEmpty()) return false;
                return !FileExist(file);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Permet de supprimer un fichier
        /// </summary>
        /// <param name="fichier">Le chemin du fichier</param>
        /// <returns>string</returns>
        public static bool DeleteFile(string fichier = null)
        {
            try
            {
                if (fichier.IsNullOrEmpty() || FileNotExist(fichier)) return false;
                // ReSharper disable once AssignNullToNotNullAttribute
                File.Delete(fichier);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Permet de récupérer la date de création d'un fichier
        /// </summary>
        /// <param name="fichier">Le chemin du fichier</param>
        /// <returns>string</returns>
        public static string DateCreation(string fichier = null)
        {
            try
            {
                if (fichier.IsNullOrEmpty() || FileNotExist(fichier)) return null;
                // ReSharper disable once AssignNullToNotNullAttribute
                var monFichier = new FileInfo(fichier);
                return monFichier.IsNull() ? null : monFichier.CreationTime.ToLongDateString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Permet de récupérer la date de la dernière modification du fichier
        /// </summary>
        /// <param name="fichier">Le chemin du fichier</param>
        /// <returns>string</returns>
        public static DateTime DateLastUpdate(string fichier = null)
        {
            try
            {
                if (fichier.IsNullOrEmpty() || FileNotExist(fichier)) return new DateTime();
                // ReSharper disable once AssignNullToNotNullAttribute
                var monFichier = new FileInfo(fichier);
                return monFichier.IsNull() ? new DateTime() : monFichier.LastWriteTime;
            }
            catch (Exception)
            {
                return new DateTime();
            }
        }

        /// <summary>
        ///     Permet de récupérer la date du dernier acces au fichier
        /// </summary>
        /// <param name="fichier">Le chemin du fichier</param>
        /// <returns>string</returns>
        public static string DateLastAcces(string fichier = null)
        {
            try
            {
                if (fichier.IsNullOrEmpty() || FileNotExist(fichier)) return null;
                // ReSharper disable once AssignNullToNotNullAttribute
                var monFichier = new FileInfo(fichier);
                return monFichier.IsNull() ? null : monFichier.LastAccessTime.ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Permet de cacher un fichier
        /// </summary>
        /// <param name="fichier">Le chemin du fichier</param>
        /// <returns>string</returns>
        public static bool ToHidden(string fichier = null)
        {
            try
            {
                if (fichier.IsNullOrEmpty() || FileNotExist(fichier)) return false;
                // ReSharper disable once AssignNullToNotNullAttribute
                File.SetAttributes(fichier, FileAttributes.Hidden);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool MoveFilm(string chemin = null)
        {
            try
            {
                if (chemin.IsNullOrEmpty()) return false;

                DirectoryInfo[] dossier = GetDirectories(chemin);
                if (dossier.IsEmpty()) return false;

                foreach (FileInfo fileInfo in from directoryInfo in dossier
                    where !directoryInfo.IsNull()
                    select GetFiles(directoryInfo.FullName)
                    into files
                    where !files.IsEmpty()
                    from fileInfo in files
                    where !fileInfo.IsNull() && !fileInfo.Extension.IsNullOrEmpty()
                    where fileInfo.Extension.Contains("mkv")
                    select fileInfo)
                {
                    File.Move(fileInfo.FullName, chemin + "\\" + fileInfo.Name);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool Rename(FileInfo fileInfo, string newName)
        {
            try
            {
                if (fileInfo.IsNull() || newName.IsNullOrEmpty()) return false;
                if (fileInfo.Directory.IsNull()) return false;
                fileInfo.MoveTo(fileInfo.Directory.FullName + "\\" + newName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}