using System;
using System.Collections.Generic;
using System.Linq;
using R.Web;
using R.Youtube;

// ReSharper disable once CheckNamespace
namespace R
{
    public class GestionYoutube
    {
        private readonly Dictionary<string, string> _listeUrl;
        private readonly string _cheminYoutube;

        public GestionYoutube(Dictionary<string, string> url)
        {
            _listeUrl = url;
            if (_listeUrl.IsEmpty()) return;
            _cheminYoutube = @"D:\Dropbox\IA\IA\bin\Debug\youtube\";
            //Création des xml
            foreach (var node in _listeUrl)
            {
                if (RFichier.FileNotExist(_cheminYoutube + node.Key + ".xml"))
                    RFichier.CreerFichier(_cheminYoutube + node.Key + ".xml");
            }
        }

        public event Action<List<Video>, string> NewVideo;
        public event Action<string> Erreur;

        public bool RechercheVideo(int time = 1000)
        {
            try
            {
                if (_listeUrl.IsEmpty()) return false;

                RThread.DoAsync(() =>
                {
                    var firstPassage = true;
                    while (true)
                    {
                        if (firstPassage.IsFalse())
                            time.SleepToMin();
                        firstPassage = false;

                        foreach (var url in _listeUrl)
                        {
                            if (url.Value.IsNullOrEmpty()) continue;

                            //Chargement de la page
                            var document = url.Value.RloadUrl();
                            if (document.IsNull()) continue;

                            //Get des li des vidéos
                            var listeVideos =
                                document.DocumentNode.SelectNodes("//*[@id='channels-browse-content-grid']//li");
                            if (listeVideos.IsNull()) continue;

                            //On récup les anciennes vidéos de l'artiste
                            var oldListeVideo = (_cheminYoutube + url.Key + ".xml").FileXmlUnserialize<List<Video>>();
                            var newListeVideo = (from video in listeVideos
                                                 where !video.IsNull()
                                                 select video.SelectSingleNode(".//h3[@class='yt-lockup-title']/a[@class='yt-uix-sessionlink yt-uix-tile-link  spf-link  yt-ui-ellipsis yt-ui-ellipsis-2']")
                                                     into nodeUrl
                                                     where !nodeUrl.IsNull()
                                                     let attrHref = nodeUrl.Attributes["href"]
                                                     where !attrHref.IsNull()
                                                     let href = "https://www.youtube.com" + attrHref.Value
                                                     let titre = nodeUrl.InnerText
                                                     select new Video
                                                     {
                                                         Titre = titre,
                                                         Url = href
                                                     }).ToList();

                            //On récup les nouvelles vidéos

                            if (newListeVideo.IsEmpty()) continue;

                            //On compare pour voir les vidéos à ne pas citer
                            var listeTemp = new List<Video>();
                            if (oldListeVideo.IsNotEmpty())
                            {
                                listeTemp.AddRange(
                                    newListeVideo.Where(video => !video.IsNull())
                                        .Where(
                                            video =>
                                                oldListeVideo.Where(oldVideo => !oldVideo.IsNull())
                                                    .Any(oldVideo => oldVideo.Url.Equals(video.Url))));
                            }

                            //On assigne la nouvelle liste des vidéos
                            oldListeVideo = newListeVideo.Clone();

                            //On supprime les vidéos déjà présentes
                            foreach (var video in listeTemp)
                            {
                                newListeVideo.RemoveElement(video);
                            }

                            //On écrit dans le fichier pour save
                            if (oldListeVideo.XmlSerialize(_cheminYoutube + url.Key + ".xml").IsFalse())
                            {
                                OnErreur("Erreur lors de la sauvegarde des vidéos youtube de " + url.Key + ".");
                                continue;
                            }

                            if (newListeVideo.IsEmpty()) continue;
                            //On déclenche l'event
                            OnNewVideo(newListeVideo, url.Key);
                        }
                    }
                    // ReSharper disable once FunctionNeverReturns
                });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected virtual void OnNewVideo(List<Video> obj, string artiste)
        {
            var handler = NewVideo;
            if (handler != null) handler(obj, artiste);
        }

        protected virtual void OnErreur(string obj)
        {
            var handler = Erreur;
            if (handler != null) handler(obj);
        }
    }
}