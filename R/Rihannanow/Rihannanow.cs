using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace R
{
    public class Rihannanow
    {
        public event Action<List<Tour>> NewTour;
        public event Action<string> ErreurRecherche;
        public RFichier GestionFichier;
        private List<Tour> _listeTour;
        private readonly string _nameXmlTour;

        public Rihannanow()
        {
            try
            {
                _nameXmlTour = RFichier.DirectoryAppWithFile("rihannanow.xml");
                GestionFichier = new RFichier(_nameXmlTour);
                if (GestionFichier.VerifFichier.IsFalse())
                {
                    OnErreurRecherche("Le fichier des concerts est introuvable.");
                    return;
                }
                _listeTour = _nameXmlTour.FileXmlUnserialize<List<Tour>>();
            }
            catch (Exception)
            {
                // ReSharper disable once RedundantJumpStatement
                return;
            }
        }

        public bool RechercheTour(string lienSearch, int timerSearch = 6000)
        {
            try
            {
                if (lienSearch.IsNullOrEmpty())
                {
                    OnErreurRecherche("Impossible de rechercher les concerts de rihanna.");
                    return false;
                }

                RThread.DoAsync(() =>
                {
                    var firstPassage = true;
                    var globalListeTour = new List<Tour>();

                    while (true)
                    {
                        if (!firstPassage)
                            timerSearch.SleepToMin();
                        firstPassage = false;

                        //On charge la page
                        TourJson tour;
                        using (var w = new WebClient())
                        {
                            // attempt to download JSON data as a string
                            try
                            {
                                var jsonData = w.DownloadString(lienSearch);
                                tour = !string.IsNullOrEmpty(jsonData)
                                    ? JsonConvert.DeserializeObject<TourJson>(jsonData)
                                    : new TourJson();
                            }
                            catch (Exception)
                            {
                                OnErreurRecherche("Erreur lors de l'importation des données du site.");
                                continue;
                            }
                        }

                        //Check données
                        if (tour.IsNull() || tour.Data.IsEmpty()) continue;

                        //On convertit le json en object Tour
                        foreach (var t in tour.Data)
                        {
                            if (t.IsNull()) continue;
                            Tour a = new Tour();
                            foreach (var v in t)
                            {
                                if (v.Key.Equals("title"))
                                    a.Title = v.Value;
                                else if (v.Key.Equals("day"))
                                    a.Day = v.Value;
                                else if (v.Key.Equals("month"))
                                    a.Month = v.Value;
                                else if (v.Key.Equals("location"))
                                    a.Lieu = v.Value;
                            }
                            globalListeTour.Add(a);
                        }
                        if (globalListeTour.IsEmpty()) continue;

                        //On cherche les concerts à supprimer
                        List<Tour> concertSupprimer = new List<Tour>();
                        if(_listeTour.IsNotEmpty())
                        {
                            foreach (var tour1 in globalListeTour)
                            {
                                if (tour1.IsNull()) continue;
                                foreach (var tour2 in _listeTour)
                                {
                                    if (tour2.IsNull()) continue;
                                    if (!tour2.Title.Equals(tour1.Title) || !tour2.Lieu.Equals(tour1.Lieu)) continue;
                                    concertSupprimer.Add(tour1);
                                    break;
                                }
                            }
                        }
                        

                        //On assigne la nouvelle liste
                        _listeTour = globalListeTour;

                        //On supprime les concerts inutiles
                        foreach (var tour1 in concertSupprimer)
                        {
                            globalListeTour.RemoveElement(tour1);
                        }
                        if (globalListeTour.IsEmpty()) continue;

                        //On écrit dans le fichier pour save
                        if (_listeTour.XmlSerialize(_nameXmlTour).IsFalse())
                        {
                            OnErreurRecherche("Erreur lors de la sauvegarde des concerts de rihanna.");
                            continue;
                        }

                        //Déclenche event
                        OnNewTour(globalListeTour);

                        //On remet à zéro la liste
                        globalListeTour.Clear();
                    }
                    // ReSharper disable once FunctionNeverReturns
                });
                return true;
            }
            catch (Exception)
            {
                OnErreurRecherche("La recherche de nouveaux concerts de rihanna est interrompue.");
                return false;
            }
        }
        
        private void OnErreurRecherche(string obj)
        {
            var handler = ErreurRecherche;
            if (handler != null) handler(obj);
        }

        private void OnNewTour(List<Tour> obj)
        {
            var handler = NewTour;
            if (handler != null) handler(obj);
        }
    }
}
