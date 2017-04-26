using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Speech.Synthesis;
using System.Windows.Input;
using Google.Apis.Gmail.v1.Data;
using R.Gmail;
using R.ScanArticle;
using R.Web;
using R.Youtube;

// ReSharper disable once CheckNamespace
namespace R
{
    public class GestionIa
    {
        private GestionScan _gestionScan;
        private readonly GestionChrome _gestionChrome;
        private readonly GestionKeyEvent _gestionKeyEvent;
        private readonly GestionMeteo _gestionMeteo;
        private readonly List<Article> _listeArticle;
        private readonly SpeechSynthesizer _synth;
        private Object _lock;

        public GestionIa()
        {
            //Init
            _lock = new object();
            _synth = new SpeechSynthesizer {Rate = 1, Volume = 100};
            _gestionMeteo = new GestionMeteo();
            _gestionScan = new GestionScan();
            _gestionKeyEvent = new GestionKeyEvent();
            _gestionChrome = new GestionChrome();
            _listeArticle = new List<Article>();
            var gestionBonCoin = new GestionBonCoin();
            _synth.SetOutputToDefaultAudioDevice();
            var gestionRihanna = new Rihannanow();
            var gestionYoutube = new GestionYoutube(new Dictionary<string, string>
            {
                {
                    "Rihanna", 
                    "https://www.youtube.com/user/RihannaVEVO/videos"
                },
                {
                    "Katy Perry",
                    "https://www.youtube.com/user/KatyPerryVEVO/videos"
                }
            });

            //Event
            NetworkChange.NetworkAddressChanged += ChangeNetwork;
            GestionScan.NewArticle += NewArticle;
           // GestionScan.ErrorGetElementPage += ErrorGetElementPage;
            _gestionKeyEvent.GetPressKey(() =>
            {
                if (_listeArticle.IsEmpty()) return;
                var article = _listeArticle.GetLastElement();
                //Afficher page web
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    try
                    {
                        if (_gestionChrome.OpenUrl(article.Url))
                            _listeArticle.RemoveElement(article);
                    }
                        // ReSharper disable once EmptyGeneralCatchClause
                    catch (Exception)
                    {
                    }
                }
                //Speak contenu article
                if (!_gestionKeyEvent.KeyDown("y")) return;
                if (article.IsNotNull()) Parler(article.ContenuArticle);
            });

            //Accueil
            Parler("Bonjour Monsieur Marsot");

            //Meteo
            //GetMeteo();
            //Lancement LeBonCoin
            gestionBonCoin.NewArticle += NewArticleBonCoin;
            gestionBonCoin.ErreurRecherche += ErreurRechercheBonCoin;
            gestionBonCoin.SearchNewDemande(
                "http://www.leboncoin.fr/annonces/demandes/rhone_alpes/occasions/?f=a&th=1&q=webmaster");

            //Lancement RihannaNow
            gestionRihanna.NewTour += NewTourRihanna;
            gestionRihanna.RechercheTour(
                "http://www.rihannanow.com/wp-admin/admin-ajax.php?action=tourdataset&offset=0&pagesize=11&filter=");

            //Lancement Youtube
            gestionYoutube.NewVideo += NewVideoYoutube;
            gestionYoutube.Erreur += ErreurYoutube;
            gestionYoutube.RechercheVideo();

            //Lancement Gmail
            var gestiongmail = new GestionGmail();
            gestiongmail.NewMessage += NewMessageGmail;
            gestiongmail.RunMessageUnRead();

            //Lancement HelloBiz
            var hellobiz = new GestionHelloBiz();
            hellobiz.NewArticle += NewArticleHelloBiz;
            hellobiz.Erreur += ErreurHelloBiz;
            hellobiz.GetNewArticle("http://hellobiz.fr/");
        }

        private void ErreurHelloBiz(string obj)
        {
            try
            {
                if (obj.IsNullOrEmpty()) return;
                Parler(obj);
            }
            catch (Exception)
            {
                // ReSharper disable once RedundantJumpStatement
                return;
            }
        }

        private void NewArticleHelloBiz(List<ArticleHelloBiz> obj)
        {
            try
            {
                if (obj.IsEmpty()) return;
                var parler = "Monsieur, il semblerait que vous ayez des nouveaux articles en provenance d'HelloBiz.";

                var count = 1;
                foreach (var articleHelloBiz in obj.Where(articleHelloBiz => !articleHelloBiz.Titre.IsNullOrEmpty()))
                {
                    parler += "Article numéro : " + count+".";
                    parler += articleHelloBiz.Titre+".";
                    count++;
                }
                Parler(parler);
            }
            catch (Exception)
            {
                // ReSharper disable once RedundantJumpStatement
                return;
            }
        }

