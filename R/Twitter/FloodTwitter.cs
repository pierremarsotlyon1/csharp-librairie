using System;
using System.Collections.Generic;
using System.Linq;
using Tweetinvi.Core.Enum;
using Tweetinvi.Core.Interfaces;

namespace R.Twitter
{
    public class FloodTwitter : GestionTwitter
    {
        private readonly List<MessageTwitter> _listeMessageTwitter;
        private readonly Object _lockCompteur;
        private long _compteur;

        public FloodTwitter(List<MessageTwitter> listeMessageTwitter = null, string accessToken = null,
            string accessTokenSecret = null, string consumerKey = null,
            string consumerKeySecret = null)
            : base(accessToken, accessTokenSecret, consumerKey, consumerKeySecret)
        {
            _listeMessageTwitter = listeMessageTwitter;
            _compteur = 0;
            _lockCompteur = new Object();
        }

        public void RunFood(string nameFirstUser = null)
        {
            try
            {
                if (nameFirstUser.IsNullOrEmpty() || !CheckInit()) return;

                //On get l'user via son nom
                IUser user = GetUserFromName(nameFirstUser);
                if (user.IsNull()) return;

                //Démarrage du flood
                RunFood(user);
            }
// ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        public void RunFood(IUser firstUser = null)
        {
            try
            {
                //Check initiale
                if (firstUser.IsNull() || !CheckInit()) return;
                //Démarrage du flood
                StartFlood(firstUser);
            }
// ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        private void StartFlood(IUser firstUser = null)
        {
            try
            {
                RThread.DoAsync(() =>
                {
                    //On récup le nombre d'abonnées de l'user
                    long nbAbonnees = GetNumberTotalAbonnees(firstUser);
                    if (nbAbonnees < 0) return;

                    //On récup les abonnées de l'user
                    var listeAbonnees = GetAbonnementsUser(firstUser, 10);
                    var enumerable = listeAbonnees as IList<IUser> ?? listeAbonnees.ToList();
                    if (enumerable.IsEmpty()) return;

                    //Pour chaque abonnées on envoie le message qi correspond à sa langue
                    foreach (var abonnee in enumerable)
                    {
                        //On récup la langue de l'abo
                        Language? langue = GetLanguageDefaultUser(abonnee);
                        if (langue.IsNull()) continue;

                        //On récup le bon message
                        string message = null;
                        foreach (
                            MessageTwitter messageTwitter in
                                _listeMessageTwitter.Where(messageTwitter => messageTwitter.Language == langue))
                        {
                            message = messageTwitter.Message;
                        }
                        if (message.IsNullOrEmpty()) continue;

                        //On envoie un message privé à l'abonnée
                        SendMessagePrivate(message, abonnee);
                        IncrementCompteur();
                        //On fait pareil avec les abonnées de l'abonné
                        StartFlood(abonnee);
                    }
                });
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        private bool CheckInit()
        {
            try
            {
                return !_listeMessageTwitter.IsEmpty() && Connexion;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void IncrementCompteur()
        {
            lock (_lockCompteur)
            {
                try
                {
                    _compteur++;
                }
// ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }
            }
        }

        public long GetCompteur()
        {
            lock (_lockCompteur)
            {
                try
                {
                    return _compteur;
                }
                catch (Exception)
                {
                    return -1;
                }
            }
        }
    }
}