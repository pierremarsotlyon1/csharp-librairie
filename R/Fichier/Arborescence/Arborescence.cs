using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Markup;
using R.Fichier.Arborescence;

namespace R
{
    public class Arborescence
    {
        public List<Base> ArborescenceFile { get; set; }
        public List<Dossier> ListeEtages { get; set; }
        public string[] FilterExtension { get; set; }
        private readonly bool _convertToByte;

        public Arborescence(string rootPath, string[] filterExtension = null, bool convertToByte = false)
        {
            ArborescenceFile = new List<Base>();
            ListeEtages = new List<Dossier>();
            FilterExtension = filterExtension;
            _convertToByte = false;
            AddPath(rootPath);
        }

        /// <summary>
        /// Permet d'ajouter un dossier à l'arborescence
        /// </summary>
        /// <param name="path">Le chemin du dossier</param>
        /// <returns>bool</returns>
        public bool AddPath(string path)
        {
            try
            {
                if (path.IsNullOrEmpty()) return false;
                Scaner(ref path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permet de descendre d'un étage
        /// </summary>
        /// <param name="objBase">Le dossier de provenance</param>
        /// <returns>List{Base}</returns>
        public List<Base> DownEtage(Dossier objBase = null)
        {
            try
            {
                if (objBase.IsNull()) return null;
                ListeEtages.Add(objBase);
                return SearchEtage();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Permet de remonter d'un étage
        /// </summary>
        /// <returns>List{Base}</returns>
        public List<Base> UpEtage()
        {
            try
            {
                if (!ListeEtages.Any()) return null;
                if (!ListeEtages.RemoveLastElement()) return null;
                return SearchEtage();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Permet de chercher un étage
        /// </summary>
        /// <returns>List{Base}</returns>
        private List<Base> SearchEtage()
        {
            try
            {
                Dossier dossier = null;
                if (!ListeEtages.Any())
                    //On est au RDC
                    return ArborescenceFile.ToList();

                foreach (var etage in ListeEtages)
                {
                    int index = -1;
                    if (dossier.IsNull())
                    {
                        index = ArborescenceFile.IndexElement(etage);
                        dossier = (Dossier)ArborescenceFile[index];
                    }
                    else
                    {
                        index = dossier.listeFichier.IndexElement(etage);
                        dossier = (Dossier)dossier.listeFichier[index];
                    }
                }
                return dossier.listeFichier.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        private void Scaner(ref string path)
        {
            //On récup les fichiers
            var listeFichier = RFichier.GetFiles(path);
            //On récup les dossier
            var listeDossier = RFichier.GetDirectories(path);

            foreach (var fichier in listeFichier.Where(fichier => !CheckExtension(fichier)))
            {
                ArborescenceFile.Add(_convertToByte
                    ? new Fichier.Arborescence.Fichier
                    {
                        IsFile = true,
                        Name = fichier.Name,
                        Path = fichier.FullName,
                        Extension = fichier.Extension,
                        ByteFile = RFichier.GetByte(fichier.FullName)
                    }
                    : new Fichier.Arborescence.Fichier
                    {
                        IsFile = true,
                        Name = fichier.Name,
                        Path = fichier.FullName,
                        Extension = fichier.Extension
                    });
            }

            foreach (var dossier in listeDossier)
            {
                ArborescenceFile.Add(new Dossier { IsFile = false, Name = dossier.Name, Path = dossier.FullName }, 0);
            }

            foreach (var @base in ArborescenceFile.Where(@base => !@base.IsFile))
            {
                ScanerDossier((Dossier)@base);
            }
        }

        private void ScanerDossier(Dossier dossier)
        {
            //On récup les fichiers
            var listeFichier = RFichier.GetFiles(dossier.Path);
            //On récup les dossier
            var listeDossier = RFichier.GetDirectories(dossier.Path);

            foreach (var fichier in listeFichier.Where(fichier => !CheckExtension(fichier)))
            {
                dossier.listeFichier.Add(_convertToByte
                    ? new Fichier.Arborescence.Fichier
                    {
                        IsFile = true,
                        Name = fichier.Name,
                        Path = fichier.FullName,
                        Extension = fichier.Extension,
                        ByteFile = RFichier.GetByte(fichier.FullName)
                    }
                    : new Fichier.Arborescence.Fichier
                    {
                        IsFile = true,
                        Name = fichier.Name,
                        Path = fichier.FullName,
                        Extension = fichier.Extension
                    });
            }
            foreach (var dossier2 in listeDossier)
            {
                dossier.listeFichier.Add(new Dossier { IsFile = false, Name = dossier.Name, Path = dossier2.FullName }, 0);
            }

            foreach (var x in dossier.listeFichier.Where(x => !x.IsFile))
            {
                ScanerDossier((Dossier)x);
            }
        }

        /// <summary>
        /// Permet de vérifier si un fichier posséde l'extension
        /// </summary>
        /// <param name="fichier"></param>
        /// <returns></returns>
        private bool CheckExtension(FileInfo fichier)
        {
            try
            {
                if (fichier.IsNull() || fichier.Extension.IsNullOrEmpty() || !FilterExtension.Any()) return false;
                return FilterExtension.Any(extension => fichier.Extension.Contains(extension));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<Base> RechercheFichierByKeyWord(string motclef = null, Dossier folder = null)
        {
            try
            {
                if (motclef.IsNullOrEmpty()) return null;
                motclef = motclef.ToLower();
                var listeBase = new List<Base>();
                var arboAConsulter = folder.IsNull() ? ArborescenceFile : folder.listeFichier;
                foreach (var @base in arboAConsulter)
                {
                    //C'est un fichier
                    if (@base.IsFile)
                    {
                        if (!@base.Name.IsNotNullOrEmpty()) continue;
                        var name = @base.Name.ToLower();
                        if (name.Contains(motclef))
                            listeBase.Add(@base);
                    }
                    else
                    {
                        //C'est un dossier, on rapelle la fonction
                        var listeTemp = RechercheFichierByKeyWord(motclef, (Dossier) @base);
                        if (listeTemp.IsNull() || listeTemp.IsEmpty()) continue;
                        listeBase.AddRange(listeTemp);
                    }
                }
                return listeBase;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