        private void NewMessageGmail(List<Message> obj)
        {
            try
            {
                if (obj.IsEmpty()) return;
                string parler = "Monsieur, vous avez des nouveaux messages non lus sur Gmail. Voici les sujets des messages :";
                var count = 1;
                foreach (var subject in from message in obj
                    where !message.IsNull()
                    where !message.Payload.Headers.IsEmpty()
                    select (from messagePartHeader in message.Payload.Headers
                        where
                            !messagePartHeader.Name.IsNullOrEmpty() &&
                            !messagePartHeader.Name.ToLower().IsNotEquals("subject")
                        select messagePartHeader.Value).FirstOrDefault()
                    into subject
                    where !subject.IsNullOrEmpty()
                    select subject)
                {
                    parler += "Message numéro : " + count + ".";
                    parler += subject + ".";
                    count++;
                }
                parler += "Fin des nouveaux messages.";
                Parler(parler);
            }
            catch (Exception)
            {
                // ReSharper disable once RedundantJumpStatement
                return;
            }
        }

        private void ErreurYoutube(string obj)
        {
            try
            {
                if (obj.IsNullOrEmpty()) return;
                Parler(obj);
            }
            catch (Exception)
            {
                // ReSharper disable once RedundantJumpStatement
                return;
            }
        }

        private void NewVideoYoutube(List<Video> obj, string artiste)
        {
            try
            {
                if (obj.IsEmpty() || artiste.IsNullOrEmpty()) return;
                Parler("Des nouvelles vidéos de " + artiste + " sont disponibles sur Youtube.");
                foreach (var video in obj)
                {
                    Parler(video.Titre);
                }
            }
            catch (Exception)
            {
                // ReSharper disable once RedundantJumpStatement
                return;
            }
        }

        private void NewTourRihanna(List<Tour> obj)
        {
            try
            {
                if (obj.IsEmpty()) return;
                string parler = obj.Where(tour => !tour.IsNull()).Aggregate("Des nouveaux concerts de Rihanna sont prévus.", (current, tour) => current + ("A " + tour.Lieu + " le " + tour.Day + " " + tour.Month + ", le titre du concert est : " + tour.Title));

                Parler(parler);
            }
            catch (Exception)
            {
                // ReSharper disable once RedundantJumpStatement
                return;
            }
        }

        private void ErreurRechercheBonCoin(string obj)
        {
            try
            {
                Parler(obj);
            }
            catch (Exception)
            {
                // ReSharper disable once RedundantJumpStatement
                return;
            }
        }

        private void NewArticleBonCoin(List<ArticleLeBonCoin> obj)
        {
            try
            {
                if (obj.IsNull() || obj.IsEmpty()) return;
                Parler("Monsieur, il semblerait que vous ayez de nouveaux articles sur le bon coin.");
            }
            catch (Exception)
            {
                // ReSharper disable once RedundantJumpStatement
                return;
            }
        }

        /// <summary>
        ///     Méthode qui se déclenche lorsqu'une erreur de récupération de donnée se produit dans une page web
        /// </summary>
        /// <param name="tab"></param>
        private void ErrorGetElementPage(string[] tab)
        {
            try
            {
                if (tab.IsEmpty()) return;
                foreach (var s in tab)
                {
                    Parler(s);
                }
            }
            catch (Exception)
            {
                // ReSharper disable once RedundantJumpStatement
                return;
            }
        }

        /// <summary>
        ///     Méthode qui se déclenche lorsqu'un nouvel article est découvert
        /// </summary>
        /// <param name="obj">L'article</param>
        private void NewArticle(Article obj)
        {
            try
            {
                if (obj.IsNull()) return;
                _listeArticle.Add(obj);
                Parler("Un nouvel article vient d'être publié");
            }
                // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///     Méthode qui se déclenche lorsqu'un changement d'adresse ip est trouvé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeNetwork(object sender, EventArgs e)
        {
            Parler("Changement d'une adresse ip.");
        }

        /// <summary>
        ///     Permet de speak l'heure
        /// </summary>
        private void GetHeure()
        {
            try
            {
                Parler(RDateTime.GetTime());
            }
            catch (Exception)
            {
                // ReSharper disable once RedundantJumpStatement
                return;
            }
        }

        /// <summary>
        ///     Permet de speak la météo
        /// </summary>
        private void GetMeteo()
        {
            try
            {
                Parler(_gestionMeteo.GetConditionOneCity("Montélimar"));
            }
                // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///     Permet de speak
        /// </summary>
        /// <param name="str">La phrase à dire</param>
        /// <returns>bool</returns>
        public bool Parler(string str)
        {
            try
            {
                lock (_lock)
                {
                    if (str.IsNullOrEmpty()) return false;
                    _synth.Speak(str);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}